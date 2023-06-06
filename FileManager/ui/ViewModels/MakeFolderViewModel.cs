using System;
using BetterMVVM;
using FileManager.Domain.UseCases;

namespace FileManager.ui.ViewModels;

public class MakeFolderViewModel : ViewModelBase
{
    private readonly FileManagerInteractor _fileManagerInteractor;
    private string _absolutePath;
    private string _directoryName;

    public MakeFolderViewModel(FileManagerInteractor fileManagerInteractor, string absolutePath)
    {
        _fileManagerInteractor =
            fileManagerInteractor ?? throw new ArgumentNullException(nameof(fileManagerInteractor));
        _absolutePath = absolutePath;
    }

    public string DirectoryName
    {
        get { return _directoryName; }
        set
        {
            _directoryName = value;
            OnPropertyChange(nameof(DirectoryName));
        }
    }

    private RelayCommand _createDirectoryCommand;
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

    public RelayCommand CreateDirectoryCommand
    {
        get
        {
            return _createDirectoryCommand ?? new RelayCommand(
                _execute => CreateDirectory(),
                _canExecute => true
            );
        }
    }

    private void CreateDirectory()
    {
        try
        {
            _fileManagerInteractor.CreateDirectory(_absolutePath + $"\\{DirectoryName}");
            OnCloseModalAction();
            ExplorerContentChanged?.Invoke();
        }
        catch (Exception ex)
        {
            ExceptionEvent?.Invoke(this, ex.Message);
        }
    }

    public EventHandler<string> ExceptionEvent { get; set; }
    public Action ExplorerContentChanged { get; set; }
}