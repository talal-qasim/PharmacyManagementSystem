using Common.MVVM;

namespace PharmacyApp.Models
{
    /// <summary>
    /// General medicine identity (Sections 13/14/60). Does NOT hold strength,
    /// package size, or stock — those live on ProductVariant. One Medicine can
    /// have many ProductVariants (e.g. Panadol 250mg Tablet, Panadol 500mg Tablet).
    /// </summary>
    public class Medicine : BindableModel
    {
        #region Properties

        private int _Id;
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

        /// <summary>
        /// Brand/commercial name, e.g. "Panadol".
        /// </summary>
        private string _Name;
        public string Name
        {
            get => _Name;
            set
            {
                if (_Name != value)
                {
                    _Name = value;
                    OnPropertyChanged("Name");
                }
            }
        }

        /// <summary>
        /// Generic/chemical name, e.g. "Paracetamol". Optional but recommended (Section 14).
        /// </summary>
        private string _GenericName;
        public string GenericName
        {
            get => _GenericName;
            set
            {
                if (_GenericName != value)
                {
                    _GenericName = value;
                    OnPropertyChanged("GenericName");
                }
            }
        }

        private int _MedicineCategoryId;
        public int MedicineCategoryId
        {
            get => _MedicineCategoryId;
            set
            {
                if (_MedicineCategoryId != value)
                {
                    _MedicineCategoryId = value;
                    OnPropertyChanged("MedicineCategoryId");
                }
            }
        }

        private MedicineCategory _MedicineCategory;
        public MedicineCategory MedicineCategory
        {
            get => _MedicineCategory;
            set
            {
                if (_MedicineCategory != value)
                {
                    _MedicineCategory = value;
                    OnPropertyChanged("MedicineCategory");
                }
            }
        }

        private int _ManufacturerId;
        public int ManufacturerId
        {
            get => _ManufacturerId;
            set
            {
                if (_ManufacturerId != value)
                {
                    _ManufacturerId = value;
                    OnPropertyChanged("ManufacturerId");
                }
            }
        }

        private Manufacturer _Manufacturer;
        public Manufacturer Manufacturer
        {
            get => _Manufacturer;
            set
            {
                if (_Manufacturer != value)
                {
                    _Manufacturer = value;
                    OnPropertyChanged("Manufacturer");
                }
            }
        }

        /// <summary>
        /// Section 41 — MVP prescription flag. If true, the sale workflow will
        /// require a prescription reference (implemented in Step 6 — Sales).
        /// </summary>
        private bool _RequiresPrescription;
        public bool RequiresPrescription
        {
            get => _RequiresPrescription;
            set
            {
                if (_RequiresPrescription != value)
                {
                    _RequiresPrescription = value;
                    OnPropertyChanged("RequiresPrescription");
                }
            }
        }

        /// <summary>
        /// Section 42 — generic flag only, no jurisdiction-specific legal logic attached.
        /// </summary>
        private bool _IsControlledOrRestricted;
        public bool IsControlledOrRestricted
        {
            get => _IsControlledOrRestricted;
            set
            {
                if (_IsControlledOrRestricted != value)
                {
                    _IsControlledOrRestricted = value;
                    OnPropertyChanged("IsControlledOrRestricted");
                }
            }
        }

        /// <summary>
        /// Section 34 — Active/Inactive. Separate concept from stock quantity.
        /// </summary>
        private MedicineStatusEnum _Status;
        public MedicineStatusEnum Status
        {
            get => _Status;
            set
            {
                if (_Status != value)
                {
                    _Status = value;
                    OnPropertyChanged("Status");
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