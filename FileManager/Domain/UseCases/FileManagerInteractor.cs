using System.IO;
using System.Security.AccessControl;
using FileManager.Domain.Repositories;

namespace FileManager.Domain.UseCases;

public class FileManagerInteractor
{
    private readonly IFileManagerRepository _fileManagerRepository;

    public FileManagerInteractor(IFileManagerRepository fileManagerRepository)
    {
        _fileManagerRepository = fileManagerRepository;
    }

    public void RenameFile(string old_path, string new_path)
    {
        _fileManagerRepository.RenameFile(old_path, new_path);
    }

    public string[] GetDirectoryContents(string path)
    {
        return _fileManagerRepository.GetDirectoryContents(path);
    }

    public void RemoveFile(string path)
    {
        _fileManagerRepository.RemoveFile(path);
    }

    public void RemoveDirectoryContents(string path)
    {
        _fileManagerRepository.RemoveDirectoryContents(path);
    }

    public void CopyFile(string old_path, string new_path)
    {
        _fileManagerRepository.CopyFile(old_path, new_path);
    }

    public void CreateDirectory(string path)
    {
        _fileManagerRepository.CreateDirectory(path);
    }

    public long GetFileSize(string path)
    {
       return _fileManagerRepository.GetFileSize(path); 
    }

    public void ChangeFileAccess(string filePath, string userOrGroupName, FileSystemRights fileRights,
        AccessControlType accessControlType)
    {
        _fileManagerRepository.ChangeFileAccess(filePath, userOrGroupName, fileRights, accessControlType);
    }

    public string GetGroups()
    {
        return _fileManagerRepository.GetGroups();
    }

    public string GetUsers()
    {
        return _fileManagerRepository.GetUsers();
    }

    public string[] GetDisks()
    {
        return Directory.GetLogicalDrives();
    }

    public void OpenFile(string path)
    {
        _fileManagerRepository.OpenFile(path);
    }
}