using Common.MVVM;

namespace PharmacyApp.Models
{
    /// <summary>
    /// The actual sellable unit of a Medicine (Sections 6-14, 38, 39, 46).
    /// Example rows for "Hydraline": one ProductVariant for the 250ml bottle,
    /// a separate ProductVariant for the 500ml bottle — each with its own stock,
    /// batches, price, and reorder thresholds.
    /// </summary>
    public class ProductVariant : BindableModel
    {
        #region Properties

        private int _Id;

        /// <summary>
        /// Combined display label used in combo boxes elsewhere (e.g. BatchView's
        /// variant picker), so the user sees "Panadol — 250mg — 14 tablets" instead
        /// of a bare Id. Computed, not stored — EF ignores get-only properties by
        /// convention.
        /// </summary>
        public string DisplayName =>
            (Medicine != null ? Medicine.Name : "Unknown") + " — " + Strength + " — " + PackageSize;
        public int Id
        {
            get => _Id;
            set
            {
                if (_Id != value)
                {
                    _Id = value;
                    OnPropertyChanged("Id");
                }
            }
        }

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

        private Medicine _Medicine;
        public Medicine Medicine
        {
            get => _Medicine;
            set
            {
                if (_Medicine != value)
                {
                    _Medicine = value;
                    OnPropertyChanged("Medicine");
                }
            }
        }

        /// <summary>
        /// e.g. "250mg", "250mg/5ml". Free text — concentration formats vary too much
        /// for a strict numeric type (Section 12).
        /// </summary>
        private string _Strength;
        public string Strength
        {
            get => _Strength;
            set
            {
                if (_Strength != value)
                {
                    _Strength = value;
                    OnPropertyChanged("Strength");
                }
            }
        }

        private DosageFormEnum _DosageForm;
        public DosageFormEnum DosageForm
        {
            get => _DosageForm;
            set
            {
                if (_DosageForm != value)
                {
                    _DosageForm = value;
                    OnPropertyChanged("DosageForm");
                }
            }
        }

        /// <summary>
        /// The commercial packaging description, e.g. "14 tablets", "250ml", "20g".
        /// Characteristic of the product — NOT the stock unit (Section 12).
        /// </summary>
        private string _PackageSize;
        public string PackageSize
        {
            get => _PackageSize;
            set
            {
                if (_PackageSize != value)
                {
                    _PackageSize = value;
                    OnPropertyChanged("PackageSize");
                }
            }
        }

        /// <summary>
        /// The actual sellable/stock-deducted unit (Section 6/61) — Tablet, Capsule,
        /// Tube, Bottle, Vial, Ampoule.
        /// </summary>
        private StockUnitEnum _StockUnit;
        public StockUnitEnum StockUnit
        {
            get => _StockUnit;
            set
            {
                if (_StockUnit != value)
                {
                    _StockUnit = value;
                    OnPropertyChanged("StockUnit");
                }
            }
        }

        /// <summary>
        /// For tablets/capsules only: how many StockUnits are in one commercial pack
        /// (Section 9), e.g. 14. Used to convert "sell 1 pack" into "-14 tablets".
        /// Leave 1 (or null-equivalent) for tubes/bottles/vials where 1 package = 1 stock unit.
        /// </summary>
        private int _UnitsPerPackage;
        public int UnitsPerPackage
        {
            get => _UnitsPerPackage;
            set
            {
                if (_UnitsPerPackage != value)
                {
                    _UnitsPerPackage = value;
                    OnPropertyChanged("UnitsPerPackage");
                }
            }
        }

        /// <summary>
        /// Optional barcode for scanning (Section 39). Nullable — search must still
        /// work without a scanner.
        /// </summary>
        private string _Barcode;
        public string Barcode
        {
            get => _Barcode;
            set
            {
                if (_Barcode != value)
                {
                    _Barcode = value;
                    OnPropertyChanged("Barcode");
                }
            }
        }

        /// <summary>
        /// Current selling price per StockUnit. Historical SaleItems freeze their own
        /// price at time of sale (Section 33) — this field only drives NEW sales.
        /// </summary>
        private decimal _SellingPrice;
        public decimal SellingPrice
        {
            get => _SellingPrice;
            set
            {
                if (_SellingPrice != value)
                {
                    _SellingPrice = value;
                    OnPropertyChanged("SellingPrice");
                }
            }
        }

        /// <summary>
        /// Restock threshold (Section 46). Below this, status becomes Low Stock.
        /// </summary>
        private int _MinimumStockLevel;
        public int MinimumStockLevel
        {
            get => _MinimumStockLevel;
            set
            {
                if (_MinimumStockLevel != value)
                {
                    _MinimumStockLevel = value;
                    OnPropertyChanged("MinimumStockLevel");
                }
            }
        }

        /// <summary>
        /// Suggested purchase quantity when restocking (Section 46/49).
        /// </summary>
        private int _ReorderQuantity;
        public int ReorderQuantity
        {
            get => _ReorderQuantity;
            set
            {
                if (_ReorderQuantity != value)
                {
                    _ReorderQuantity = value;
                    OnPropertyChanged("ReorderQuantity");
                }
            }
        }

        private bool _IsDeleted;
        public bool IsDeleted
        {
            get => _IsDeleted;
            set
            {
                if (_IsDeleted != value)
                {
                    _IsDeleted = value;
                    OnPropertyChanged("IsDeleted");
                }
            }
        }

        #endregion
    }
}