using System;
using System.Collections.ObjectModel;
using System.Windows.Input;
using Common.MVVM;
using GalaSoft.MvvmLight.Messaging;
using PharmacyApp.EntityFramework;
using PharmacyApp.Models;

namespace PharmacyApp.ViewModels
{
    public class PurchaseItemViewModel : BindableViewModel
    {
        #region Constructor

        public PurchaseItemViewModel()
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

        private PurchaseItem _PurchaseItem;
        public PurchaseItem PurchaseItem
        {
            get => _PurchaseItem;
            set
            {
                if (_PurchaseItem != value)
                {
                    _PurchaseItem = value;
                    OnPropertyChanged("PurchaseItem");
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

        private ObservableCollection<ProductVariant> _ProductVariants;
        public ObservableCollection<ProductVariant> ProductVariants
        {
            get => _ProductVariants;
            set
            {
                if (_ProductVariants != value)
                {
                    _ProductVariants = value;
                    OnPropertyChanged("ProductVariants");
                }
            }
        }

        #endregion

        #region Methods

        private async void LoadLookups()
        {
            try
            {
                var variants = await ProductVariantDAL.GetAllProductVariants();
                ProductVariants = variants.ToObservableCollection();
            }
            catch (Exception ex)
            {
                GlobalModels.FoundExceptionUI(ex);
            }
        }

        private bool Validate()
        {
            if (PurchaseItem == null || PurchaseItem.ProductVariantId == 0)
            {
                ValidationMessage = "Please select a product variant.";
                return false;
            }

            if (string.IsNullOrWhiteSpace(PurchaseItem.BatchNumber))
            {
                ValidationMessage = "Batch number is required (Section 21).";
                return false;
            }

            if (PurchaseItem.Quantity <= 0)
            {
                ValidationMessage = "Quantity must be greater than 0.";
                return false;
            }

            if (PurchaseItem.PurchaseCost < 0)
            {
                ValidationMessage = "Purchase cost cannot be negative.";
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

                if (PurchaseItem.Id == 0)
                {
                    var inserted = await PurchaseItemDAL.Insert(PurchaseItem);
                    if (inserted != null)
                    {
                        Messenger.Default.Send(new EntityMessage<PurchaseItem>(inserted, EntityMessageType.Added));
                        CloseDialog = true;
                    }
                    else
                    {
                        ValidationMessage = "Could not add item — the purchase may no longer be in Draft status.";
                    }
                }
                else
                {
                    var updated = await PurchaseItemDAL.Update(PurchaseItem);
                    if (updated != null)
                    {
                        Messenger.Default.Send(new EntityMessage<PurchaseItem>(updated, EntityMessageType.Changed));
                        CloseDialog = true;
                    }
                    else
                    {
                        ValidationMessage = "Could not update item — the purchase may no longer be in Draft status.";
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