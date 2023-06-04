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
    private ObservableCollection<FileManagerItemViewModel> _leftFileManagerContents;
    private ObservableCollection<FileManagerItemViewModel> _rightFileManagerContents;
    private FileManagerItemViewModel _selectedFileViewModel;
    private readonly FileManagerInteractor _fileManagerInteractor;
    private string _currentPath;
    private string _absolutePath = "";
    private const string _goBack = "..";
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
        _leftFileManagerContents = GetDisks(FileManagerPosition.Left);
        _rightFileManagerContents = GetDisks(FileManagerPosition.Right);
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

    public ObservableCollection<FileManagerItemViewModel> LeftFileManagerContents
    {
        get { return _leftFileManagerContents; }
        set
        {
            _leftFileManagerContents = value;
            OnPropertyChange(nameof(LeftFileManagerContents));
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

    public ObservableCollection<FileManagerItemViewModel> RightFileManagerContents
    {
        get { return _rightFileManagerContents; }
        set
        {
            _rightFileManagerContents = value;
            OnPropertyChange(nameof(RightFileManagerContents));
        }
    }

    private RelayCommand _fileManagerItemClick;

    public RelayCommand FileManagerItemClick
    {
        get
        {
            return _fileClickCommand ?? new RelayCommand(
                _execute => ParseClick(),
                _canExecute => true
            );
        }
    }

    public FileManagerItemViewModel SelectedFileViewModel
    {
        get { return _selectedFileViewModel; }
        set
        {
            _selectedFileViewModel = value;
            CurrentPath = value.FullPath; 

            OnPropertyChange(nameof(SelectedFileViewModel));
        }
    }

    private RelayCommand _fileClickCommand;

    public RelayCommand FileClickCommand
    {
        get
        {
            return _fileClickCommand ?? new RelayCommand(
                _execute => ParseClick(),
                _canExecute => true
            );
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

    private ObservableCollection<FileManagerItemViewModel> GetDisks(FileManagerPosition position)
    {
        ObservableCollection<FileManagerItemViewModel>
            collection = new ObservableCollection<FileManagerItemViewModel>();
        string[] disks = _fileManagerInteractor.GetDisks();

        foreach (string disk in disks)
            collection.Add(new FileManagerItemViewModel(disk, disk, position));

        return collection;
    }

    private void ParseClick()
    {
        _absolutePath = CurrentPath;
        if (SelectedFileViewModel.FileName == _goBack)
        {
            CurrentPath = DeleteLastName(SelectedFileViewModel.FullPath);
            if (CurrentPath.Length == 3)
            {
                if (SelectedFileViewModel.FileManagerPosition == FileManagerPosition.Left)
                    LeftFileManagerContents = GetDisks(FileManagerPosition.Left);
                else
                    RightFileManagerContents = GetDisks(FileManagerPosition.Right);
                _absolutePath = "";
                return;
            }

            UpdateFileManagerContents(SelectedFileViewModel.FileManagerPosition, CurrentPath);
            return;
        }

        FileAttributes attr = File.GetAttributes(SelectedFileViewModel.FullPath);

        if ((attr & FileAttributes.Directory) == FileAttributes.Directory)
            UpdateFileManagerContents(SelectedFileViewModel.FileManagerPosition, CurrentPath);
        else
            _fileManagerInteractor.OpenFile(SelectedFileViewModel.FullPath);
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

    private void UpdateFileManagerContents(FileManagerPosition fileManagerPosition, string path)
    {
        if (fileManagerPosition == FileManagerPosition.Left)
            LeftFileManagerContents =
                Parse(_fileManagerInteractor.GetDirectoryContents(path), fileManagerPosition);
        else
            RightFileManagerContents =
                Parse(_fileManagerInteractor.GetDirectoryContents(path), fileManagerPosition);
    }

    private ObservableCollection<FileManagerItemViewModel> Parse(string[] strings,
        FileManagerPosition fileManagerPosition)
    {
        ObservableCollection<FileManagerItemViewModel>
            collection = new ObservableCollection<FileManagerItemViewModel>();

        collection.Add(new FileManagerItemViewModel(_goBack, CurrentPath, fileManagerPosition));

        foreach (string str in strings)
            collection.Add(new FileManagerItemViewModel(str.Split("\\").Last(), str, fileManagerPosition));

        return collection;
    }

    private void OnCloseModalFunc()
    {
        CloseModal?.Invoke();
    }

    private void ExitApplication()
    {
        ExitApplicationEvent?.Invoke();
    }
    
    private void OnFileManagerContentChanged() {
        UpdateFileManagerContents(SelectedFileViewModel.FileManagerPosition, _absolutePath);
    }
    
    public Action OpenModal { get; set; }
    public Action CloseModal { get; set; }
    public Action ExitApplicationEvent { get; set; }
}