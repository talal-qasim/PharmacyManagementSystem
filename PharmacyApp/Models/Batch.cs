using System;
using Common.MVVM;

namespace PharmacyApp.Models
{
    /// <summary>
    /// One received lot of a ProductVariant (Sections 15-20). Quantity here IS the
    /// batch's current stock — there's no separate BatchStock table, since splitting
    /// a 1:1 concept into two tables would just add an unnecessary join (Section 70).
    ///
    /// IMPORTANT: Quantity must never be set directly from a UI form. It only ever
    /// changes through EntityFramework.InventoryDAL (ReceiveBatch / DeductStockFefo /
    /// AdjustStock), each of which also writes a matching StockTransaction row in the
    /// same database transaction (Section 51 — atomic operations).
    /// </summary>
    public class Batch : BindableModel
    {
        #region Properties

        private int _Id;
        public int Id
        {
            get => _Id;
            set { if (_Id != value) { _Id = value; OnPropertyChanged("Id"); } }
        }

        private int _ProductVariantId;
        public int ProductVariantId
        {
            get => _ProductVariantId;
            set { if (_ProductVariantId != value) { _ProductVariantId = value; OnPropertyChanged("ProductVariantId"); } }
        }

        private ProductVariant _ProductVariant;
        public ProductVariant ProductVariant
        {
            get => _ProductVariant;
            set { if (_ProductVariant != value) { _ProductVariant = value; OnPropertyChanged("ProductVariant"); } }
        }

        private string _BatchNumber;
        public string BatchNumber
        {
            get => _BatchNumber;
            set { if (_BatchNumber != value) { _BatchNumber = value; OnPropertyChanged("BatchNumber"); } }
        }

        private DateTime _ExpiryDate;
        public DateTime ExpiryDate
        {
            get => _ExpiryDate;
            set { if (_ExpiryDate != value) { _ExpiryDate = value; OnPropertyChanged("ExpiryDate"); OnPropertyChanged("ExpiryStatus"); } }
        }

        private DateTime _ReceivedDate;
        public DateTime ReceivedDate
        {
            get => _ReceivedDate;
            set { if (_ReceivedDate != value) { _ReceivedDate = value; OnPropertyChanged("ReceivedDate"); } }
        }

        /// <summary>
        /// Cost paid per stock unit for THIS batch (Section 24) — never overwritten
        /// by a later purchase's different cost.
        /// </summary>
        private decimal _PurchaseCost;
        public decimal PurchaseCost
        {
            get => _PurchaseCost;
            set { if (_PurchaseCost != value) { _PurchaseCost = value; OnPropertyChanged("PurchaseCost"); } }
        }

        private int _Quantity;
        public int Quantity
        {
            get => _Quantity;
            set { if (_Quantity != value) { _Quantity = value; OnPropertyChanged("Quantity"); } }
        }

        private bool _IsDeleted;
        public bool IsDeleted
        {
            get => _IsDeleted;
            set { if (_IsDeleted != value) { _IsDeleted = value; OnPropertyChanged("IsDeleted"); } }
        }

        #endregion

        #region Computed

        /// <summary>
        /// Section 19 — configurable in a future Settings module; hardcoded for now.
        /// </summary>
        public const int NearExpiryThresholdDays = 90;

        /// <summary>
        /// Computed, not persisted (Sections 18-20). EF ignores get-only properties
        /// by convention; also explicitly ignored in DataContext for clarity.
        /// </summary>
        public ExpiryStatusEnum ExpiryStatus
        {
            get
            {
                if (ExpiryDate.Date < DateTime.Now.Date) return ExpiryStatusEnum.Expired;
                if ((ExpiryDate.Date - DateTime.Now.Date).TotalDays <= NearExpiryThresholdDays) return ExpiryStatusEnum.NearExpiry;
                return ExpiryStatusEnum.Normal;
            }
        }

        #endregion
    }
}