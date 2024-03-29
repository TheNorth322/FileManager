﻿using System;
using BetterMVVM;
using FileManager.Domain.UseCases;

namespace FileManager.ui.ViewModels;

public class DeleteViewModel : ViewModelBase
{
    private readonly FileManagerInteractor _fileManagerInteractor;
    public string CurrentPath { get; }
    public DeleteViewModel(FileManagerInteractor fileManagerInteractor, string currentPath)
    {
        _fileManagerInteractor =
            fileManagerInteractor ?? throw new ArgumentNullException(nameof(fileManagerInteractor));
        CurrentPath = currentPath;
    }

    private RelayCommand _closeModalCommand;
    private RelayCommand _deleteFileCommand;
    
    public RelayCommand CloseModalCommand
    {
        get
        {
            return _closeModalCommand ?? new RelayCommand(
                _execute => OnCloseModalAction(),
                _canExecute => true
            );
        }
    }

    public RelayCommand DeleteFileCommand
    {
        get { return _deleteFileCommand ?? new RelayCommand(
            _execute => DeleteFile(),
            _canExecute => true); }
    }

    private void DeleteFile()
    {
        try
        {
            _fileManagerInteractor.RemoveFile(CurrentPath);
            ExplorerContentChanged?.Invoke();
            OnCloseModalAction();
        }
        catch (Exception ex)
        {
            ExceptionEvent?.Invoke(this, ex.Message);  
        }
    }
    
    public EventHandler<string> ExceptionEvent { get; set; }
    public Action ExplorerContentChanged { get; set; }
}