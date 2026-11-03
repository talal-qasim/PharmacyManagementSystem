using System;
using System.Windows.Input;
using Common.MVVM;
using GalaSoft.MvvmLight.Messaging;
using PharmacyApp.EntityFramework;
using PharmacyApp.Models;

namespace PharmacyApp.ViewModels
{
    public class ManufacturerViewModel : BindableViewModel
    {
        public ManufacturerViewModel() { }

        private Manufacturer _Manufacturer;
        public Manufacturer Manufacturer
        {
            get => _Manufacturer;
            set { if (_Manufacturer != value) { _Manufacturer = value; OnPropertyChanged("Manufacturer"); } }
        }

        private string _ValidationMessage;
        public string ValidationMessage
        {
            get => _ValidationMessage;
            set { if (_ValidationMessage != value) { _ValidationMessage = value; OnPropertyChanged("ValidationMessage"); } }
        }

        private bool Validate()
        {
            if (Manufacturer == null || string.IsNullOrWhiteSpace(Manufacturer.Name))
            {
                ValidationMessage = "Manufacturer name is required.";
                return false;
            }
            ValidationMessage = string.Empty;
            return true;
        }

        RelayCommand _SaveCommand;
        public ICommand SaveCommand
        {
            get
            {
                if (_SaveCommand == null) { _SaveCommand = new RelayCommand(async p => await ExecuteSaveCommand(p)); }
                return _SaveCommand;
            }
        }

        RelayCommand _CancelCommand;
        public ICommand CancelCommand
        {
            get
            {
                if (_CancelCommand == null) { _CancelCommand = new RelayCommand(p => this.ExecuteCancelCommand(p)); }
                return _CancelCommand;
            }
        }

        private async System.Threading.Tasks.Task ExecuteSaveCommand(object parameter)
        {
            try
            {
                if (!Validate()) return;

                if (Manufacturer.Id == 0)
                {
                    var inserted = await ManufacturerDAL.Insert(Manufacturer);
                    if (inserted != null)
                    {
                        Messenger.Default.Send(new EntityMessage<Manufacturer>(inserted, EntityMessageType.Added));
                        CloseDialog = true;
                    }
                }
                else
                {
                    var updated = await ManufacturerDAL.Update(Manufacturer);
                    if (updated != null)
                    {
                        Messenger.Default.Send(new EntityMessage<Manufacturer>(updated, EntityMessageType.Changed));
                        CloseDialog = true;
                    }
                }
            }
            catch (Exception ex) { GlobalModels.FoundExceptionUI(ex); }
        }

        private void ExecuteCancelCommand(object parameter)
        {
            try { CloseDialog = false; }
            catch (Exception ex) { GlobalModels.FoundExceptionUI(ex); }
        }
    }
}
