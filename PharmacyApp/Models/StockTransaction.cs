using System;
using Common.MVVM;

namespace PharmacyApp.Models
{
    /// <summary>
    /// Immutable audit-log row for a single quantity change on a single batch
    /// (Section 28). Rows are only ever inserted — never updated or (soft-)deleted,
    /// which is why this entity deliberately has no IsDeleted flag, unlike the
    /// rulebook's default master-data pattern. This is intentional: StockTransaction
    /// is transactional/audit data, not master data with a lifecycle to soft-delete.
    /// </summary>
    public class StockTransaction : BindableModel
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

        private int _BatchId;
        public int BatchId
        {
            get => _BatchId;
            set { if (_BatchId != value) { _BatchId = value; OnPropertyChanged("BatchId"); } }
        }

        private Batch _Batch;
        public Batch Batch
        {
            get => _Batch;
            set { if (_Batch != value) { _Batch = value; OnPropertyChanged("Batch"); } }
        }

        /// <summary>
        /// Negative for deductions (Sale, Damage, Expiry), positive for additions
        /// (Purchase, SaleCancellation restoring stock, positive Adjustment).
        /// </summary>
        private int _QuantityChange;
        public int QuantityChange
        {
            get => _QuantityChange;
            set { if (_QuantityChange != value) { _QuantityChange = value; OnPropertyChanged("QuantityChange"); } }
        }

        private StockTransactionTypeEnum _TransactionType;
        public StockTransactionTypeEnum TransactionType
        {
            get => _TransactionType;
            set { if (_TransactionType != value) { _TransactionType = value; OnPropertyChanged("TransactionType"); } }
        }

        private DateTime _TransactionDate;
        public DateTime TransactionDate
        {
            get => _TransactionDate;
            set { if (_TransactionDate != value) { _TransactionDate = value; OnPropertyChanged("TransactionDate"); } }
        }

        /// <summary>
        /// Free-text pointer to the source document (e.g. "Purchase #12", "Sale #45").
        /// Populated properly once Purchases/Sales exist in Steps 5-6.
        /// </summary>
        private string _Reference;
        public string Reference
        {
            get => _Reference;
            set { if (_Reference != value) { _Reference = value; OnPropertyChanged("Reference"); } }
        }

        private string _Reason;
        public string Reason
        {
            get => _Reason;
            set { if (_Reason != value) { _Reason = value; OnPropertyChanged("Reason"); } }
        }

        /// <summary>
        /// Section 68 — auditability. Nullable until the User module exists (Step 8).
        /// </summary>
        private int? _UserId;
        public int? UserId
        {
            get => _UserId;
            set { if (_UserId != value) { _UserId = value; OnPropertyChanged("UserId"); } }
        }

        #endregion
    }
}