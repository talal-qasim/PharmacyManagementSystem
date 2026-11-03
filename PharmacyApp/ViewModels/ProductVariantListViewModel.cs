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
    /// <summary>
    /// Unlike other List ViewModels, this one is scoped to a single Medicine
    /// (Section 13 — variants belong to exactly one medicine). It is constructed
    /// fresh each time "Manage Variants" is opened from MedicineListViewModel.
    /// </summary>
    public class ProductVariantListViewModel : BindableViewModel
    {
        #region Constructor

        public ProductVariantListViewModel(int medicineId, string medicineName)
        {
            try
            {
                MedicineId = medicineId;
                MedicineName = medicineName;

                Messenger.Default.Register<EntityMessage<ProductVariant>>(this, OnMessage);
                LoadList();
            }
            catch (Exception ex)
            {
                GlobalModels.FoundExceptionUI(ex);
            }
        }

        #endregion

        #region Properties

        private int _MedicineId;
        public int MedicineId
        {
            get => _MedicineId;
            set
            {
                if (_MedicineId != value)
                {
                    _MedicineId = value;
                    OnPropertyChanged("MedicineId");
                }
            }
        }

        private string _MedicineName;
        public string MedicineName
        {
            get => _MedicineName;
            set
            {
                if (_MedicineName != value)
                {
                    _MedicineName = value;
                    OnPropertyChanged("MedicineName");
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

        private ProductVariant _SelectedProductVariant;
        public ProductVariant SelectedProductVariant
        {
            get => _SelectedProductVariant;
            set
            {
                if (_SelectedProductVariant != value)
                {
                    _SelectedProductVariant = value;
                    OnPropertyChanged("SelectedProductVariant");
                    IsRowSelected = _SelectedProductVariant != null;
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
                var result = await ProductVariantDAL.GetProductVariantsByMedicineId(MedicineId);
                ProductVariants = result.ToObservableCollection();
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
                var viewModel = new ProductVariantViewModel
                {
                    ProductVariant = new ProductVariant
                    {
                        MedicineId = MedicineId,
                        UnitsPerPackage = 1
                    }
                };

                var view = new ProductVariantView
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
                var viewModel = new ProductVariantViewModel
                {
                    ProductVariant = await ProductVariantDAL.GetProductVariantById(SelectedProductVariant.Id)
                };

                var view = new ProductVariantView
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
                    "Delete Variant",
                    $"Are you sure you want to delete this variant ({SelectedProductVariant.Strength}, {SelectedProductVariant.PackageSize})?",
                    MessageBoxIconType.Warning,
                    MessageBoxButtonsType.YesNo);

                if (result == MessageBoxResultType.Yes)
                {
                    bool deleted = await ProductVariantDAL.Delete(SelectedProductVariant);
                    if (deleted)
                    {
                        Messenger.Default.Send(new EntityMessage<ProductVariant>(SelectedProductVariant, EntityMessageType.Deleted));
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

        void OnMessage(EntityMessage<ProductVariant> message)
        {
            // Only react to changes belonging to THIS medicine, since
            // ProductVariant messages are global but this list is scoped.
            if (message.Entity.MedicineId != MedicineId)
            {
                return;
            }

            switch (message.MessageType)
            {
                case EntityMessageType.Added:
                    ProductVariants.Add(message.Entity);
                    break;
                case EntityMessageType.Changed:
                    {
                        var found = ProductVariants.FirstOrDefault(x => x.Id == message.Entity.Id);
                        if (found != null)
                        {
                            var index = ProductVariants.IndexOf(found);
                            ProductVariants.Remove(found);
                            ProductVariants.Insert(index, message.Entity);
                        }
                    }
                    break;
                case EntityMessageType.Deleted:
                    {
                        var found = ProductVariants.FirstOrDefault(x => x.Id == message.Entity.Id);
                        if (found != null)
                        {
                            ProductVariants.Remove(found);
                        }
                    }
                    break;
            }
        }

        #endregion
    }
}