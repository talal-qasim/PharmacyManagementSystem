using System;
using Common.MVVM;

namespace PharmacyApp.Models
{
    /// <summary>
    /// One purchased line item (Section 21-22). While the parent Purchase is Draft,
    /// this only exists as a plan; it becomes a real Batch (with matching
    /// StockTransaction) only when PurchasingDAL.ReceivePurchase runs (Section 23).
    /// </summary>
    public class PurchaseItem : BindableModel
    {
        #region Properties

        private int _Id;
        public int Id
        {
            get => _Id;
            set { if (_Id != value) { _Id = value; OnPropertyChanged("Id"); } }
        }

        private int _PurchaseId;
        public int PurchaseId
        {
            get => _PurchaseId;
            set { if (_PurchaseId != value) { _PurchaseId = value; OnPropertyChanged("PurchaseId"); } }
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

        /// <summary>
        /// Quantity in the variant's actual stock unit (Section 22 — already
        /// converted, e.g. "10 packs x 14 tablets" is entered here as 140).
        /// </summary>
        private int _Quantity;
        public int Quantity
        {
            get => _Quantity;
            set { if (_Quantity != value) { _Quantity = value; OnPropertyChanged("Quantity"); OnPropertyChanged("LineTotal"); } }
        }

        private decimal _PurchaseCost;
        public decimal PurchaseCost
        {
            get => _PurchaseCost;
            set { if (_PurchaseCost != value) { _PurchaseCost = value; OnPropertyChanged("PurchaseCost"); OnPropertyChanged("LineTotal"); } }
        }

        /// <summary>
        /// The batch/lot number this line will become on receiving (Section 21).
        /// </summary>
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
            set { if (_ExpiryDate != value) { _ExpiryDate = value; OnPropertyChanged("ExpiryDate"); } }
        }

        #endregion

        #region Computed

        public decimal LineTotal => Quantity * PurchaseCost;

        #endregion
    }
}