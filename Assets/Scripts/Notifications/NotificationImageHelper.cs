using System.IO;
using UnityEngine;

public static class NotificationImageHelper
{
    
    public static string GetPersistentImagePath(string fileName)
    {
        return Path.Combine(Application.persistentDataPath, fileName);
    }
    public static string CopyImageToPersistentPath(string fileName)
    {
        string sourcePath = Path.Combine(Application.streamingAssetsPath, "Notifications", fileName);
        string targetPath = GetPersistentImagePath( fileName);

        if (!File.Exists(targetPath))
        {
            if (sourcePath.Contains("://")) // Android / iOS
            {
                var www = new WWW(sourcePath);
                while (!www.isDone) { }
                File.WriteAllBytes(targetPath, www.bytes);
            }
            else
            {
                File.Copy(sourcePath, targetPath, true);
            }
        }

        return targetPath;
    }
}