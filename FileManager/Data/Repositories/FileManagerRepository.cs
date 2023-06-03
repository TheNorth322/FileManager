using System;
using System.IO;
using System.Security.AccessControl;
using FileManager.Domain.Repositories;
using FileManager.Model.Interfaces;
using FileManager.Domain.Entities;
namespace FileManager.Data.Repositories;

public class FileManagerRepository : IFileManagerRepository
{
    private static IFileManagerRepository instance;  
    private IFileManager _fileManager;

    protected FileManagerRepository()
    {
        _fileManager = new Domain.Entities.FileManager();
    }
    
    public static IFileManagerRepository GetInstance()
    {
        instance ??= new FileManagerRepository();
        return instance;
    }
    
    public void RenameFile(string old_path, string new_path)
    {
        _fileManager.RenameFile(old_path, new_path); 
    }

    public string[] GetDirectoryContents(string path)
    {
        return _fileManager.GetDirectoryContents(path);
    }

    public void RemoveFile(string path)
    {
        _fileManager.RemoveFile(path);
    }

    public void RemoveDirectoryContents(string path)
    {
        _fileManager.RemoveDirectoryContents(path);
    }

    public void CopyFile(string old_path, string new_path)
    {
       _fileManager.CopyFile(old_path, new_path); 
    }

    public void CreateDirectory(string path)
    {
        _fileManager.CreateDirectory(path);
    }

    public long GetFileSize(string path)
    {
        return _fileManager.GetFileSize(path);
    }

    public void ChangeFileAccess(string filePath, string userOrGroupName, FileSystemRights fileRights,
        AccessControlType accessControlType)
    {
        _fileManager.ChangeFileAccess(filePath, userOrGroupName, fileRights, accessControlType); 
    }

    public string GetGroups() => _fileManager.GetGroups();

    public string GetUsers() => _fileManager.GetUsers();
}