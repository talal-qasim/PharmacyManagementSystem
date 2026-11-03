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
    /// One editor serves two modes: receiving a brand-new batch (Section 21-23,
    /// ProductVariant selectable) and editing an existing batch's metadata only
    /// (ProductVariant locked — Quantity is never editable here at all, per
    /// InventoryDAL's design; use "Adjust Stock" for quantity changes).
    /// </summary>
    public class BatchViewModel : BindableViewModel
    {
        #region Constructor

        public BatchViewModel()
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

        private Batch _Batch;
        public Batch Batch
        {
            get => _Batch;
            set
            {
                if (_Batch != value)
                {
                    _Batch = value;
                    OnPropertyChanged("Batch");
                    OnPropertyChanged("IsNewBatch");
                }
            }
        }

        /// <summary>
        /// True when receiving a new batch (ProductVariant + initial Quantity are
        /// editable). False when editing an existing batch (those fields lock).
        /// </summary>
        public bool IsNewBatch => Batch != null && Batch.Id == 0;

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
            if (Batch == null || Batch.ProductVariantId == 0)
            {
                ValidationMessage = "Please select a product variant.";
                return false;
            }

            if (string.IsNullOrWhiteSpace(Batch.BatchNumber))
            {
                ValidationMessage = "Batch number is required.";
                return false;
            }

            if (IsNewBatch && Batch.Quantity <= 0)
            {
                ValidationMessage = "Quantity received must be greater than 0.";
                return false;
            }

            if (Batch.PurchaseCost < 0)
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

                if (IsNewBatch)
                {
                    var received = await InventoryDAL.ReceiveBatch(Batch, reference: "Manual batch entry");
                    if (received != null)
                    {
                        Messenger.Default.Send(new EntityMessage<Batch>(received, EntityMessageType.Added));
                        CloseDialog = true;
                    }
                    else
                    {
                        ValidationMessage = "Failed to save batch. Please try again.";
                    }
                }
                else
                {
                    var updated = await BatchDAL.Update(Batch);
                    if (updated != null)
                    {
                        Messenger.Default.Send(new EntityMessage<Batch>(updated, EntityMessageType.Changed));
                        CloseDialog = true;
                    }
                    else
                    {
                        ValidationMessage = "Failed to update batch. Please try again.";
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