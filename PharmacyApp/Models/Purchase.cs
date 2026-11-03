using System;
using Common.MVVM;

namespace PharmacyApp.Models
{
    /// <summary>
    /// Purchase header (Section 21). Supplier-based. TotalAmount is NOT stored —
    /// it's computed from PurchaseItems whenever displayed, so it can never drift
    /// out of sync with the actual line items.
    /// </summary>
    public class Purchase : BindableModel
    {
        #region Properties

        private int _Id;
        public int Id
        {
            get => _Id;
            set { if (_Id != value) { _Id = value; OnPropertyChanged("Id"); } }
        }

        private int _SupplierId;
        public int SupplierId
        {
            get => _SupplierId;
            set { if (_SupplierId != value) { _SupplierId = value; OnPropertyChanged("SupplierId"); } }
        }

        private Supplier _Supplier;
        public Supplier Supplier
        {
            get => _Supplier;
            set { if (_Supplier != value) { _Supplier = value; OnPropertyChanged("Supplier"); } }
        }

        private DateTime _PurchaseDate;
        public DateTime PurchaseDate
        {
            get => _PurchaseDate;
            set { if (_PurchaseDate != value) { _PurchaseDate = value; OnPropertyChanged("PurchaseDate"); } }
        }

        /// <summary>
        /// Optional PO number / supplier invoice reference.
        /// </summary>
        private string _ReferenceNumber;
        public string ReferenceNumber
        {
            get => _ReferenceNumber;
            set { if (_ReferenceNumber != value) { _ReferenceNumber = value; OnPropertyChanged("ReferenceNumber"); } }
        }

        private PurchaseStatusEnum _Status;
        public PurchaseStatusEnum Status
        {
            get => _Status;
            set { if (_Status != value) { _Status = value; OnPropertyChanged("Status"); OnPropertyChanged("IsDraft"); } }
        }

        private DateTime? _ReceivedDate;
        public DateTime? ReceivedDate
        {
            get => _ReceivedDate;
            set { if (_ReceivedDate != value) { _ReceivedDate = value; OnPropertyChanged("ReceivedDate"); } }
        }

        private bool _IsDeleted;
        public bool IsDeleted
        {
            get => _IsDeleted;
            set { if (_IsDeleted != value) { _IsDeleted = value; OnPropertyChanged("IsDeleted"); } }
        }

        #endregion

        #region Computed

        public bool IsDraft => Status == PurchaseStatusEnum.Draft;

        #endregion
    }
}