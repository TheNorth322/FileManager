﻿using System;
using System.IO;
using System.Security.AccessControl;
using System.Security.Principal;
using FileManager.Model.Interfaces;
using Microsoft.VisualBasic;

namespace FileManager.Domain.Entities;

public class FileManager : IFileManager
{
    public void RenameFile(string old_path, string new_path)
    {
        CheckFileExist(old_path);
        File.Move(old_path, new_path);
    }

    // TODO
    public string[] GetDirectoryContents(string path)
    {
        CheckDirectoryExist(path);

        string[] directoryInfos = Directory.GetDirectories(path);
        string[] fileInfos = Directory.GetFiles(path);
        string[] directoryContets = new string[directoryInfos.Length + fileInfos.Length];

        int i = 0;

        for (; i < directoryInfos.Length; i++)
            directoryContets[i] = directoryInfos[i];

        for (; i < fileInfos.Length + directoryInfos.Length; i++)
            directoryContets[i] = fileInfos[i - fileInfos.Length + 1];

        return directoryContets;
    }

    public void RemoveFile(string path)
    {
        CheckFileExist(path);

        File.Delete(path);
    }

    public void RemoveDirectoryContents(string path)
    {
        CheckDirectoryExist(path);
        DirectoryInfo dir = new DirectoryInfo(path);
        dir.Delete(true);
    }

    public void CopyFile(string old_path, string new_path)
    {
        CheckFileExist(old_path);

        File.Copy(old_path, new_path);
    }

    public void CreateDirectory(string path)
    {
        try
        {
            CheckDirectoryExist(path);
        }
        catch (DirectoryNotFoundException ex)
        {
            Directory.CreateDirectory(path);
        }
    }

    public long GetFileSize(string path)
    {
        CheckFileExist(path);
        return new FileInfo(path).Length;
    }


    public void ChangeFileAccess(string filePath, string userOrGroupName, FileSystemRights fileRights,
        AccessControlType accessControlType)
    {
        CheckFileExist(filePath);

        FileInfo fileInfo = new FileInfo(filePath);
        FileSecurity fileSecurity = fileInfo.GetAccessControl();

        fileSecurity.AddAccessRule(new FileSystemAccessRule(userOrGroupName, fileRights, accessControlType));
        fileInfo.SetAccessControl(fileSecurity);
    }
        
    public string GetGroups()
    {
        string groups = new NTAccount("NT AUTHORITY\\Authenticated Users").Translate(typeof(SecurityIdentifier)).Value;
        SecurityIdentifier groupsIdentity = new SecurityIdentifier(groups);
        NTAccount groupsGroup = (NTAccount)groupsIdentity.Translate(typeof(NTAccount));
        return groupsGroup.Value;
    }

    public string GetUsers()
    {
        string users = new NTAccount("NT AUTHORITY\\Authenticated Users").Translate(typeof(SecurityIdentifier)).Value;
        SecurityIdentifier usersIdentity = new SecurityIdentifier(users);
        NTAccount usersGroup = (NTAccount)usersIdentity.Translate(typeof(NTAccount));
        return usersGroup.Value;
    }

    private void CheckFileExist(string path)
    {
        if (!File.Exists(path))
            throw new FileNotFoundException(nameof(path));
    }

    private void CheckDirectoryExist(string path)
    {
        if (!Directory.Exists(path))
            throw new DirectoryNotFoundException(nameof(path));
    }
}