using System;
using System.Collections.ObjectModel;
using System.Windows.Input;
using Common.MVVM;
using GalaSoft.MvvmLight.Messaging;
using PharmacyApp.EntityFramework;
using PharmacyApp.Models;

namespace PharmacyApp.ViewModels
{
    public class ProductVariantViewModel : BindableViewModel
    {
        #region Constructor

        public ProductVariantViewModel()
        {
            try
            {
                DosageForms = new ObservableCollection<DosageFormEnum>(
                    (DosageFormEnum[])Enum.GetValues(typeof(DosageFormEnum)));

                StockUnits = new ObservableCollection<StockUnitEnum>(
                    (StockUnitEnum[])Enum.GetValues(typeof(StockUnitEnum)));
            }
            catch (Exception ex)
            {
                GlobalModels.FoundExceptionUI(ex);
            }
        }

        #endregion

        #region Properties

        private ProductVariant _ProductVariant;
        public ProductVariant ProductVariant
        {
            get => _ProductVariant;
            set
            {
                if (_ProductVariant != value)
                {
                    _ProductVariant = value;
                    OnPropertyChanged("ProductVariant");
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

        private ObservableCollection<DosageFormEnum> _DosageForms;
        public ObservableCollection<DosageFormEnum> DosageForms
        {
            get => _DosageForms;
            set
            {
                if (_DosageForms != value)
                {
                    _DosageForms = value;
                    OnPropertyChanged("DosageForms");
                }
            }
        }

        private ObservableCollection<StockUnitEnum> _StockUnits;
        public ObservableCollection<StockUnitEnum> StockUnits
        {
            get => _StockUnits;
            set
            {
                if (_StockUnits != value)
                {
                    _StockUnits = value;
                    OnPropertyChanged("StockUnits");
                }
            }
        }

        #endregion

        #region Methods

        private bool Validate()
        {
            if (ProductVariant == null || string.IsNullOrWhiteSpace(ProductVariant.Strength))
            {
                ValidationMessage = "Strength is required.";
                return false;
            }

            if (string.IsNullOrWhiteSpace(ProductVariant.PackageSize))
            {
                ValidationMessage = "Package size is required.";
                return false;
            }

            if (ProductVariant.UnitsPerPackage <= 0)
            {
                ValidationMessage = "Units per package must be at least 1 (Section 9 — 1 tube/bottle/vial = 1 stock unit).";
                return false;
            }

            if (ProductVariant.SellingPrice <= 0)
            {
                ValidationMessage = "Selling price must be greater than 0.";
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

                if (ProductVariant.Id == 0)
                {
                    var inserted = await ProductVariantDAL.Insert(ProductVariant);
                    if (inserted != null)
                    {
                        Messenger.Default.Send(new EntityMessage<ProductVariant>(inserted, EntityMessageType.Added));
                        CloseDialog = true;
                    }
                }
                else
                {
                    var updated = await ProductVariantDAL.Update(ProductVariant);
                    if (updated != null)
                    {
                        Messenger.Default.Send(new EntityMessage<ProductVariant>(updated, EntityMessageType.Changed));
                        CloseDialog = true;
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