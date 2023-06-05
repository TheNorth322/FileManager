﻿using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using FileManager.Domain.UseCases;
using FileManager.ui.Commands;

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
            CurrentPathChanged?.Invoke(this, System.EventArgs.Empty);
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

    private void ParseClick()
    {
        if (SelectedFileViewModel.FileName == _goBack)
        {
            if (_absolutePath.Length == 3)
            {
                GetDisks();
                return;
            }

            UpdateFileManagerContents(CurrentPath);
            return;
        }

        FileAttributes attr = File.GetAttributes(SelectedFileViewModel.FullPath);

        if ((attr & FileAttributes.Directory) == FileAttributes.Directory)
            UpdateFileManagerContents(CurrentPath);
        else
            _fileManagerInteractor.OpenFile(SelectedFileViewModel.FullPath);
    }

    public void UpdateFileManagerContents(string path)
    {
        FileManagerContents =
            Parse(_fileManagerInteractor.GetDirectoryContents(path));
        _absolutePath = CurrentPath;
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

    public EventHandler CurrentPathChanged { get; set; }
}