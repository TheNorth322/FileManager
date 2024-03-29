﻿using System;
using BetterMVVM;
using FileManager.Domain.UseCases;

namespace FileManager.ui.ViewModels;

public class HelpViewModel : ViewModelBase
{
    private readonly FileManagerInteractor _fileManagerInteractor;

    public HelpViewModel(FileManagerInteractor fileManagerInteractor)
    {
        _fileManagerInteractor =
            fileManagerInteractor ?? throw new ArgumentNullException(nameof(fileManagerInteractor));
    }

    private RelayCommand _closeModalCommand;
    
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
}