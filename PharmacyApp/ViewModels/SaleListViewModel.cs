using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using Common.MVVM;
using GalaSoft.MvvmLight.Messaging;
using MyMessageBox;
using PharmacyApp.EntityFramework;
using PharmacyApp.Models;
using PharmacyApp.Views;

namespace PharmacyApp.ViewModels
{
    public class SaleListViewModel : BindableViewModel
    {
        public SaleListViewModel()
        {
            try
            {
                Messenger.Default.Register<EntityMessage<Sale>>(this, OnMessage);
                LoadList();
            }
            catch (Exception ex)
            {
                GlobalModels.FoundExceptionUI(ex);
            }
        }

        private ObservableCollection<Sale> _Sales;
        public ObservableCollection<Sale> Sales
        {
            get => _Sales;
            set
            {
                if (_Sales != value)
                {
                    _Sales = value;
                    OnPropertyChanged("Sales");
                }
            }
        }

        private Sale _SelectedSale;
        public Sale SelectedSale
        {
            get => _SelectedSale;
            set
            {
                if (_SelectedSale != value)
                {
                    _SelectedSale = value;
                    OnPropertyChanged("SelectedSale");
                    IsRowSelected = _SelectedSale != null;
                }
            }
        }

        private bool _IsRowSelected;
        public bool IsRowSelected
        {
            get => _IsRowSelected;
            set
            {
                if (_IsRowSelected != value)
                {
                    _IsRowSelected = value;
                    OnPropertyChanged("IsRowSelected");
                }
            }
        }

        private async void LoadList()
        {
            try
            {
                var result = await SaleDAL.GetSales();
                Sales = result.ToObservableCollection();
            }
            catch (Exception ex)
            {
                GlobalModels.FoundExceptionUI(ex);
            }
        }

        RelayCommand _AddCommand;
        public ICommand AddCommand
        {
            get
            {
                if (_AddCommand == null)
                {
                    _AddCommand = new RelayCommand(p => this.ExecuteAddCommand(p));
                }
                return _AddCommand;
            }
        }

        RelayCommand _CancelSaleCommand;
        public ICommand CancelSaleCommand
        {
            get
            {
                if (_CancelSaleCommand == null)
                {
                    _CancelSaleCommand = new RelayCommand(async p => await ExecuteCancelSaleCommand(p),
                        p => IsRowSelected && SelectedSale.Status == SaleStatusEnum.Completed);
                }
                return _CancelSaleCommand;
            }
        }

        private void ExecuteAddCommand(object parameter)
        {
            try
            {
                var viewModel = new SaleCartViewModel();
                var view = new SaleCartView
                {
                    DataContext = viewModel
                };
                view.ShowDialog();
            }
            catch (Exception ex)
            {
                GlobalModels.FoundExceptionUI(ex);
            }
        }

        private async System.Threading.Tasks.Task ExecuteCancelSaleCommand(object parameter)
        {
            try
            {
                var result = CustomMessageBoxView.Show(
                    "Cancel Sale",
                    $"Are you sure you want to cancel Sale #{SelectedSale.Id}? This will restore stock to inventory.",
                    MessageBoxIconType.Warning,
                    MessageBoxButtonsType.YesNo);

                if (result == MessageBoxResultType.Yes)
                {
                    bool cancelled = await SalesTransactionDAL.CancelSale(SelectedSale.Id, "User cancelled sale via UI");
                    if (cancelled)
                    {
                        var refreshed = await SaleDAL.GetSaleById(SelectedSale.Id);
                        if (refreshed != null)
                        {
                            Messenger.Default.Send(new EntityMessage<Sale>(refreshed, EntityMessageType.Changed));
                        }
                        CustomMessageBoxView.Show("Success", "Sale cancelled and stock restored.",
                            MessageBoxIconType.Success, MessageBoxButtonsType.Ok);
                    }
                    else
                    {
                        GlobalModels.FoundExceptionUI(new Exception("Failed to cancel sale. It may already be cancelled or have missing stock batches."));
                    }
                }
            }
            catch (Exception ex)
            {
                GlobalModels.FoundExceptionUI(ex);
            }
        }

        void OnMessage(EntityMessage<Sale> message)
        {
            switch (message.MessageType)
            {
                case EntityMessageType.Added:
                    Sales.Insert(0, message.Entity);
                    break;
                case EntityMessageType.Changed:
                    {
                        var found = Sales.FirstOrDefault(x => x.Id == message.Entity.Id);
                        if (found != null)
                        {
                            var index = Sales.IndexOf(found);
                            Sales.Remove(found);
                            Sales.Insert(index, message.Entity);
                        }
                    }
                    break;
                case EntityMessageType.Deleted:
                    {
                        var found = Sales.FirstOrDefault(x => x.Id == message.Entity.Id);
                        if (found != null)
                        {
                            Sales.Remove(found);
                        }
                    }
                    break;
            }
        }
    }
}
