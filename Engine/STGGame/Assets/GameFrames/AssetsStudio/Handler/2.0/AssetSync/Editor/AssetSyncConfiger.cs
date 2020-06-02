using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using XLibEditor;
using XLibrary;

namespace ASEditor
{
    public class AssetSyncConfiger : ConfigObject<AssetSyncConfiger>
    {
        public int minVersion;                      //需要同步的最小版本
        public string repositoryRootPath;           //版本库路径
        public List<AssetSyncItem> syncItems;

        public string GetRepositoryRootPath()
        {
            return repositoryRootPath;
        }

        //相对与版本库下层路径
        public string GetCheckFolderRelativePath(string fullPath)
        {
            if (string.IsNullOrEmpty(repositoryRootPath))
                return "";
            if (string.IsNullOrEmpty(fullPath))
                return "";
            if (fullPath.IndexOf(repositoryRootPath) < 0)
                return "";

            fullPath = XPathTools.NormalizePath(fullPath);
            string relaPath = fullPath.Replace(repositoryRootPath, "");
            if (relaPath.StartsWith("/")) relaPath = relaPath.Remove(0,1);

            int startPos = relaPath.IndexOf("/");
            if (startPos > 0) relaPath = relaPath.Remove(0, startPos+1);

            return relaPath;
        }

        public int GetRepositoryVersion(string path)
        {
            string folderName = Path.GetFileNameWithoutExtension(path);
            int.TryParse(folderName, out int version);
            return version;
        }
    }
}
