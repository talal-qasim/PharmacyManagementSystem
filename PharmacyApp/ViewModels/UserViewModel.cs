using System;
using System.Windows.Input;
using Common.MVVM;
using GalaSoft.MvvmLight.Messaging;
using PharmacyApp.EntityFramework;
using PharmacyApp.Models;

namespace PharmacyApp.ViewModels
{
    public class UserViewModel : BindableViewModel
    {
        public UserViewModel()
        {
            Model = new User();
        }

        public UserViewModel(User user)
        {
            Model = user;
        }

        private User _Model;
        public User Model
        {
            get => _Model;
            set
            {
                if (_Model != value)
                {
                    _Model = value;
                    OnPropertyChanged("Model");
                }
            }
        }

        private string _NewPassword;
        public string NewPassword
        {
            get => _NewPassword;
            set { if (_NewPassword != value) { _NewPassword = value; OnPropertyChanged("NewPassword"); } }
        }

        private string _ValidationMessage;
        public string ValidationMessage
        {
            get => _ValidationMessage;
            set { if (_ValidationMessage != value) { _ValidationMessage = value; OnPropertyChanged("ValidationMessage"); } }
        }

        public Action CloseAction { get; set; }

        RelayCommand _SaveCommand;
        public ICommand SaveCommand
        {
            get
            {
                if (_SaveCommand == null)
                    _SaveCommand = new RelayCommand(async p => await ExecuteSaveCommand());
                return _SaveCommand;
            }
        }

        private async System.Threading.Tasks.Task ExecuteSaveCommand()
        {
            if (string.IsNullOrWhiteSpace(Model.FullName) || string.IsNullOrWhiteSpace(Model.Username))
            {
                ValidationMessage = "Full Name and Username are required.";
                return;
            }

            if (Model.Id == 0 && string.IsNullOrWhiteSpace(NewPassword))
            {
                ValidationMessage = "Password is required for new users.";
                return;
            }

            if (Model.Id == 0)
            {
                var inserted = await UserDAL.Insert(Model, NewPassword);
                if (inserted != null)
                {
                    Messenger.Default.Send(new EntityMessage<User>(inserted, EntityMessageType.Added));
                    CloseAction?.Invoke();
                }
                else
                {
                    ValidationMessage = "Failed to create user. Username may already exist.";
                }
            }
            else
            {
                var updated = await UserDAL.Update(Model, NewPassword);
                if (updated != null)
                {
                    Messenger.Default.Send(new EntityMessage<User>(updated, EntityMessageType.Changed));
                    CloseAction?.Invoke();
                }
                else
                {
                    ValidationMessage = "Failed to update user.";
                }
            }
        }
    }
}
