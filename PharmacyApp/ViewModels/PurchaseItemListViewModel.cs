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
    public class PurchaseItemListViewModel : BindableViewModel
    {
        public int PurchaseId { get; private set; }

        public PurchaseItemListViewModel(int purchaseId, Purchase purchase)
        {
            PurchaseId = purchaseId;
            Purchase = purchase;
            try
            {
                Messenger.Default.Register<EntityMessage<PurchaseItem>>(this, OnMessage);
                LoadList();
            }
            catch (Exception ex)
            {
                GlobalModels.FoundExceptionUI(ex);
            }
        }

        private Purchase _Purchase;
        public Purchase Purchase
        {
            get => _Purchase;
            set { if (_Purchase != value) { _Purchase = value; OnPropertyChanged("Purchase"); } }
        }

        private ObservableCollection<PurchaseItem> _PurchaseItems;
        public ObservableCollection<PurchaseItem> PurchaseItems
        {
            get => _PurchaseItems;
            set
            {
                if (_PurchaseItems != value)
                {
                    _PurchaseItems = value;
                    OnPropertyChanged("PurchaseItems");
                    OnPropertyChanged("GrandTotal");
                }
            }
        }

        private PurchaseItem _SelectedPurchaseItem;
        public PurchaseItem SelectedPurchaseItem
        {
            get => _SelectedPurchaseItem;
            set
            {
                if (_SelectedPurchaseItem != value)
                {
                    _SelectedPurchaseItem = value;
                    OnPropertyChanged("SelectedPurchaseItem");
                    IsRowSelected = _SelectedPurchaseItem != null;
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

        public decimal GrandTotal
        {
            get
            {
                if (PurchaseItems == null) return 0;
                return PurchaseItems.Sum(x => x.LineTotal);
            }
        }

        private async void LoadList()
        {
            try
            {
                var result = await PurchaseItemDAL.GetPurchaseItemsByPurchaseId(PurchaseId);
                PurchaseItems = result.ToObservableCollection();
                OnPropertyChanged("GrandTotal");
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
                    _AddCommand = new RelayCommand(p => ExecuteAddCommand(p), p => Purchase != null && Purchase.Status == PurchaseStatusEnum.Draft);
                return _AddCommand;
            }
        }

        RelayCommand _UpdateCommand;
        public ICommand UpdateCommand
        {
            get
            {
                if (_UpdateCommand == null)
                    _UpdateCommand = new RelayCommand(p => ExecuteUpdateCommand(p), p => IsRowSelected && Purchase != null && Purchase.Status == PurchaseStatusEnum.Draft);
                return _UpdateCommand;
            }
        }

        RelayCommand _DeleteCommand;
        public ICommand DeleteCommand
        {
            get
            {
                if (_DeleteCommand == null)
                    _DeleteCommand = new RelayCommand(async p => await ExecuteDeleteCommand(p), p => IsRowSelected && Purchase != null && Purchase.Status == PurchaseStatusEnum.Draft);
                return _DeleteCommand;
            }
        }

        private void ExecuteAddCommand(object parameter)
        {
            try
            {
                var viewModel = new PurchaseItemViewModel
                {
                    PurchaseItem = new PurchaseItem
                    {
                        PurchaseId = PurchaseId,
                        Quantity = 1,
                        ExpiryDate = DateTime.Now.AddYears(1)
                    }
                };

                var view = new PurchaseItemView
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

        private void ExecuteUpdateCommand(object parameter)
        {
            try
            {
                var viewModel = new PurchaseItemViewModel
                {
                    PurchaseItem = new PurchaseItem
                    {
                        Id = SelectedPurchaseItem.Id,
                        PurchaseId = SelectedPurchaseItem.PurchaseId,
                        ProductVariantId = SelectedPurchaseItem.ProductVariantId,
                        BatchNumber = SelectedPurchaseItem.BatchNumber,
                        ExpiryDate = SelectedPurchaseItem.ExpiryDate,
                        Quantity = SelectedPurchaseItem.Quantity,
                        PurchaseCost = SelectedPurchaseItem.PurchaseCost
                    }
                };

                var view = new PurchaseItemView
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

        private async System.Threading.Tasks.Task ExecuteDeleteCommand(object parameter)
        {
            try
            {
                var result = CustomMessageBoxView.Show(
                    "Delete Item",
                    "Are you sure you want to delete this purchase item?",
                    MessageBoxIconType.Warning,
                    MessageBoxButtonsType.YesNo);

                if (result == MessageBoxResultType.Yes)
                {
                    bool deleted = await PurchaseItemDAL.Delete(SelectedPurchaseItem);
                    if (deleted)
                    {
                        Messenger.Default.Send(new EntityMessage<PurchaseItem>(SelectedPurchaseItem, EntityMessageType.Deleted));
                    }
                    else
                    {
                        GlobalModels.FoundExceptionUI(new Exception("Failed to delete. Purchase might not be in Draft state."));
                    }
                }
            }
            catch (Exception ex)
            {
                GlobalModels.FoundExceptionUI(ex);
            }
        }

        void OnMessage(EntityMessage<PurchaseItem> message)
        {
            if (message.Entity.PurchaseId != PurchaseId) return;

            switch (message.MessageType)
            {
                case EntityMessageType.Added:
                    PurchaseItems.Add(message.Entity);
                    OnPropertyChanged("GrandTotal");
                    break;
                case EntityMessageType.Changed:
                    {
                        var found = PurchaseItems.FirstOrDefault(x => x.Id == message.Entity.Id);
                        if (found != null)
                        {
                            var index = PurchaseItems.IndexOf(found);
                            PurchaseItems.Remove(found);
                            PurchaseItems.Insert(index, message.Entity);
                            OnPropertyChanged("GrandTotal");
                        }
                    }
                    break;
                case EntityMessageType.Deleted:
                    {
                        var found = PurchaseItems.FirstOrDefault(x => x.Id == message.Entity.Id);
                        if (found != null)
                        {
                            PurchaseItems.Remove(found);
                            OnPropertyChanged("GrandTotal");
                        }
                    }
                    break;
            }
        }
    }
}
