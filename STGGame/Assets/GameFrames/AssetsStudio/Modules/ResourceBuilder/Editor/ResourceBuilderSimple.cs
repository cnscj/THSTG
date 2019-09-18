using System.Collections.Generic;
using System.IO;
using ASGame;
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
            List<string> filList = new List<string>();
            XFolderTools.TraverseFiles(m_buildInfo.srcResFolder, (fullPath) =>
            {
                string relaPath = XFileTools.GetFileRelativePath(fullPath);
                string fileEx = Path.GetExtension(relaPath).ToLower();
                if (fileEx.Contains("meta"))
                {
                    return;
                }
                filList.Add(relaPath);
            });
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
            if (!m_buildInfo.isSubFolderBuildOne)
            {
                string fileNameNotEx = Path.GetFileNameWithoutExtension(assetPath).ToLower();
                SetBundleName(assetPath, string.Format(m_buildInfo.bundleName, fileNameNotEx));
            }
            else
            {
                string modelName = GetModulePath(assetPath).ToLower();
                SetBundleName(assetPath, string.Format(m_buildInfo.bundleName, modelName));
            }
        }

        protected override void OnShareOnce(string assetPath, int dependCount)
        {
            if (!m_buildInfo.isSubFolderBuildOne)
            {
                SetBundleName(assetPath, string.Format(m_buildInfo.bundleName, "share.ab"));
            }
        }
    }
}

