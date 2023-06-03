using System;
using System.Collections.ObjectModel;
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
    public FileManagerViewModel()
    {
        _fileManagerInteractor = new FileManagerInteractor(FileManagerRepository.GetInstance());
        _leftFileManagerContents = Parse(GetDisks());
        _rightFileManagerContents = Parse(GetDisks());
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

    public FileManagerItemViewModel SelectedFileViewModel
    {
        get { return _selectedFileViewModel; }
        set
        {
            _selectedFileViewModel = value;
            CurrentPath = (String.IsNullOrEmpty(_absolutePath)) 
                ? _absolutePath + value.FullPath 
                : _absolutePath + $"\\{value.FullPath}";
            
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
    private string[] GetDisks()
    {
        return _fileManagerInteractor.GetDisks();
    }

    private void ParseClick()
    {
            
    }
    private ObservableCollection<FileManagerItemViewModel> Parse(string[] strings)
    {
        ObservableCollection<FileManagerItemViewModel>
            collection = new ObservableCollection<FileManagerItemViewModel>();
        
        foreach (string str in strings)
           collection.Add(new FileManagerItemViewModel(str, (String.IsNullOrEmpty(CurrentPath)) ? str : $"{CurrentPath}\\{str}"));

        return collection;
    }
    
}