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
    public class MedicineCategoryListViewModel : BindableViewModel
    {
        #region Constructor

        public MedicineCategoryListViewModel()
        {
            try
            {
                Messenger.Default.Register<EntityMessage<MedicineCategory>>(this, OnMessage);
                LoadList();
            }
            catch (Exception ex)
            {
                GlobalModels.FoundExceptionUI(ex);
            }
        }

        #endregion

        #region Properties

        private ObservableCollection<MedicineCategory> _MedicineCategories;
        public ObservableCollection<MedicineCategory> MedicineCategories
        {
            get => _MedicineCategories;
            set
            {
                if (_MedicineCategories != value)
                {
                    _MedicineCategories = value;
                    OnPropertyChanged("MedicineCategories");
                }
            }
        }

        private MedicineCategory _SelectedMedicineCategory;
        public MedicineCategory SelectedMedicineCategory
        {
            get => _SelectedMedicineCategory;
            set
            {
                if (_SelectedMedicineCategory != value)
                {
                    _SelectedMedicineCategory = value;
                    OnPropertyChanged("SelectedMedicineCategory");
                    IsRowSelected = _SelectedMedicineCategory != null;
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
                var result = await MedicineCategoryDAL.GetMedicineCategories();
                MedicineCategories = result.ToObservableCollection();
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

        #endregion

        #region Command Methods

        private void ExecuteAddCommand(object parameter)
        {
            try
            {
                var viewModel = new MedicineCategoryViewModel
                {
                    MedicineCategory = new MedicineCategory()
                };

                var view = new MedicineCategoryView
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
                var viewModel = new MedicineCategoryViewModel
                {
                    MedicineCategory = await MedicineCategoryDAL.GetMedicineCategoryById(SelectedMedicineCategory.Id)
                };

                var view = new MedicineCategoryView
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
                    "Delete Category",
                    $"Are you sure you want to delete '{SelectedMedicineCategory.Name}'?",
                    MessageBoxIconType.Warning,
                    MessageBoxButtonsType.YesNo);

                if (result == MessageBoxResultType.Yes)
                {
                    bool deleted = await MedicineCategoryDAL.Delete(SelectedMedicineCategory);
                    if (deleted)
                    {
                        Messenger.Default.Send(new EntityMessage<MedicineCategory>(SelectedMedicineCategory, EntityMessageType.Deleted));
                    }
                }
            }
            catch (Exception ex)
            {
                GlobalModels.FoundExceptionUI(ex);
            }
        }

        #endregion

        #region Messenger Handler

        void OnMessage(EntityMessage<MedicineCategory> message)
        {
            switch (message.MessageType)
            {
                case EntityMessageType.Added:
                    MedicineCategories.Add(message.Entity);
                    break;
                case EntityMessageType.Changed:
                    {
                        var found = MedicineCategories.FirstOrDefault(x => x.Id == message.Entity.Id);
                        if (found != null)
                        {
                            var index = MedicineCategories.IndexOf(found);
                            MedicineCategories.Remove(found);
                            MedicineCategories.Insert(index, message.Entity);
                        }
                    }
                    break;
                case EntityMessageType.Deleted:
                    {
                        var found = MedicineCategories.FirstOrDefault(x => x.Id == message.Entity.Id);
                        if (found != null)
                        {
                            MedicineCategories.Remove(found);
                        }
                    }
                    break;
            }
        }

        #endregion
    }
}