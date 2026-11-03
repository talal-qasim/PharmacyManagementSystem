using System;
using System.Windows.Input;
using Common.MVVM;
using PharmacyApp.EntityFramework;
using PharmacyApp.Models;

namespace PharmacyApp.ViewModels
{
    public class LoginViewModel : BindableViewModel
    {
        private string _Username;
        public string Username
        {
            get => _Username;
            set { if (_Username != value) { _Username = value; OnPropertyChanged("Username"); } }
        }

        private string _Password;
        public string Password
        {
            get => _Password;
            set { if (_Password != value) { _Password = value; OnPropertyChanged("Password"); } }
        }

        private string _ValidationMessage;
        public string ValidationMessage
        {
            get => _ValidationMessage;
            set { if (_ValidationMessage != value) { _ValidationMessage = value; OnPropertyChanged("ValidationMessage"); } }
        }

        public bool LoginSucceeded { get; private set; }
        public Action CloseAction { get; set; }

        RelayCommand _LoginCommand;
        public ICommand LoginCommand
        {
            get
            {
                if (_LoginCommand == null)
                    _LoginCommand = new RelayCommand(async p => await ExecuteLoginCommand());
                return _LoginCommand;
            }
        }

        private bool _RememberPassword;
        public bool RememberPassword
        {
            get => _RememberPassword;
            set { if (_RememberPassword != value) { _RememberPassword = value; OnPropertyChanged("RememberPassword"); } }
        }

        public LoginViewModel()
        {
            LoadCredentials();
        }

        private void LoadCredentials()
        {
            try
            {
                if (System.IO.File.Exists("credentials.dat"))
                {
                    var data = System.IO.File.ReadAllText("credentials.dat").Split('|');
                    if (data.Length == 2)
                    {
                        Username = data[0];
                        Password = data[1];
                        RememberPassword = true;
                    }
                }
            }
            catch { }
        }

        private void SaveCredentials()
        {
            try
            {
                if (RememberPassword)
                {
                    System.IO.File.WriteAllText("credentials.dat", $"{Username}|{Password}");
                }
                else
                {
                    if (System.IO.File.Exists("credentials.dat"))
                        System.IO.File.Delete("credentials.dat");
                }
            }
            catch { }
        }

        private async System.Threading.Tasks.Task ExecuteLoginCommand()
        {
            if (string.IsNullOrWhiteSpace(Username) || string.IsNullOrWhiteSpace(Password))
            {
                ValidationMessage = "Please enter both username and password.";
                return;
            }

            var user = await UserDAL.Authenticate(Username, Password);
            if (user != null)
            {
                GlobalEntities.CurrentUser = user;
                LoginSucceeded = true;
                SaveCredentials();
                CloseAction?.Invoke();
            }
            else
            {
                ValidationMessage = "Invalid username or password.";
            }
        }
    }
}
