using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using THGame;
using System.IO;

namespace THEditor
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

        protected override void OnOnce(string assetPath)
        {
            string fileNameNotEx = Path.GetFileNameWithoutExtension(assetPath);
            SetBundleName(assetPath, string.Format(m_buildInfo.bundleName, fileNameNotEx));
        }

        protected override void OnShareOnce(string assetPath, int dependCount)
        {
            SetBundleName(assetPath, string.Format(m_buildInfo.bundleName, "share.ab"));
        }
    }
}

