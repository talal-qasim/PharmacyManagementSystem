using System;
using System.Collections.ObjectModel;
using System.Windows.Input;
using Common.MVVM;
using GalaSoft.MvvmLight.Messaging;
using PharmacyApp.EntityFramework;
using PharmacyApp.Models;

namespace PharmacyApp.ViewModels
{
    /// <summary>
    /// Header-only editor (Supplier, date, reference). Items are managed separately
    /// via PurchaseItemListViewModel, same split as Medicine/ProductVariant in Step 3.
    /// </summary>
    public class PurchaseViewModel : BindableViewModel
    {
        #region Constructor

        public PurchaseViewModel()
        {
            try
            {
                LoadLookups();
            }
            catch (Exception ex)
            {
                GlobalModels.FoundExceptionUI(ex);
            }
        }

        #endregion

        #region Properties

        private Purchase _Purchase;
        public Purchase Purchase
        {
            get => _Purchase;
            set
            {
                if (_Purchase != value)
                {
                    _Purchase = value;
                    OnPropertyChanged("Purchase");
                }
            }
        }

        private string _ValidationMessage;
        public string ValidationMessage
        {
            get => _ValidationMessage;
            set
            {
                if (_ValidationMessage != value)
                {
                    _ValidationMessage = value;
                    OnPropertyChanged("ValidationMessage");
                }
            }
        }

        private ObservableCollection<Supplier> _Suppliers;
        public ObservableCollection<Supplier> Suppliers
        {
            get => _Suppliers;
            set
            {
                if (_Suppliers != value)
                {
                    _Suppliers = value;
                    OnPropertyChanged("Suppliers");
                }
            }
        }

        #endregion

        #region Methods

        private async void LoadLookups()
        {
            try
            {
                var suppliers = await SupplierDAL.GetSuppliers();
                Suppliers = suppliers.ToObservableCollection();
            }
            catch (Exception ex)
            {
                GlobalModels.FoundExceptionUI(ex);
            }
        }

        private bool Validate()
        {
            if (Purchase == null || Purchase.SupplierId == 0)
            {
                ValidationMessage = "Please select a supplier.";
                return false;
            }

            ValidationMessage = string.Empty;
            return true;
        }

        #endregion

        #region Commands

        RelayCommand _SaveCommand;
        public ICommand SaveCommand
        {
            get
            {
                if (_SaveCommand == null)
                {
                    _SaveCommand = new RelayCommand(async p => await ExecuteSaveCommand(p));
                }
                return _SaveCommand;
            }
        }

        RelayCommand _CancelCommand;
        public ICommand CancelCommand
        {
            get
            {
                if (_CancelCommand == null)
                {
                    _CancelCommand = new RelayCommand(p => this.ExecuteCancelCommand(p));
                }
                return _CancelCommand;
            }
        }

        #endregion

        #region Command Methods

        private async System.Threading.Tasks.Task ExecuteSaveCommand(object parameter)
        {
            try
            {
                if (!Validate())
                {
                    return;
                }

                if (Purchase.Id == 0)
                {
                    var inserted = await PurchaseDAL.Insert(Purchase);
                    if (inserted != null)
                    {
                        Messenger.Default.Send(new EntityMessage<Purchase>(inserted, EntityMessageType.Added));
                        CloseDialog = true;
                    }
                }
                else
                {
                    var updated = await PurchaseDAL.Update(Purchase);
                    if (updated != null)
                    {
                        Messenger.Default.Send(new EntityMessage<Purchase>(updated, EntityMessageType.Changed));
                        CloseDialog = true;
                    }
                    else
                    {
                        ValidationMessage = "This purchase can no longer be edited (already received).";
                    }
                }
            }
            catch (Exception ex)
            {
                GlobalModels.FoundExceptionUI(ex);
            }
        }

        private void ExecuteCancelCommand(object parameter)
        {
            try
            {
                CloseDialog = false;
            }
            catch (Exception ex)
            {
                GlobalModels.FoundExceptionUI(ex);
            }
        }

        #endregion
    }
}