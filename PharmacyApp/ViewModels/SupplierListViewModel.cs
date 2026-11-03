using System;
using System.Linq;
using System.Windows.Input;
using Common.MVVM;
using GalaSoft.MvvmLight.Messaging;
using MyMessageBox;
using PharmacyApp.EntityFramework;
using PharmacyApp.Models;
using PharmacyApp.Views;
using System.Collections.ObjectModel;

namespace PharmacyApp.ViewModels
{
    public class SupplierListViewModel : BindableViewModel
    {
        public SupplierListViewModel()
        {
            try
            {
                Messenger.Default.Register<EntityMessage<Supplier>>(this, OnMessage);
                LoadList();
            }
            catch (Exception ex) { GlobalModels.FoundExceptionUI(ex); }
        }

        private ObservableCollection<Supplier> _Suppliers;
        public ObservableCollection<Supplier> Suppliers
        {
            get => _Suppliers;
            set { if (_Suppliers != value) { _Suppliers = value; OnPropertyChanged("Suppliers"); } }
        }

        private Supplier _SelectedSupplier;
        public Supplier SelectedSupplier
        {
            get => _SelectedSupplier;
            set
            {
                if (_SelectedSupplier != value)
                {
                    _SelectedSupplier = value;
                    OnPropertyChanged("SelectedSupplier");
                    IsRowSelected = _SelectedSupplier != null;
                }
            }
        }

        private bool _IsRowSelected;
        public bool IsRowSelected
        {
            get => _IsRowSelected;
            set { if (_IsRowSelected != value) { _IsRowSelected = value; OnPropertyChanged("IsRowSelected"); } }
        }

        private async void LoadList()
        {
            try
            {
                var result = await SupplierDAL.GetSuppliers();
                Suppliers = result.ToObservableCollection();
            }
            catch (Exception ex) { GlobalModels.FoundExceptionUI(ex); }
        }

        RelayCommand _AddCommand;
        public ICommand AddCommand
        {
            get
            {
                if (_AddCommand == null) { _AddCommand = new RelayCommand(p => this.ExecuteAddCommand(p)); }
                return _AddCommand;
            }
        }

        RelayCommand _UpdateCommand;
        public ICommand UpdateCommand
        {
            get
            {
                if (_UpdateCommand == null) { _UpdateCommand = new RelayCommand(async p => await ExecuteUpdateCommand(p), p => IsRowSelected); }
                return _UpdateCommand;
            }
        }

        RelayCommand _DeleteCommand;
        public ICommand DeleteCommand
        {
            get
            {
                if (_DeleteCommand == null) { _DeleteCommand = new RelayCommand(async p => await ExecuteDeleteCommand(p), p => IsRowSelected); }
                return _DeleteCommand;
            }
        }

        private void ExecuteAddCommand(object parameter)
        {
            try
            {
                var viewModel = new SupplierViewModel { Supplier = new Supplier() };
                var view = new SupplierView { DataContext = viewModel };
                view.ShowDialog();
            }
            catch (Exception ex) { GlobalModels.FoundExceptionUI(ex); }
        }

        private async System.Threading.Tasks.Task ExecuteUpdateCommand(object parameter)
        {
            try
            {
                var viewModel = new SupplierViewModel { Supplier = await SupplierDAL.GetSupplierById(SelectedSupplier.Id) };
                var view = new SupplierView { DataContext = viewModel };
                view.ShowDialog();
            }
            catch (Exception ex) { GlobalModels.FoundExceptionUI(ex); }
        }

        private async System.Threading.Tasks.Task ExecuteDeleteCommand(object parameter)
        {
            try
            {
                var result = CustomMessageBoxView.Show(
                    "Delete Supplier",
                    $"Are you sure you want to delete '{SelectedSupplier.Name}'?",
                    MessageBoxIconType.Warning,
                    MessageBoxButtonsType.YesNo);

                if (result == MessageBoxResultType.Yes)
                {
                    bool deleted = await SupplierDAL.Delete(SelectedSupplier);
                    if (deleted)
                    {
                        Messenger.Default.Send(new EntityMessage<Supplier>(SelectedSupplier, EntityMessageType.Deleted));
                    }
                }
            }
            catch (Exception ex) { GlobalModels.FoundExceptionUI(ex); }
        }

        void OnMessage(EntityMessage<Supplier> message)
        {
            switch (message.MessageType)
            {
                case EntityMessageType.Added:
                    Suppliers.Add(message.Entity);
                    break;
                case EntityMessageType.Changed:
                    {
                        var found = Suppliers.FirstOrDefault(x => x.Id == message.Entity.Id);
                        if (found != null)
                        {
                            var index = Suppliers.IndexOf(found);
                            Suppliers.Remove(found);
                            Suppliers.Insert(index, message.Entity);
                        }
                    }
                    break;
                case EntityMessageType.Deleted:
                    {
                        var found = Suppliers.FirstOrDefault(x => x.Id == message.Entity.Id);
                        if (found != null)
                        {
                            Suppliers.Remove(found);
                        }
                    }
                    break;
            }
        }
    }
}
