using Common.MVVM;

namespace PharmacyApp.Models
{
    public class User : BindableModel
    {
        private int _Id;
        public int Id
        {
            get => _Id;
            set { if (_Id != value) { _Id = value; OnPropertyChanged("Id"); } }
        }

        private string _FullName;
        public string FullName
        {
            get => _FullName;
            set { if (_FullName != value) { _FullName = value; OnPropertyChanged("FullName"); } }
        }

        private string _Username;
        public string Username
        {
            get => _Username;
            set { if (_Username != value) { _Username = value; OnPropertyChanged("Username"); } }
        }

        private string _PasswordHash;
        public string PasswordHash
        {
            get => _PasswordHash;
            set { if (_PasswordHash != value) { _PasswordHash = value; OnPropertyChanged("PasswordHash"); } }
        }

        private string _PasswordSalt;
        public string PasswordSalt
        {
            get => _PasswordSalt;
            set { if (_PasswordSalt != value) { _PasswordSalt = value; OnPropertyChanged("PasswordSalt"); } }
        }

        private UserRoleEnum _Role;
        public UserRoleEnum Role
        {
            get => _Role;
            set { if (_Role != value) { _Role = value; OnPropertyChanged("Role"); } }
        }

        private bool _IsActive;
        public bool IsActive
        {
            get => _IsActive;
            set { if (_IsActive != value) { _IsActive = value; OnPropertyChanged("IsActive"); } }
        }

        private bool _IsDeleted;
        public bool IsDeleted
        {
            get => _IsDeleted;
            set { if (_IsDeleted != value) { _IsDeleted = value; OnPropertyChanged("IsDeleted"); } }
        }
    }
}
