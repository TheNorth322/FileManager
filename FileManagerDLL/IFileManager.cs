using System.IO;
using System.Security.AccessControl;

namespace FileManager;

public interface IFileManager
{
    void RenameFile(string old_path, string new_path);
    string[] GetDirectoryContents(string path);
    void Remove(string path);
    void RemoveDirectoryContents(string path);
    void CopyFile(string old_path, string new_path);
    void CreateDirectory(string path);
    long GetFileSize(string path);
    void ChangeFileAccess(string filePath, string userOrGroupName, FileSystemRights fileRights,
        AccessControlType accessControlType);
    string GetGroups();
    string GetUsers();

    void OpenFile(string path);
}