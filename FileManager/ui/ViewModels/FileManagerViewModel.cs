using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using FileManager.Data.Repositories;
using FileManager.Domain.Entities;
using FileManager.Domain.UseCases;
using FileManager.Model.Interfaces;
using FileManager.ui.Commands;

namespace FileManager.ui.ViewModels;

public class FileManagerViewModel : ViewModelBase
{
    private FileExplorerViewModel _leftExplorerViewModel;
    private FileExplorerViewModel _rightExplorerViewModel;
    
    private readonly FileManagerInteractor _fileManagerInteractor;
    
    private string _currentPath;
    private string _absolutePath = "";
    private ViewModelBase _currentModalViewModel;
    
    private RelayCommand _showHelpModalCommand;
    private RelayCommand _showCopyModalCommand;
    private RelayCommand _showMoveModalCommand;
    private RelayCommand _showMakeFolderModalCommand;
    private RelayCommand _showDeleteModalCommand;
    private RelayCommand _showQuitModalCommand;

    public FileManagerViewModel()
    {
        _fileManagerInteractor = new FileManagerInteractor(FileManagerRepository.GetInstance());
        _leftExplorerViewModel = new FileExplorerViewModel(_fileManagerInteractor);
        _rightExplorerViewModel = new FileExplorerViewModel(_fileManagerInteractor);
    }

    public FileExplorerViewModel LeftExplorerViewModel
    {
        get { return _leftExplorerViewModel; }
        set
        {
            _leftExplorerViewModel = value;
            OnPropertyChange(nameof(LeftExplorerViewModel));
        }
    }

    public FileExplorerViewModel RightExplorerViewModel
    {
        get { return _rightExplorerViewModel; }
        set
        {
            _rightExplorerViewModel = value;
            OnPropertyChange(nameof(RightExplorerViewModel));
        }
    }
    public ViewModelBase CurrentModalViewModel
    {
        get { return _currentModalViewModel; }
        set
        {
            _currentModalViewModel = value;
            CurrentModalViewModel.OnCloseModalAction += OnCloseModalFunc;

            if (_currentModalViewModel.GetType() == typeof(QuitViewModel))
                (_currentModalViewModel as QuitViewModel).ExitApplicationEvent += ExitApplication;
            
            if (_currentModalViewModel.GetType() == typeof(DeleteViewModel))
                (_currentModalViewModel as DeleteViewModel).ExplorerContentChanged += OnFileManagerContentChanged;  

            if (_currentModalViewModel.GetType() == typeof(MakeFolderViewModel))
                (_currentModalViewModel as MakeFolderViewModel).ExplorerContentChanged += OnFileManagerContentChanged;

            if (_currentModalViewModel.GetType() == typeof(MoveViewModel))
                (_currentModalViewModel as MoveViewModel).ExplorerContentChanged += OnFileManagerContentChanged;
            
            OnPropertyChange(nameof(CurrentModalViewModel));
        }
    }


    public string CurrentPath
    {
        get => _currentPath;
        set
        {
            _currentPath = value;
            OnPropertyChange(nameof(CurrentPath));
        }
    }
    
    public RelayCommand ShowHelpModalCommand
    {
        get
        {
            return _showHelpModalCommand ?? new RelayCommand(
                _execute => ShowHelpModal(),
                _canExecute => true
            );
        }
    }

    public RelayCommand ShowCopyModalCommand
    {
        get
        {
            return _showCopyModalCommand ?? new RelayCommand(
                _execute => ShowCopyModal(),
                _canExecute => (!String.IsNullOrEmpty(_absolutePath))
            );
        }
    }

    public RelayCommand ShowMoveModalCommand
    {
        get
        {
            return _showMoveModalCommand ?? new RelayCommand(
                _execute => ShowMoveModal(),
                _canExecute => (!String.IsNullOrEmpty(_absolutePath))
            );
        }
    }

    public RelayCommand ShowMakeFolderModalCommand
    {
        get
        {
            return _showMakeFolderModalCommand ?? new RelayCommand(
                _execute => ShowMakeFolderModal(),
                _canExecute => (!String.IsNullOrEmpty(_absolutePath))
            );
        }
    }

    public RelayCommand ShowDeleteModalCommand
    {
        get
        {
            return _showDeleteModalCommand ?? new RelayCommand(
                _execute => ShowDeleteModal(),
                _canExecute => (!String.IsNullOrEmpty(_absolutePath))
            );
        }
    }

    public RelayCommand ShowQuitModalCommand
    {
        get
        {
            return _showQuitModalCommand ?? new RelayCommand(
                _execute => ShowQuitModal(),
                _canExecute => true
            );
        }
    }

    private void ShowHelpModal()
    {
        CurrentModalViewModel = new HelpViewModel(_fileManagerInteractor);
        OpenModal?.Invoke();
    }

    private void ShowCopyModal()
    {
        CurrentModalViewModel = new CopyViewModel(_fileManagerInteractor, _absolutePath, CurrentPath);
        OpenModal?.Invoke();
    }

    private void ShowMoveModal()
    {
        CurrentModalViewModel = new MoveViewModel(_fileManagerInteractor);
        OpenModal?.Invoke();
    }

    private void ShowDeleteModal()
    {
        CurrentModalViewModel = new DeleteViewModel(_fileManagerInteractor, CurrentPath);
        OpenModal?.Invoke();
    }

    private void ShowQuitModal()
    {
        CurrentModalViewModel = new QuitViewModel(_fileManagerInteractor);
        OpenModal?.Invoke();
    }

    private void ShowMakeFolderModal()
    {
        CurrentModalViewModel = new MakeFolderViewModel(_fileManagerInteractor, _absolutePath);
        OpenModal?.Invoke();
    }

    private string DeleteLastName(string path)
    {
        for (int i = path.Length - 1; i >= 0; i--)
        {
            if (path[i] == '\\')
                return path.Substring(0, (i == 2) ? ++i : i);
        }

        return path;
    }

    private void OnCloseModalFunc()
    {
        CloseModal?.Invoke();
    }

    private void ExitApplication()
    {
        ExitApplicationEvent?.Invoke();
    }
    
    // TODO
    private void OnFileManagerContentChanged() {
        _leftExplorerViewModel.UpdateFileManagerContents(_leftExplorerViewModel._absolutePath);
        _rightExplorerViewModel.UpdateFileManagerContents(_rightExplorerViewModel._absolutePath);
    }

    public void OnCurrentPathChanged(object sender, System.EventArgs e)
    {
        CurrentPath = (sender as FileExplorerViewModel).CurrentPath;
    }
    public Action OpenModal { get; set; }
    public Action CloseModal { get; set; }
    public Action ExitApplicationEvent { get; set; }
}