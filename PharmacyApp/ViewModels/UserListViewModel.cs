using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using Common.MVVM;
using GalaSoft.MvvmLight.Messaging;
using MyMessageBox;
using PharmacyApp.EntityFramework;
using PharmacyApp.Models;
using PharmacyApp.Views;

namespace PharmacyApp.ViewModels
{
    public class UserListViewModel : BindableViewModel
    {
        public UserListViewModel()
        {
            try
            {
                Messenger.Default.Register<EntityMessage<User>>(this, OnMessage);
                LoadList();
            }
            catch (Exception ex)
            {
                GlobalModels.FoundExceptionUI(ex);
            }
        }

        private ObservableCollection<User> _Users;
        public ObservableCollection<User> Users
        {
            get => _Users;
            set
            {
                if (_Users != value)
                {
                    _Users = value;
                    OnPropertyChanged("Users");
                }
            }
        }

        private User _SelectedUser;
        public User SelectedUser
        {
            get => _SelectedUser;
            set
            {
                if (_SelectedUser != value)
                {
                    _SelectedUser = value;
                    OnPropertyChanged("SelectedUser");
                    IsRowSelected = _SelectedUser != null;
                }
            }
        }

        private bool _IsRowSelected;
        public bool IsRowSelected
        {
            get => _IsRowSelected;
            set
            {
                if (_IsRowSelected != value)
                {
                    _IsRowSelected = value;
                    OnPropertyChanged("IsRowSelected");
                }
            }
        }

        private async void LoadList()
        {
            try
            {
                var result = await UserDAL.GetUsers();
                Users = result.ToObservableCollection();
            }
            catch (Exception ex)
            {
                GlobalModels.FoundExceptionUI(ex);
            }
        }

        RelayCommand _AddCommand;
        public ICommand AddCommand
        {
            get
            {
                if (_AddCommand == null)
                    _AddCommand = new RelayCommand(p => ExecuteAddCommand());
                return _AddCommand;
            }
        }

        RelayCommand _EditCommand;
        public ICommand EditCommand
        {
            get
            {
                if (_EditCommand == null)
                    _EditCommand = new RelayCommand(p => ExecuteEditCommand(), p => IsRowSelected);
                return _EditCommand;
            }
        }

        RelayCommand _DeleteCommand;
        public ICommand DeleteCommand
        {
            get
            {
                if (_DeleteCommand == null)
                    _DeleteCommand = new RelayCommand(async p => await ExecuteDeleteCommand(), p => IsRowSelected);
                return _DeleteCommand;
            }
        }

        private void ExecuteAddCommand()
        {
            var vm = new UserViewModel();
            var view = new UserView { DataContext = vm };
            vm.CloseAction = () => view.Close();
            view.ShowDialog();
        }

        private void ExecuteEditCommand()
        {
            // Create a clone to edit
            var clone = new User
            {
                Id = SelectedUser.Id,
                FullName = SelectedUser.FullName,
                Username = SelectedUser.Username,
                Role = SelectedUser.Role,
                IsActive = SelectedUser.IsActive
            };
            var vm = new UserViewModel(clone);
            var view = new UserView { DataContext = vm };
            vm.CloseAction = () => view.Close();
            view.ShowDialog();
        }

        private async System.Threading.Tasks.Task ExecuteDeleteCommand()
        {
            if (SelectedUser.Id == GlobalEntities.CurrentUser.Id)
            {
                CustomMessageBoxView.Show("Cannot Delete", "You cannot delete your own account.", MessageBoxIconType.Warning, MessageBoxButtonsType.Ok);
                return;
            }

            var result = CustomMessageBoxView.Show("Confirm Delete", $"Are you sure you want to delete {SelectedUser.FullName}?", MessageBoxIconType.Warning, MessageBoxButtonsType.YesNo);
            if (result == MessageBoxResultType.Yes)
            {
                bool deleted = await UserDAL.Delete(SelectedUser);
                if (deleted)
                {
                    Messenger.Default.Send(new EntityMessage<User>(SelectedUser, EntityMessageType.Deleted));
                }
            }
        }

        void OnMessage(EntityMessage<User> message)
        {
            switch (message.MessageType)
            {
                case EntityMessageType.Added:
                    Users.Add(message.Entity);
                    break;
                case EntityMessageType.Changed:
                    var found = Users.FirstOrDefault(x => x.Id == message.Entity.Id);
                    if (found != null)
                    {
                        var idx = Users.IndexOf(found);
                        Users.Remove(found);
                        Users.Insert(idx, message.Entity);
                    }
                    break;
                case EntityMessageType.Deleted:
                    var delFound = Users.FirstOrDefault(x => x.Id == message.Entity.Id);
                    if (delFound != null)
                        Users.Remove(delFound);
                    break;
            }
        }
    }
}
