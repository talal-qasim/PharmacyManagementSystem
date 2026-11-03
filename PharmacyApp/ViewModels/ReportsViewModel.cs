using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using Common.MVVM;
using PharmacyApp.EntityFramework;
using PharmacyApp.Models;

namespace PharmacyApp.ViewModels
{
    public class ReportsViewModel : BindableViewModel
    {
        public ReportsViewModel()
        {
            FromDate = DateTime.Now.AddDays(-30);
            ToDate = DateTime.Now;
            ReportType = ReportTypeEnum.SalesReport;
            LoadVariants();
        }

        private async void LoadVariants()
        {
            try
            {
                var variants = await ProductVariantDAL.GetAllProductVariants();
                AvailableVariants = variants.ToObservableCollection();
                if (AvailableVariants.Any())
                {
                    SelectedVariant = AvailableVariants.First();
                }
            }
            catch (Exception ex)
            {
                GlobalModels.FoundExceptionUI(ex);
            }
        }

        private ObservableCollection<ProductVariant> _AvailableVariants;
        public ObservableCollection<ProductVariant> AvailableVariants
        {
            get => _AvailableVariants;
            set { if (_AvailableVariants != value) { _AvailableVariants = value; OnPropertyChanged("AvailableVariants"); } }
        }

        private ProductVariant _SelectedVariant;
        public ProductVariant SelectedVariant
        {
            get => _SelectedVariant;
            set { if (_SelectedVariant != value) { _SelectedVariant = value; OnPropertyChanged("SelectedVariant"); } }
        }

        private ReportTypeEnum _ReportType;
        public ReportTypeEnum ReportType
        {
            get => _ReportType;
            set 
            { 
                if (_ReportType != value) 
                { 
                    _ReportType = value; 
                    OnPropertyChanged("ReportType"); 
                    OnPropertyChanged("ShowDateRange");
                    OnPropertyChanged("ShowVariantSelector");
                } 
            }
        }

        public bool ShowDateRange => ReportType == ReportTypeEnum.SalesReport || 
                                     ReportType == ReportTypeEnum.DailySales || 
                                     ReportType == ReportTypeEnum.PurchaseReport || 
                                     ReportType == ReportTypeEnum.StockMovement;

        public bool ShowVariantSelector => ReportType == ReportTypeEnum.StockMovement;

        private DateTime _FromDate;
        public DateTime FromDate
        {
            get => _FromDate;
            set { if (_FromDate != value) { _FromDate = value; OnPropertyChanged("FromDate"); } }
        }

        private DateTime _ToDate;
        public DateTime ToDate
        {
            get => _ToDate;
            set { if (_ToDate != value) { _ToDate = value; OnPropertyChanged("ToDate"); } }
        }

        private object _ReportResults;
        public object ReportResults
        {
            get => _ReportResults;
            set { if (_ReportResults != value) { _ReportResults = value; OnPropertyChanged("ReportResults"); } }
        }

        RelayCommand _RunReportCommand;
        public ICommand RunReportCommand
        {
            get
            {
                if (_RunReportCommand == null)
                    _RunReportCommand = new RelayCommand(async p => await ExecuteRunReportCommand());
                return _RunReportCommand;
            }
        }

        private async System.Threading.Tasks.Task ExecuteRunReportCommand()
        {
            try
            {
                switch (ReportType)
                {
                    case ReportTypeEnum.SalesReport:
                        ReportResults = await ReportDAL.GetSalesReport(FromDate, ToDate);
                        break;
                    case ReportTypeEnum.DailySales:
                        ReportResults = await ReportDAL.GetDailySalesReport(FromDate);
                        break;
                    case ReportTypeEnum.PurchaseReport:
                        ReportResults = await ReportDAL.GetPurchaseReport(FromDate, ToDate);
                        break;
                    case ReportTypeEnum.LowStock:
                        ReportResults = await ReportDAL.GetLowStockReport();
                        break;
                    case ReportTypeEnum.ExpiryReport:
                        ReportResults = await ReportDAL.GetExpiryReport();
                        break;
                    case ReportTypeEnum.StockMovement:
                        if (SelectedVariant != null)
                        {
                            ReportResults = await ReportDAL.GetStockMovementReport(SelectedVariant.Id, FromDate, ToDate);
                        }
                        break;
                }
            }
            catch (Exception ex)
            {
                GlobalModels.FoundExceptionUI(ex);
            }
        }
    }
}
