using System;
using BetterMVVM;
using FileManager.Domain.UseCases;

namespace FileManager.ui.ViewModels;

public class CopyViewModel : ViewModelBase
{
    private readonly FileManagerInteractor _fileManagerInteractor;

    private string _oldPath;
    private string _newPath;

    public string NewPath
    {
        get { return _newPath; }
        set
        {
            _newPath = value;
            OnPropertyChange(nameof(NewPath));
        }
    }

    public CopyViewModel(FileManagerInteractor fileManagerInteractor, string newPath, string oldPath)
    {
        _fileManagerInteractor =
            fileManagerInteractor ?? throw new ArgumentNullException(nameof(fileManagerInteractor));
        _newPath = newPath;
        _oldPath = oldPath;
    }

    private RelayCommand _closeModalCommand;

    private RelayCommand _copyFileCommand;

    public RelayCommand CopyFileCommand
    {
        get
        {
            return _copyFileCommand ?? new RelayCommand(
                _execute => CopyFile(),
                _canExecute => true
            );
        }
    }

    private void CopyFile()
    {
        try
        {
            _fileManagerInteractor.CopyFile(_oldPath, _newPath + $"\\{GetFileName(_oldPath)}");
            OnCloseModalAction();
            ExplorerContentChanged?.Invoke();
        }
        catch (Exception ex)
        {
            ExceptionEvent?.Invoke(this, ex.Message);     
        }
    }

    private string GetFileName(string path)
    {
        for (int i = path.Length - 1; i >= 0; i--)
        {
            if (path[i] == '\\')
                return path.Substring(i + 1, path.Length - i - 1);
        }

        return path;
    }

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
    public EventHandler<string> ExceptionEvent { get; set; }
}