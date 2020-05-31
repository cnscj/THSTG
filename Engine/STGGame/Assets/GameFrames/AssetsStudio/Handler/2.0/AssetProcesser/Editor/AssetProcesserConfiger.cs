using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using XLibEditor;
using XLibrary;

namespace ASEditor
{
    public class AssetProcesserConfiger : ConfigObject<AssetProcesserConfiger>
    {
        public static readonly string DEFAULT_OUTPUT_FOLDER_PATH = "Assets/ASOutput";
        public static readonly string DEFAULT_MD5_FOLDER_NAME = "Checkfile";
        public static readonly string DEFAULT_PROCESS_FOLDER_NAME = "";

        public string outputFolderPath = DEFAULT_OUTPUT_FOLDER_PATH;
        public string processFolderName = DEFAULT_PROCESS_FOLDER_NAME;
        public string checkfileFolderName = DEFAULT_MD5_FOLDER_NAME;
        public bool createFolderOrAddSuffix = true;
        public bool useGUID4SaveCheckfileName;



        public string GetOutputFolderPath()
        {
            return string.IsNullOrEmpty(outputFolderPath) ? DEFAULT_OUTPUT_FOLDER_PATH : outputFolderPath;
        }

        public string GetCheckfileFloderPath()
        {
            string newCheckfileFolderName = string.IsNullOrEmpty(checkfileFolderName) ? DEFAULT_MD5_FOLDER_NAME : checkfileFolderName;
            string checkfileFolderPath = XPathTools.Combine(GetOutputFolderPath(), newCheckfileFolderName);
            return checkfileFolderPath;
        }

        public string GetCheckfileSaveFolderPath(string progressName)
        {
            string checkfileFolderPath = GetCheckfileFloderPath();
            string checkfileSaveParentPath = checkfileFolderPath;
            if (createFolderOrAddSuffix)
            {
                checkfileSaveParentPath = XPathTools.Combine(checkfileFolderPath, progressName);
            }
            return checkfileSaveParentPath;
        }

        public string GetCheckfileSavePath(string progressName, string srcFilePath, string suffix = null)
        {
            string outputName = Path.GetFileNameWithoutExtension(srcFilePath);      //有些可能是同一个Key,二层Key的
            string checkfileSaveParentPath = GetCheckfileSaveFolderPath(progressName);
            string cehckfileSaveName = outputName;

            if (!createFolderOrAddSuffix)
            {
                if (string.IsNullOrEmpty(suffix))
                {
                    cehckfileSaveName = string.Format("{0}_{1}", cehckfileSaveName, progressName);
                }
                else
                {
                    cehckfileSaveName = string.Format("{0}_{1}{2}", cehckfileSaveName, progressName, suffix);
                }
            }
            else
            {
                if (string.IsNullOrEmpty(suffix))
                {
                    cehckfileSaveName = string.Format("{0}", cehckfileSaveName);
                }
                else
                {
                    cehckfileSaveName = string.Format("{0}{1}", cehckfileSaveName, suffix);
                }
            }

            if (useGUID4SaveCheckfileName)
            {
                cehckfileSaveName = AssetDatabase.AssetPathToGUID(srcFilePath);
            }

            string md5SavePath = XPathTools.Combine(checkfileSaveParentPath, cehckfileSaveName);

            return md5SavePath;
        }

        public string GetProcessFloderPath()
        {
            string processFolderPath;
            if (string.IsNullOrEmpty(processFolderName))
            {
                processFolderPath = GetOutputFolderPath();
            }
            else
            {
                processFolderPath = XPathTools.Combine(GetOutputFolderPath(), processFolderName);
            }
            return processFolderPath;
        }

        public string GetProcessSaveFolderPath(string progressName)
        {
            return Path.Combine(GetProcessFloderPath(), progressName);
        }
    }
}
