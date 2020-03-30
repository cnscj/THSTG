using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using XLibEditor;

namespace ASEditor
{
    public class AssetProcesserConfiger : ConfigObject<AssetProcesserConfiger>
    {
        public static readonly string DEFAULT_OUTPUT_FOLDER_PATH = "Assets/ASOutput";
        public static readonly string DEFAULT_MD5_FOLDER_NAME = "Md5";
        public static readonly string DEFAULT_PROCESS_FOLDER_NAME = "Process";

        public string outputFolderPath = DEFAULT_OUTPUT_FOLDER_PATH;
        public string md5FolderName = DEFAULT_MD5_FOLDER_NAME;
        public bool createFolderOrAddSuffix;
        public bool useGUID4SaveMd5Name;

        public string processFolderName = DEFAULT_MD5_FOLDER_NAME;

        public string GetOutputFolderPath()
        {
            return string.IsNullOrEmpty(outputFolderPath) ? DEFAULT_OUTPUT_FOLDER_PATH : outputFolderPath;
        }

        public string GetMd5FloderPath()
        {
            string newMd5FolderName = string.IsNullOrEmpty(md5FolderName) ? DEFAULT_MD5_FOLDER_NAME : md5FolderName;
            string md5FolderPath = Path.Combine(GetOutputFolderPath(), newMd5FolderName);
            return md5FolderPath;
        }

        public string GetMd5SaveFolderPath(string progressName)
        {
            string md5FolderPath = GetMd5FloderPath();
            string md5SaveParentPath = md5FolderPath;
            if (createFolderOrAddSuffix)
            {
                md5SaveParentPath = Path.Combine(md5FolderPath, progressName);
            }
            return md5SaveParentPath;
        }

        public string GetMd5SavePath(string progressName, string srcFilePath)
        {
            string outputName = Path.GetFileNameWithoutExtension(srcFilePath);
            string md5SaveParentPath = GetMd5SaveFolderPath(progressName);
            string md5SaveName = outputName;

            if (!createFolderOrAddSuffix)
            {
                md5SaveName = string.Format("{0}_{1}", md5SaveName, progressName);
            }

            if (useGUID4SaveMd5Name)
            {
                md5SaveName = AssetDatabase.AssetPathToGUID(srcFilePath);
            }

            string md5SavePath = Path.Combine(md5SaveParentPath, md5SaveName);

            return md5SavePath;
        }

        public string GetProcessFloderPath()
        {
            string newProcessFolderName = string.IsNullOrEmpty(processFolderName) ? DEFAULT_PROCESS_FOLDER_NAME : processFolderName;
            string processFolderPath = Path.Combine(GetOutputFolderPath(), newProcessFolderName);
            return processFolderPath;
        }

        public string GetProcessSaveFolderPath(string progressName)
        {
            return Path.Combine(GetProcessFloderPath(), progressName);
        }
    }
}
