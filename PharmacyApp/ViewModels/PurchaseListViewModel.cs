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
    public class PurchaseListViewModel : BindableViewModel
    {
        #region Constructor

        public PurchaseListViewModel()
        {
            try
            {
                Messenger.Default.Register<EntityMessage<Purchase>>(this, OnMessage);
                LoadList();
            }
            catch (Exception ex)
            {
                GlobalModels.FoundExceptionUI(ex);
            }
        }

        #endregion

        #region Properties

        private ObservableCollection<Purchase> _Purchases;
        public ObservableCollection<Purchase> Purchases
        {
            get => _Purchases;
            set
            {
                if (_Purchases != value)
                {
                    _Purchases = value;
                    OnPropertyChanged("Purchases");
                }
            }
        }

        private Purchase _SelectedPurchase;
        public Purchase SelectedPurchase
        {
            get => _SelectedPurchase;
            set
            {
                if (_SelectedPurchase != value)
                {
                    _SelectedPurchase = value;
                    OnPropertyChanged("SelectedPurchase");
                    IsRowSelected = _SelectedPurchase != null;
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

        #endregion

        #region Methods

        private async void LoadList()
        {
            try
            {
                var result = await PurchaseDAL.GetPurchases();
                Purchases = result.ToObservableCollection();
            }
            catch (Exception ex)
            {
                GlobalModels.FoundExceptionUI(ex);
            }
        }

        private async System.Threading.Tasks.Task RefreshPurchase(int purchaseId)
        {
            var refreshed = await PurchaseDAL.GetPurchaseById(purchaseId);
            if (refreshed != null)
            {
                Messenger.Default.Send(new EntityMessage<Purchase>(refreshed, EntityMessageType.Changed));
            }
        }

        #endregion

        #region Commands

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

        RelayCommand _UpdateCommand;
        public ICommand UpdateCommand
        {
            get
            {
                if (_UpdateCommand == null)
                {
                    _UpdateCommand = new RelayCommand(async p => await ExecuteUpdateCommand(p),
                        p => IsRowSelected && SelectedPurchase.IsDraft);
                }
                return _UpdateCommand;
            }
        }

        RelayCommand _DeleteCommand;
        public ICommand DeleteCommand
        {
            get
            {
                if (_DeleteCommand == null)
                {
                    _DeleteCommand = new RelayCommand(async p => await ExecuteDeleteCommand(p),
                        p => IsRowSelected && SelectedPurchase.IsDraft);
                }
                return _DeleteCommand;
            }
        }

        RelayCommand _ManageItemsCommand;
        public ICommand ManageItemsCommand
        {
            get
            {
                if (_ManageItemsCommand == null)
                {
                    _ManageItemsCommand = new RelayCommand(p => this.ExecuteManageItemsCommand(p), p => IsRowSelected);
                }
                return _ManageItemsCommand;
            }
        }

        RelayCommand _ReceiveCommand;
        public ICommand ReceiveCommand
        {
            get
            {
                if (_ReceiveCommand == null)
                {
                    _ReceiveCommand = new RelayCommand(async p => await ExecuteReceiveCommand(p),
                        p => IsRowSelected && SelectedPurchase.IsDraft);
                }
                return _ReceiveCommand;
            }
        }

        #endregion

        #region Command Methods

        private void ExecuteAddCommand(object parameter)
        {
            try
            {
                var viewModel = new PurchaseViewModel
                {
                    Purchase = new Purchase
                    {
                        PurchaseDate = DateTime.Now,
                        Status = PurchaseStatusEnum.Draft
                    }
                };

                var view = new PurchaseView
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

        private async System.Threading.Tasks.Task ExecuteUpdateCommand(object parameter)
        {
            try
            {
                var viewModel = new PurchaseViewModel
                {
                    Purchase = await PurchaseDAL.GetPurchaseById(SelectedPurchase.Id)
                };

                var view = new PurchaseView
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
                    "Delete Purchase",
                    $"Are you sure you want to delete Draft purchase #{SelectedPurchase.Id}?",
                    MessageBoxIconType.Warning,
                    MessageBoxButtonsType.YesNo);

                if (result == MessageBoxResultType.Yes)
                {
                    bool deleted = await PurchaseDAL.Delete(SelectedPurchase);
                    if (deleted)
                    {
                        Messenger.Default.Send(new EntityMessage<Purchase>(SelectedPurchase, EntityMessageType.Deleted));
                    }
                    else
                    {
                        GlobalModels.FoundExceptionUI(new Exception("Only Draft purchases can be deleted."));
                    }
                }
            }
            catch (Exception ex)
            {
                GlobalModels.FoundExceptionUI(ex);
            }
        }

        private void ExecuteManageItemsCommand(object parameter)
        {
            try
            {
                var viewModel = new PurchaseItemListViewModel(SelectedPurchase.Id, SelectedPurchase);

                var view = new PurchaseItemListView
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

        private async System.Threading.Tasks.Task ExecuteReceiveCommand(object parameter)
        {
            try
            {
                var result = CustomMessageBoxView.Show(
                    "Receive Purchase",
                    $"Receive purchase #{SelectedPurchase.Id}? This will create batches and increase stock immediately, and cannot be undone (Section 23/27).",
                    MessageBoxIconType.Warning,
                    MessageBoxButtonsType.YesNo);

                if (result == MessageBoxResultType.Yes)
                {
                    bool received = await PurchasingDAL.ReceivePurchase(SelectedPurchase.Id);
                    if (received)
                    {
                        await RefreshPurchase(SelectedPurchase.Id);
                        CustomMessageBoxView.Show("Success", "Purchase received. Stock has been updated.",
                            MessageBoxIconType.Success, MessageBoxButtonsType.Ok);
                    }
                    else
                    {
                        CustomMessageBoxView.Show("Cannot Receive", "This purchase has no items, or is not in Draft status.",
                            MessageBoxIconType.Warning, MessageBoxButtonsType.Ok);
                    }
                }
            }
            catch (Exception ex)
            {
                GlobalModels.FoundExceptionUI(ex);
            }
        }

        #endregion

        #region Messenger Handler

        void OnMessage(EntityMessage<Purchase> message)
        {
            switch (message.MessageType)
            {
                case EntityMessageType.Added:
                    Purchases.Insert(0, message.Entity);
                    break;
                case EntityMessageType.Changed:
                    {
                        var found = Purchases.FirstOrDefault(x => x.Id == message.Entity.Id);
                        if (found != null)
                        {
                            var index = Purchases.IndexOf(found);
                            Purchases.Remove(found);
                            Purchases.Insert(index, message.Entity);
                        }
                    }
                    break;
                case EntityMessageType.Deleted:
                    {
                        var found = Purchases.FirstOrDefault(x => x.Id == message.Entity.Id);
                        if (found != null)
                        {
                            Purchases.Remove(found);
                        }
                    }
                    break;
            }
        }

        #endregion
    }
}