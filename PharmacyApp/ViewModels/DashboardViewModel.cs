using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;
using Common.MVVM;
using PharmacyApp.EntityFramework;
using PharmacyApp.Models;

namespace PharmacyApp.ViewModels
{
    public class DashboardViewModel : BindableViewModel
    {
        public DashboardViewModel()
        {
            LoadData();
        }

        private int _TotalActiveProducts;
        public int TotalActiveProducts
        {
            get => _TotalActiveProducts;
            set { if (_TotalActiveProducts != value) { _TotalActiveProducts = value; OnPropertyChanged("TotalActiveProducts"); } }
        }

        private int _OutOfStockCount;
        public int OutOfStockCount
        {
            get => _OutOfStockCount;
            set { if (_OutOfStockCount != value) { _OutOfStockCount = value; OnPropertyChanged("OutOfStockCount"); } }
        }

        private int _LowStockCount;
        public int LowStockCount
        {
            get => _LowStockCount;
            set { if (_LowStockCount != value) { _LowStockCount = value; OnPropertyChanged("LowStockCount"); } }
        }

        private int _NearExpiryBatchCount;
        public int NearExpiryBatchCount
        {
            get => _NearExpiryBatchCount;
            set { if (_NearExpiryBatchCount != value) { _NearExpiryBatchCount = value; OnPropertyChanged("NearExpiryBatchCount"); } }
        }

        private int _ExpiredBatchCount;
        public int ExpiredBatchCount
        {
            get => _ExpiredBatchCount;
            set { if (_ExpiredBatchCount != value) { _ExpiredBatchCount = value; OnPropertyChanged("ExpiredBatchCount"); } }
        }

        private int _TodaySalesCount;
        public int TodaySalesCount
        {
            get => _TodaySalesCount;
            set { if (_TodaySalesCount != value) { _TodaySalesCount = value; OnPropertyChanged("TodaySalesCount"); } }
        }

        private decimal _TodayRevenue;
        public decimal TodayRevenue
        {
            get => _TodayRevenue;
            set { if (_TodayRevenue != value) { _TodayRevenue = value; OnPropertyChanged("TodayRevenue"); } }
        }

        private decimal _MonthlySalesTotal;
        public decimal MonthlySalesTotal
        {
            get => _MonthlySalesTotal;
            set { if (_MonthlySalesTotal != value) { _MonthlySalesTotal = value; OnPropertyChanged("MonthlySalesTotal"); } }
        }

        private ObservableCollection<Purchase> _RecentPurchases;
        public ObservableCollection<Purchase> RecentPurchases
        {
            get => _RecentPurchases;
            set { if (_RecentPurchases != value) { _RecentPurchases = value; OnPropertyChanged("RecentPurchases"); } }
        }

        RelayCommand _RefreshCommand;
        public ICommand RefreshCommand
        {
            get
            {
                if (_RefreshCommand == null)
                    _RefreshCommand = new RelayCommand(p => LoadData());
                return _RefreshCommand;
            }
        }

        private async void LoadData()
        {
            try
            {
                TotalActiveProducts = await DashboardDAL.GetTotalActiveProducts();
                OutOfStockCount = await DashboardDAL.GetOutOfStockCount();
                LowStockCount = await DashboardDAL.GetLowStockCount();
                NearExpiryBatchCount = await DashboardDAL.GetNearExpiryBatchCount();
                ExpiredBatchCount = await DashboardDAL.GetExpiredBatchCount();
                TodaySalesCount = await DashboardDAL.GetTodaySalesCount();
                TodayRevenue = await DashboardDAL.GetTodayRevenue();
                MonthlySalesTotal = await DashboardDAL.GetMonthlySalesTotal();
                
                var purchases = await DashboardDAL.GetRecentPurchases();
                RecentPurchases = purchases.ToObservableCollection();
            }
            catch (Exception ex)
            {
                GlobalModels.FoundExceptionUI(ex);
            }
        }
    }
}
