using System;
using System.Collections.ObjectModel;
using System.Windows.Input;
using Common.MVVM;
using GalaSoft.MvvmLight.Messaging;
using PharmacyApp.EntityFramework;
using PharmacyApp.Models;

namespace PharmacyApp.ViewModels
{
    public class MedicineViewModel : BindableViewModel
    {
        #region Constructor

        public MedicineViewModel()
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

        private Medicine _Medicine;
        public Medicine Medicine
        {
            get => _Medicine;
            set
            {
                if (_Medicine != value)
                {
                    _Medicine = value;
                    OnPropertyChanged("Medicine");
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

        /// <summary>
        /// Combo-box source for MedicineCategory selection.
        /// </summary>
        private ObservableCollection<MedicineCategory> _Categories;
        public ObservableCollection<MedicineCategory> Categories
        {
            get => _Categories;
            set
            {
                if (_Categories != value)
                {
                    _Categories = value;
                    OnPropertyChanged("Categories");
                }
            }
        }

        /// <summary>
        /// Combo-box source for Manufacturer selection.
        /// </summary>
        private ObservableCollection<Manufacturer> _Manufacturers;
        public ObservableCollection<Manufacturer> Manufacturers
        {
            get => _Manufacturers;
            set
            {
                if (_Manufacturers != value)
                {
                    _Manufacturers = value;
                    OnPropertyChanged("Manufacturers");
                }
            }
        }

        /// <summary>
        /// Combo-box source for the Status enum (Active/Inactive).
        /// </summary>
        private ObservableCollection<MedicineStatusEnum> _Statuses;
        public ObservableCollection<MedicineStatusEnum> Statuses
        {
            get => _Statuses;
            set
            {
                if (_Statuses != value)
                {
                    _Statuses = value;
                    OnPropertyChanged("Statuses");
                }
            }
        }

        #endregion

        #region Methods

        private async void LoadLookups()
        {
            try
            {
                var categories = await MedicineCategoryDAL.GetMedicineCategories();
                Categories = categories.ToObservableCollection();

                var manufacturers = await ManufacturerDAL.GetManufacturers();
                Manufacturers = manufacturers.ToObservableCollection();

                Statuses = new ObservableCollection<MedicineStatusEnum>(
                    (MedicineStatusEnum[])Enum.GetValues(typeof(MedicineStatusEnum)));
            }
            catch (Exception ex)
            {
                GlobalModels.FoundExceptionUI(ex);
            }
        }

        private bool Validate()
        {
            if (Medicine == null || string.IsNullOrWhiteSpace(Medicine.Name))
            {
                ValidationMessage = "Medicine name is required.";
                return false;
            }

            if (Medicine.MedicineCategoryId == 0)
            {
                ValidationMessage = "Please select a category.";
                return false;
            }

            if (Medicine.ManufacturerId == 0)
            {
                ValidationMessage = "Please select a manufacturer.";
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

                if (Medicine.Id == 0)
                {
                    var inserted = await MedicineDAL.Insert(Medicine);
                    if (inserted != null)
                    {
                        Messenger.Default.Send(new EntityMessage<Medicine>(inserted, EntityMessageType.Added));
                        CloseDialog = true;
                    }
                }
                else
                {
                    var updated = await MedicineDAL.Update(Medicine);
                    if (updated != null)
                    {
                        Messenger.Default.Send(new EntityMessage<Medicine>(updated, EntityMessageType.Changed));
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