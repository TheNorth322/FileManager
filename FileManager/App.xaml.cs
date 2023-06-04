using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using FileManager.ui.ViewModels;
using FileManager.ui.Views;

namespace FileManager
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            FileManagerViewModel vm = new FileManagerViewModel();
            vm.ExitApplicationEvent += ExitApplication;
            MainWindow = new FileManagerView(vm);
            MainWindow.Show();
            base.OnStartup(e);
        }

        private void ExitApplication()
        {
            Application.Current.Shutdown(); 
        }
    }
}