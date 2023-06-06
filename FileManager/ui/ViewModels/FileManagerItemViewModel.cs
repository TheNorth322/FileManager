using System;
using System.IO;
using BetterMVVM;

namespace FileManager.ui.ViewModels;

public class FileManagerItemViewModel : ViewModelBase
{
    public string FileName { get; }
    public string FullPath { get; }
    
    public FileManagerItemViewModel(string fileName, string fullPath)
    {
        if (String.IsNullOrEmpty(fileName))
            throw new ArgumentException(nameof(fileName));
        if (String.IsNullOrEmpty(fullPath))
            throw new ArgumentException(nameof(fullPath));
        
        FileName = fileName;
        FullPath = fullPath;
    }
}