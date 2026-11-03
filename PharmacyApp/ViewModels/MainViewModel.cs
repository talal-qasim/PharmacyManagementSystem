using System;
using System.Windows.Input;
using Common.MVVM;
using Microsoft.EntityFrameworkCore;
using PharmacyApp.EntityFramework;
using PharmacyApp.Models;
using PharmacyApp.ViewModels;

namespace PharmacyApp.ViewModels
{
    /// <summary>
    /// Root shell ViewModel. Owns the current MainWindowTabEnum selection and,
    /// as each module is built, will instantiate that module's List ViewModel
    /// and expose it as a property bound to its Control in MainWindow.xaml
    /// (per rulebook Section 11 checklist item 9).
    /// </summary>
    public class MainViewModel : BindableViewModel
    {
        #region Constructor


        public MainViewModel()
        {
            try
            {
                ApplyDatabaseMigration();

                MedicineCategoryListViewModel = new MedicineCategoryListViewModel();
                ManufacturerListViewModel = new ManufacturerListViewModel();
                SupplierListViewModel = new SupplierListViewModel();
                MedicineListViewModel = new MedicineListViewModel();
                BatchListViewModel = new BatchListViewModel();
                StockTransactionListViewModel = new StockTransactionListViewModel();
                PurchaseListViewModel = new PurchaseListViewModel();
                SaleListViewModel = new SaleListViewModel();
                DashboardListViewModel = new DashboardViewModel();
                ReportsViewModel = new ReportsViewModel();
                UserListViewModel = new UserListViewModel();

                MainWindowTab = MainWindowTabEnum.Dashboard;
            }
            catch (Exception ex)
            {
                GlobalModels.FoundExceptionUI(ex);
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Applies any pending EF Core migrations on startup (per rulebook Rule 7.2).
        /// </summary>
        private void ApplyDatabaseMigration()
        {
            using (DataContext dataContext = new DataContext())
            {
                dataContext.Database.Migrate();
            }
        }

        #endregion

        #region Properties

        /// <summary>
        /// Private member backing variable for <see cref="MainWindowTab" />
        /// </summary>
        private MainWindowTabEnum _MainWindowTab;
        public MainWindowTabEnum MainWindowTab
        {
            get => _MainWindowTab;
            set
            {
                if (_MainWindowTab != value)
                {
                    _MainWindowTab = value;
                    OnPropertyChanged("MainWindowTab");
                }
            }
        }
        private MedicineCategoryListViewModel _MedicineCategoryListViewModel;
        public MedicineCategoryListViewModel MedicineCategoryListViewModel
        {
            get => _MedicineCategoryListViewModel;
            set
            {
                if (_MedicineCategoryListViewModel != value)
                {
                    _MedicineCategoryListViewModel = value;
                    OnPropertyChanged("MedicineCategoryListViewModel");
                }
            }
        }

        private ManufacturerListViewModel _ManufacturerListViewModel;
        public ManufacturerListViewModel ManufacturerListViewModel
        {
            get => _ManufacturerListViewModel;
            set
            {
                if (_ManufacturerListViewModel != value)
                {
                    _ManufacturerListViewModel = value;
                    OnPropertyChanged("ManufacturerListViewModel");
                }
            }
        }

        private SupplierListViewModel _SupplierListViewModel;
        public SupplierListViewModel SupplierListViewModel
        {
            get => _SupplierListViewModel;
            set
            {
                if (_SupplierListViewModel != value)
                {
                    _SupplierListViewModel = value;
                    OnPropertyChanged("SupplierListViewModel");
                }
            }
        }
        private MedicineListViewModel _MedicineListViewModel;
        public MedicineListViewModel MedicineListViewModel
        {
            get => _MedicineListViewModel;
            set
            {
                if (_MedicineListViewModel != value)
                {
                    _MedicineListViewModel = value;
                    OnPropertyChanged("MedicineListViewModel");
                }
            }
        }
        private BatchListViewModel _BatchListViewModel;
        public BatchListViewModel BatchListViewModel
        {
            get => _BatchListViewModel;
            set
            {
                if (_BatchListViewModel != value)
                {
                    _BatchListViewModel = value;
                    OnPropertyChanged("BatchListViewModel");
                }
            }
        }

        private StockTransactionListViewModel _StockTransactionListViewModel;
        public StockTransactionListViewModel StockTransactionListViewModel
        {
            get => _StockTransactionListViewModel;
            set
            {
                if (_StockTransactionListViewModel != value)
                {
                    _StockTransactionListViewModel = value;
                    OnPropertyChanged("StockTransactionListViewModel");
                }
            }
        }
        private PurchaseListViewModel _PurchaseListViewModel;
        public PurchaseListViewModel PurchaseListViewModel
        {
            get => _PurchaseListViewModel;
            set
            {
                if (_PurchaseListViewModel != value)
                {
                    _PurchaseListViewModel = value;
                    OnPropertyChanged("PurchaseListViewModel");
                }
            }
        }
        private SaleListViewModel _SaleListViewModel;
        public SaleListViewModel SaleListViewModel
        {
            get => _SaleListViewModel;
            set
            {
                if (_SaleListViewModel != value)
                {
                    _SaleListViewModel = value;
                    OnPropertyChanged("SaleListViewModel");
                }
            }
        }
        
        private DashboardViewModel _DashboardListViewModel;
        public DashboardViewModel DashboardListViewModel
        {
            get => _DashboardListViewModel;
            set
            {
                if (_DashboardListViewModel != value)
                {
                    _DashboardListViewModel = value;
                    OnPropertyChanged("DashboardListViewModel");
                }
            }
        }

        private ReportsViewModel _ReportsViewModel;
        public ReportsViewModel ReportsViewModel
        {
            get => _ReportsViewModel;
            set
            {
                if (_ReportsViewModel != value)
                {
                    _ReportsViewModel = value;
                    OnPropertyChanged("ReportsViewModel");
                }
            }
        }

        private UserListViewModel _UserListViewModel;
        public UserListViewModel UserListViewModel
        {
            get => _UserListViewModel;
            set
            {
                if (_UserListViewModel != value)
                {
                    _UserListViewModel = value;
                    OnPropertyChanged("UserListViewModel");
                }
            }
        }

        public bool CanManageUsers => GlobalEntities.CurrentUser != null && 
            (GlobalEntities.CurrentUser.Role == UserRoleEnum.Admin || GlobalEntities.CurrentUser.Role == UserRoleEnum.Manager);

        #endregion

        #region Commands

        RelayCommand _NavigateCommand;
        public ICommand NavigateCommand
        {
            get
            {
                if (_NavigateCommand == null)
                {
                    _NavigateCommand = new RelayCommand(p => this.ExecuteNavigateCommand(p));
                }
                return _NavigateCommand;
            }
        }

        #endregion

        #region Command Methods

        private void ExecuteNavigateCommand(object parameter)
        {
            try
            {
                if (parameter is MainWindowTabEnum tab)
                {
                    MainWindowTab = tab;
                }
            }
            catch (Exception ex)
            {
                GlobalModels.FoundExceptionUI(ex);
            }
        }

        #endregion
    }
}
