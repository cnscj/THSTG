using System.Collections.Generic;
using System.IO;
using System.Linq;
using ASGame;
using UnityEngine;
using XLibrary;

namespace ASEditor
{
    public class ResourceBuilderSimple : ResourceBuilder
    {
        protected ResourceBuilderInfos m_buildInfo;
        public ResourceBuilderSimple(ResourceBuilderInfos infos)
        {
            m_buildInfo = infos;
        }

        protected override List<string> OnFilter()
        {
            //取得后缀,用"|"分割
            List<string> filList = new List<string>();
            Dictionary<string, bool> suffixMap = null;
            if (!string.IsNullOrEmpty(m_buildInfo.srcBundleSuffix))
            {
                suffixMap = new Dictionary<string, bool>();
                string[] suffixList = m_buildInfo.srcBundleSuffix.Split('|');
                
                foreach(var suffix in suffixList)
                {
                    suffixMap[suffix] = true;
                }
            }
            
            XFolderTools.TraverseFiles(m_buildInfo.srcResFolder, (fullPath) =>
            {
                string relaPath = XPathTools.GetRelativePath(fullPath);
                string fileEx = Path.GetExtension(relaPath).ToLower();
                if (fileEx.Contains("meta"))    //忽略meta文件
                {
                    return;
                }

                if (suffixMap != null)
                {
                    if (!suffixMap.ContainsKey(fileEx))
                    {
                        return;
                    }
                }

                filList.Add(relaPath);
            }, m_buildInfo.isTraversal);
            return filList;
        }

        protected string GetModulePath(string assetPath)
        {
            string srcResFolder = m_buildInfo.srcResFolder.Replace("\\", "/");
            string newAssetPath = assetPath.Replace("\\", "/");
            if (assetPath.Contains(srcResFolder))
            {
                string relaPath = assetPath.Replace(string.Format("/{0}", srcResFolder),"");
                string relaParentPath = relaPath.Replace(Path.GetFileName(newAssetPath), "");
                return relaParentPath;
            }
            return Path.GetDirectoryName(newAssetPath);
        }

        protected override void OnOnce(string assetPath)
        {

            string fileRootName = Path.GetFileNameWithoutExtension(m_buildInfo.srcResFolder);
            string fileNameNotEx = Path.GetFileNameWithoutExtension(assetPath);

            string subFilePath = assetPath;
            subFilePath = subFilePath.Replace(string.Format("{0}", m_buildInfo.srcResFolder), "");
            subFilePath = subFilePath.Replace(string.Format("{0}", Path.GetFileName(assetPath)), "");
            subFilePath = subFilePath.TrimStart('/').TrimEnd('/');
            string subFolderName = m_buildInfo.isUsePathName ? subFilePath.Replace("/", "_") : subFilePath;
            string outName = m_buildInfo.isCommonPrefixion ? XStringTools.SplitPathModule(fileNameNotEx) : fileNameNotEx;
            if (subFolderName == "")
            {
                string newFormat = m_buildInfo.bundleName;
                newFormat = newFormat.Replace("/{1}", "");
                newFormat = newFormat.Replace("{2}", "{1}");
                SetBundleName(assetPath, string.Format(newFormat, fileRootName.ToLower(), outName.ToLower()));
            }
            else
            {
                SetBundleName(assetPath, string.Format(m_buildInfo.bundleName, fileRootName.ToLower(), subFolderName.ToLower(), outName.ToLower()));
            }
            
        }

        protected override void OnShareOnce(string assetPath, int dependCount)
        {
            if (ResourceBuilderConfig.GetInstance().isBuildShare)
            {
                string srcPathRootName = Path.GetFileNameWithoutExtension(m_buildInfo.srcResFolder);
                SetBundleName(assetPath, string.Format(m_buildInfo.shareBundleName, srcPathRootName.ToLower()));
            }
        }
    }
}

