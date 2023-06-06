using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows;
using BetterMVVM;
using FileManager.Domain.UseCases;

namespace FileManager.ui.ViewModels;

public class FileExplorerViewModel : ViewModelBase
{
    private ObservableCollection<FileManagerItemViewModel> _fileManagerContents;
    private FileManagerItemViewModel _selectedFileViewModel;
    private FileManagerInteractor _fileManagerInteractor;

    private string _currentPath;
    public string _absolutePath;
    private const string _goBack = "..";

    public FileExplorerViewModel(FileManagerInteractor fileManagerInteractor)
    {
        _fileManagerInteractor =
            fileManagerInteractor ?? throw new ArgumentNullException(nameof(fileManagerInteractor));
        _fileManagerContents = new ObservableCollection<FileManagerItemViewModel>();
        GetDisks();
    }

    public ObservableCollection<FileManagerItemViewModel> FileManagerContents
    {
        get { return _fileManagerContents; }
        set
        {
            _fileManagerContents = value;
            OnPropertyChange(nameof(FileManagerContents));
        }
    }

    public string CurrentPath
    {
        get { return _currentPath; }
        set
        {
            _currentPath = value;
            OnPropertyChange(nameof(_currentPath));
            CurrentPathChanged?.Invoke(this, CurrentPath);
        }
    }

    public FileManagerItemViewModel SelectedFileViewModel
    {
        get { return _selectedFileViewModel; }
        set
        {
            if (value == null) return;
            _selectedFileViewModel = value;
            CurrentPath = _selectedFileViewModel?.FullPath;
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

    private void ParseClick()
    {
        if (SelectedFileViewModel.FileName == _goBack)
        {
            try
            {
                if (CurrentPath.Length == 3)
                {
                    GetDisks();
                    return;
                }

                _absolutePath = DeleteLastName(_absolutePath);
                UpdateFileManagerContents(_absolutePath);
                return;
            }
            catch (Exception ex)
            {
                MessageBoxRequest?.Invoke(this, ex.Message);
            }
        }

        try
        {
            FileAttributes attr = File.GetAttributes(SelectedFileViewModel.FullPath);

            if ((attr & FileAttributes.Directory) == FileAttributes.Directory)
            {
                UpdateFileManagerContents(CurrentPath);
                _absolutePath = CurrentPath;
            }
            else
            {
                _fileManagerInteractor.OpenFile(SelectedFileViewModel.FullPath);
            }
        }
        catch (Exception ex)
        {
            MessageBoxRequest?.Invoke(this, ex.Message);
        }
    }

    public void UpdateFileManagerContents(string path)
    {
        if (String.IsNullOrEmpty(path))
        {
            GetDisks();
        }
        else
        {
            FileManagerContents = Parse(_fileManagerInteractor.GetDirectoryContents(path));
        }
    }

    private ObservableCollection<FileManagerItemViewModel> Parse(string[] strings)
    {
        ObservableCollection<FileManagerItemViewModel>
            collection = new ObservableCollection<FileManagerItemViewModel>();

        collection.Add(new FileManagerItemViewModel(_goBack, CurrentPath));

        foreach (string str in strings)
            collection.Add(new FileManagerItemViewModel(str.Split("\\").Last(), str));

        return collection;
    }

    private void GetDisks()
    {
        string[] disks = _fileManagerInteractor.GetDisks();

        _absolutePath = "";
        _fileManagerContents.Clear();

        foreach (string disk in disks)
            _fileManagerContents.Add(new FileManagerItemViewModel(disk, disk));
    }

    private string DeleteLastName(string path)
    {
        if (String.IsNullOrEmpty(path))
            return "";

        for (int i = path.Length - 1; i >= 0; i--)
        {
            if (path[i] == '\\')
                return path.Substring(0, (i == 2) ? ++i : i);
        }

        return path;
    }

    public EventHandler<string> CurrentPathChanged { get; set; }
    public EventHandler<string> MessageBoxRequest { get; set; }
}