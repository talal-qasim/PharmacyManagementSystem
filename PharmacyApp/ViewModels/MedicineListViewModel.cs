using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using Common.MVVM;
using GalaSoft.MvvmLight.Messaging;
using MyMessageBox;
using PharmacyApp.EntityFramework;
using PharmacyApp.Models;
using PharmacyApp.Views;

namespace PharmacyApp.ViewModels
{
    public class MedicineListViewModel : BindableViewModel
    {
        #region Constructor

        public MedicineListViewModel()
        {
            try
            {
                Messenger.Default.Register<EntityMessage<Medicine>>(this, OnMessage);
                LoadList();
            }
            catch (Exception ex)
            {
                GlobalModels.FoundExceptionUI(ex);
            }
        }

        #endregion

        #region Properties

        private ObservableCollection<Medicine> _Medicines;
        public ObservableCollection<Medicine> Medicines
        {
            get => _Medicines;
            set
            {
                if (_Medicines != value)
                {
                    _Medicines = value;
                    OnPropertyChanged("Medicines");
                }
            }
        }

        private Medicine _SelectedMedicine;
        public Medicine SelectedMedicine
        {
            get => _SelectedMedicine;
            set
            {
                if (_SelectedMedicine != value)
                {
                    _SelectedMedicine = value;
                    OnPropertyChanged("SelectedMedicine");
                    IsRowSelected = _SelectedMedicine != null;
                }
            }
        }

        private bool _IsRowSelected;
        public bool IsRowSelected
        {
            get => _IsRowSelected;
            set
            {
                if (_IsRowSelected != value)
                {
                    _IsRowSelected = value;
                    OnPropertyChanged("IsRowSelected");
                }
            }
        }

        #endregion

        #region Methods

        private async void LoadList()
        {
            try
            {
                var result = await MedicineDAL.GetMedicines();
                Medicines = result.ToObservableCollection();
            }
            catch (Exception ex)
            {
                GlobalModels.FoundExceptionUI(ex);
            }
        }

        #endregion

        #region Commands

        RelayCommand _AddCommand;
        public ICommand AddCommand
        {
            get
            {
                if (_AddCommand == null)
                {
                    _AddCommand = new RelayCommand(p => this.ExecuteAddCommand(p));
                }
                return _AddCommand;
            }
        }

        RelayCommand _UpdateCommand;
        public ICommand UpdateCommand
        {
            get
            {
                if (_UpdateCommand == null)
                {
                    _UpdateCommand = new RelayCommand(async p => await ExecuteUpdateCommand(p), p => IsRowSelected);
                }
                return _UpdateCommand;
            }
        }

        RelayCommand _DeleteCommand;
        public ICommand DeleteCommand
        {
            get
            {
                if (_DeleteCommand == null)
                {
                    _DeleteCommand = new RelayCommand(async p => await ExecuteDeleteCommand(p), p => IsRowSelected);
                }
                return _DeleteCommand;
            }
        }

        /// <summary>
        /// Opens the "Manage Variants" modal scoped to SelectedMedicine (Section 13).
        /// </summary>
        RelayCommand _ManageVariantsCommand;
        public ICommand ManageVariantsCommand
        {
            get
            {
                if (_ManageVariantsCommand == null)
                {
                    _ManageVariantsCommand = new RelayCommand(p => this.ExecuteManageVariantsCommand(p), p => IsRowSelected);
                }
                return _ManageVariantsCommand;
            }
        }

        #endregion

        #region Command Methods

        private void ExecuteAddCommand(object parameter)
        {
            try
            {
                var viewModel = new MedicineViewModel
                {
                    Medicine = new Medicine
                    {
                        Status = MedicineStatusEnum.Active
                    }
                };

                var view = new MedicineView
                {
                    DataContext = viewModel
                };
                view.ShowDialog();
            }
            catch (Exception ex)
            {
                GlobalModels.FoundExceptionUI(ex);
            }
        }

        private async System.Threading.Tasks.Task ExecuteUpdateCommand(object parameter)
        {
            try
            {
                var viewModel = new MedicineViewModel
                {
                    Medicine = await MedicineDAL.GetMedicineById(SelectedMedicine.Id)
                };

                var view = new MedicineView
                {
                    DataContext = viewModel
                };
                view.ShowDialog();
            }
            catch (Exception ex)
            {
                GlobalModels.FoundExceptionUI(ex);
            }
        }

        private async System.Threading.Tasks.Task ExecuteDeleteCommand(object parameter)
        {
            try
            {
                var result = CustomMessageBoxView.Show(
                    "Delete Medicine",
                    $"Are you sure you want to delete '{SelectedMedicine.Name}'? This does not delete its variants — remove those first if needed.",
                    MessageBoxIconType.Warning,
                    MessageBoxButtonsType.YesNo);

                if (result == MessageBoxResultType.Yes)
                {
                    bool deleted = await MedicineDAL.Delete(SelectedMedicine);
                    if (deleted)
                    {
                        Messenger.Default.Send(new EntityMessage<Medicine>(SelectedMedicine, EntityMessageType.Deleted));
                    }
                }
            }
            catch (Exception ex)
            {
                GlobalModels.FoundExceptionUI(ex);
            }
        }

        private void ExecuteManageVariantsCommand(object parameter)
        {
            try
            {
                var viewModel = new ProductVariantListViewModel(SelectedMedicine.Id, SelectedMedicine.Name);

                var view = new ProductVariantListView
                {
                    DataContext = viewModel
                };
                view.ShowDialog();
            }
            catch (Exception ex)
            {
                GlobalModels.FoundExceptionUI(ex);
            }
        }

        #endregion

        #region Messenger Handler

        void OnMessage(EntityMessage<Medicine> message)
        {
            switch (message.MessageType)
            {
                case EntityMessageType.Added:
                    Medicines.Add(message.Entity);
                    break;
                case EntityMessageType.Changed:
                    {
                        var found = Medicines.FirstOrDefault(x => x.Id == message.Entity.Id);
                        if (found != null)
                        {
                            var index = Medicines.IndexOf(found);
                            Medicines.Remove(found);
                            Medicines.Insert(index, message.Entity);
                        }
                    }
                    break;
                case EntityMessageType.Deleted:
                    {
                        var found = Medicines.FirstOrDefault(x => x.Id == message.Entity.Id);
                        if (found != null)
                        {
                            Medicines.Remove(found);
                        }
                    }
                    break;
            }
        }

        #endregion
    }
}