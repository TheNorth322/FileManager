using System;
using FileManager.Domain.UseCases;
using FileManager.ui.Commands;

namespace FileManager.ui.ViewModels;

public class MoveViewModel : ViewModelBase
{
    private readonly FileManagerInteractor _fileManagerInteractor;
    private string _newPath;
    private string _oldPath;
    
    private RelayCommand _moveFileCommand;

    public MoveViewModel(FileManagerInteractor fileManagerInteractor, string _oldPath)
    {
        _fileManagerInteractor =
            fileManagerInteractor ?? throw new ArgumentNullException(nameof(fileManagerInteractor));
        this._oldPath = _oldPath;
        NewPath = this._oldPath;
    }

    public RelayCommand MoveFileCommand
    {
        get
        {
            return _moveFileCommand ?? new RelayCommand(
                _execute => MoveFile(),
                _canExecute => true);
        }
    }

    public string NewPath
    {
        get { return _newPath; }
        set
        {
            _newPath = value;
            OnPropertyChange(nameof(_newPath));
        }
    }


    private void MoveFile()
    {
        try
        {
            _fileManagerInteractor.RenameFile(_oldPath, NewPath);
            OnCloseModalAction();
            ExplorerContentChanged?.Invoke();            
        }
        catch (Exception ex)
        {
            ExceptionEvent?.Invoke(this, ex.Message);            
        }
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


    public EventHandler<string> ExceptionEvent { get; set; }
    public Action ExplorerContentChanged { get; set; }
}