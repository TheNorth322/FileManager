using System;
using FileManager.Domain.UseCases;
using FileManager.ui.Commands;

namespace FileManager.ui.ViewModels;

public class MoveViewModel : ViewModelBase
{
    private readonly FileManagerInteractor _fileManagerInteractor;

    public MoveViewModel(FileManagerInteractor fileManagerInteractor)
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
    
    
    public Action ExplorerContentChanged { get; set; }
}