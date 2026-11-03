using Common.MVVM;

namespace PharmacyApp.Models
{
    public class Manufacturer : BindableModel
    {
        private int _Id;
        public int Id { get => _Id; set { if (_Id != value) { _Id = value; OnPropertyChanged("Id"); } } }

        private string _Name;
        public string Name { get => _Name; set { if (_Name != value) { _Name = value; OnPropertyChanged("Name"); } } }

        private string _ContactPerson;
        public string ContactPerson { get => _ContactPerson; set { if (_ContactPerson != value) { _ContactPerson = value; OnPropertyChanged("ContactPerson"); } } }

        private string _PhoneNumber;
        public string PhoneNumber { get => _PhoneNumber; set { if (_PhoneNumber != value) { _PhoneNumber = value; OnPropertyChanged("PhoneNumber"); } } }

        private string _Email;
        public string Email { get => _Email; set { if (_Email != value) { _Email = value; OnPropertyChanged("Email"); } } }

        private string _Address;
        public string Address { get => _Address; set { if (_Address != value) { _Address = value; OnPropertyChanged("Address"); } } }

        private bool _IsDeleted;
        public bool IsDeleted { get => _IsDeleted; set { if (_IsDeleted != value) { _IsDeleted = value; OnPropertyChanged("IsDeleted"); } } }
    }
}
