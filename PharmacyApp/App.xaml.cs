using System;
using System.Windows;
using PharmacyApp.Models;
using PharmacyApp.ViewModels;

namespace PharmacyApp
{
    /// <summary>
    /// Application bootstrap. Creates the single root MainViewModel, stores it in
    /// GlobalModels (Rule 10.1), and lets StartupUri (App.xaml) show MainWindow,
    /// whose DataContext is set to that same MainViewModel instance below.
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            try
            {
                // SEED ADMIN - Wrapped in Task.Run to prevent UI thread deadlock from .Result / .Wait()
                System.Threading.Tasks.Task.Run(async () => {
                    var users = await PharmacyApp.EntityFramework.UserDAL.GetUsers();
                    if (users.Count == 0) {
                        var admin = new PharmacyApp.Models.User { FullName = "System Administrator", Username = "admin", Role = PharmacyApp.Models.UserRoleEnum.Admin, IsActive = true };
                        await PharmacyApp.EntityFramework.UserDAL.Insert(admin, "admin123");
                    }
                }).Wait();

                this.ShutdownMode = ShutdownMode.OnExplicitShutdown;

                var loginViewModel = new LoginViewModel();
                var loginView = new Views.LoginView
                {
                    DataContext = loginViewModel
                };
                
                loginViewModel.CloseAction = () => loginView.Close();

                loginView.ShowDialog();

                if (!loginViewModel.LoginSucceeded || GlobalEntities.CurrentUser == null)
                {
                    Shutdown();
                    return;
                }

                var mainViewModel = new MainViewModel();
                GlobalModels.MainViewModel = mainViewModel;

                var mainWindow = new MainWindow
                {
                    DataContext = mainViewModel
                };
                this.MainWindow = mainWindow;
                this.ShutdownMode = ShutdownMode.OnMainWindowClose;
                mainWindow.Show();
            }
            catch (Exception ex)
            {
                GlobalModels.FoundExceptionUI(ex);
                Shutdown();
            }
        }
    }
}
