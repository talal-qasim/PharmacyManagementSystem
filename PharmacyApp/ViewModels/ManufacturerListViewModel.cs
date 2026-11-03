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
    public class ManufacturerListViewModel : BindableViewModel
    {
        public ManufacturerListViewModel()
        {
            try
            {
                Messenger.Default.Register<EntityMessage<Manufacturer>>(this, OnMessage);
                LoadList();
            }
            catch (Exception ex) { GlobalModels.FoundExceptionUI(ex); }
        }

        private ObservableCollection<Manufacturer> _Manufacturers;
        public ObservableCollection<Manufacturer> Manufacturers
        {
            get => _Manufacturers;
            set { if (_Manufacturers != value) { _Manufacturers = value; OnPropertyChanged("Manufacturers"); } }
        }

        private Manufacturer _SelectedManufacturer;
        public Manufacturer SelectedManufacturer
        {
            get => _SelectedManufacturer;
            set
            {
                if (_SelectedManufacturer != value)
                {
                    _SelectedManufacturer = value;
                    OnPropertyChanged("SelectedManufacturer");
                    IsRowSelected = _SelectedManufacturer != null;
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
                var result = await ManufacturerDAL.GetManufacturers();
                Manufacturers = result.ToObservableCollection();
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
                var viewModel = new ManufacturerViewModel { Manufacturer = new Manufacturer() };
                var view = new ManufacturerView { DataContext = viewModel };
                view.ShowDialog();
            }
            catch (Exception ex) { GlobalModels.FoundExceptionUI(ex); }
        }

        private async System.Threading.Tasks.Task ExecuteUpdateCommand(object parameter)
        {
            try
            {
                var viewModel = new ManufacturerViewModel { Manufacturer = await ManufacturerDAL.GetManufacturerById(SelectedManufacturer.Id) };
                var view = new ManufacturerView { DataContext = viewModel };
                view.ShowDialog();
            }
            catch (Exception ex) { GlobalModels.FoundExceptionUI(ex); }
        }

        private async System.Threading.Tasks.Task ExecuteDeleteCommand(object parameter)
        {
            try
            {
                var result = CustomMessageBoxView.Show(
                    "Delete Manufacturer",
                    $"Are you sure you want to delete '{SelectedManufacturer.Name}'?",
                    MessageBoxIconType.Warning,
                    MessageBoxButtonsType.YesNo);

                if (result == MessageBoxResultType.Yes)
                {
                    bool deleted = await ManufacturerDAL.Delete(SelectedManufacturer);
                    if (deleted)
                    {
                        Messenger.Default.Send(new EntityMessage<Manufacturer>(SelectedManufacturer, EntityMessageType.Deleted));
                    }
                }
            }
            catch (Exception ex) { GlobalModels.FoundExceptionUI(ex); }
        }

        void OnMessage(EntityMessage<Manufacturer> message)
        {
            switch (message.MessageType)
            {
                case EntityMessageType.Added:
                    Manufacturers.Add(message.Entity);
                    break;
                case EntityMessageType.Changed:
                    {
                        var found = Manufacturers.FirstOrDefault(x => x.Id == message.Entity.Id);
                        if (found != null)
                        {
                            var index = Manufacturers.IndexOf(found);
                            Manufacturers.Remove(found);
                            Manufacturers.Insert(index, message.Entity);
                        }
                    }
                    break;
                case EntityMessageType.Deleted:
                    {
                        var found = Manufacturers.FirstOrDefault(x => x.Id == message.Entity.Id);
                        if (found != null)
                        {
                            Manufacturers.Remove(found);
                        }
                    }
                    break;
            }
        }
    }
}
