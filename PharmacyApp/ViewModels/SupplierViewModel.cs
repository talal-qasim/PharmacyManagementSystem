using System;
using System.Windows.Input;
using Common.MVVM;
using GalaSoft.MvvmLight.Messaging;
using PharmacyApp.EntityFramework;
using PharmacyApp.Models;

namespace PharmacyApp.ViewModels
{
    public class SupplierViewModel : BindableViewModel
    {
        public SupplierViewModel() { }

        private Supplier _Supplier;
        public Supplier Supplier
        {
            get => _Supplier;
            set { if (_Supplier != value) { _Supplier = value; OnPropertyChanged("Supplier"); } }
        }

        private string _ValidationMessage;
        public string ValidationMessage
        {
            get => _ValidationMessage;
            set { if (_ValidationMessage != value) { _ValidationMessage = value; OnPropertyChanged("ValidationMessage"); } }
        }

        private bool Validate()
        {
            if (Supplier == null || string.IsNullOrWhiteSpace(Supplier.Name))
            {
                ValidationMessage = "Supplier name is required.";
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

                if (Supplier.Id == 0)
                {
                    var inserted = await SupplierDAL.Insert(Supplier);
                    if (inserted != null)
                    {
                        Messenger.Default.Send(new EntityMessage<Supplier>(inserted, EntityMessageType.Added));
                        CloseDialog = true;
                    }
                }
                else
                {
                    var updated = await SupplierDAL.Update(Supplier);
                    if (updated != null)
                    {
                        Messenger.Default.Send(new EntityMessage<Supplier>(updated, EntityMessageType.Changed));
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
