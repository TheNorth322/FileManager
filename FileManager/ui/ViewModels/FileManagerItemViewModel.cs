using System;
using System.IO;

namespace FileManager.ui.ViewModels;

public class FileManagerItemViewModel : ViewModelBase
{
    public string FileName { get; }
    public string FullPath { get; }
    public FileManagerPosition FileManagerPosition { get; }
    
    public FileManagerItemViewModel(string fileName, string fullPath, FileManagerPosition fileManagerPosition)
    {
        if (String.IsNullOrEmpty(fileName))
            throw new ArgumentException(nameof(fileName));
        if (String.IsNullOrEmpty(fullPath))
            throw new ArgumentException(nameof(fullPath));
        
        FileName = fileName;
        FullPath = fullPath;
        FileManagerPosition = fileManagerPosition;
    }
}