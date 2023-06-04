using System;
using FileManager.Domain.UseCases;
using FileManager.ui.Commands;

namespace FileManager.ui.ViewModels;

public class QuitViewModel : ViewModelBase
{
    private readonly FileManagerInteractor _fileManagerInteractor;

    public QuitViewModel(FileManagerInteractor fileManagerInteractor)
    {
        _fileManagerInteractor =
            fileManagerInteractor ?? throw new ArgumentNullException(nameof(fileManagerInteractor));
    }

    private RelayCommand _closeModalCommand;
    private RelayCommand _exitCommand;

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

    public RelayCommand ExitCommand
    {
        get
        {
            return _exitCommand ?? new RelayCommand(
                _execute => Exit(),
                canExecute => true);
        }
    }

    private void Exit()
    {
        ExitApplicationEvent?.Invoke();
        OnCloseModal();
    }
    
    public Action? ExitApplicationEvent { get; set; }
}