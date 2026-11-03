using System;
using System.Windows.Input;
using Common.MVVM;
using GalaSoft.MvvmLight.Messaging;
using PharmacyApp.EntityFramework;
using PharmacyApp.Models;

namespace PharmacyApp.ViewModels
{
    public class MedicineCategoryViewModel : BindableViewModel
    {
        #region Constructor

        public MedicineCategoryViewModel()
        {
        }

        #endregion

        #region Properties

        private MedicineCategory _MedicineCategory;
        public MedicineCategory MedicineCategory
        {
            get => _MedicineCategory;
            set
            {
                if (_MedicineCategory != value)
                {
                    _MedicineCategory = value;
                    OnPropertyChanged("MedicineCategory");
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

        #endregion

        #region Methods

        private bool Validate()
        {
            if (MedicineCategory == null || string.IsNullOrWhiteSpace(MedicineCategory.Name))
            {
                ValidationMessage = "Category name is required.";
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

                if (MedicineCategory.Id == 0)
                {
                    var inserted = await MedicineCategoryDAL.Insert(MedicineCategory);
                    if (inserted != null)
                    {
                        Messenger.Default.Send(new EntityMessage<MedicineCategory>(inserted, EntityMessageType.Added));
                        CloseDialog = true;
                    }
                }
                else
                {
                    var updated = await MedicineCategoryDAL.Update(MedicineCategory);
                    if (updated != null)
                    {
                        Messenger.Default.Send(new EntityMessage<MedicineCategory>(updated, EntityMessageType.Changed));
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