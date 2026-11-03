using Common.MVVM;

namespace PharmacyApp.Models
{
    /// <summary>
    /// Represents a medicine classification/category (e.g. "Analgesics", "Antibiotics").
    /// Used to group medicines for browsing/reporting. Simple lookup entity —
    /// mirrors the standard rulebook entity shape (Rule 6.2 / Section 11 checklist item 1).
    /// </summary>
    public class MedicineCategory : BindableModel
    {
        #region Properties

        /// <summary>
        /// Private member backing variable for <see cref="Id" />
        /// </summary>
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
        /// Private member backing variable for <see cref="Name" />
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
        /// Private member backing variable for <see cref="Description" />
        /// </summary>
        private string _Description;
        public string Description
        {
            get => _Description;
            set
            {
                if (_Description != value)
                {
                    _Description = value;
                    OnPropertyChanged("Description");
                }
            }
        }

        /// <summary>
        /// Private member backing variable for <see cref="IsDeleted" />
        /// </summary>
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