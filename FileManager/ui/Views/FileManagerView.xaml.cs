using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using FileManager.ui.EventArgs;
using FileManager.ui.ViewModels;

namespace FileManager.ui.Views
{
    public partial class FileManagerView : Window
    {
        public FileManagerView()
        {
            InitializeComponent();
        }

        public FileManagerView(FileManagerViewModel vm) : this()
        {
            this.DataContext = vm;
            (this.DataContext as FileManagerViewModel).OpenModal += OnOpenModal;
            (this.DataContext as FileManagerViewModel).CloseModal += OnCloseModal;
            (this.DataContext as ViewModelBase).MessageBoxRequest += OnMessageBoxRequest;
        }

        private void OnOpenModal()
        {
            modal.IsOpen = true;
        }

        private void OnCloseModal()
        {
            modal.IsOpen = false;
        }

        private void OnMessageBoxRequest(object sender, MessageBoxEventArgs e)
        {
            e.Show();     
        }
    }
}