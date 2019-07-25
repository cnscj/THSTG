using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using THGame;

namespace THEditor
{
    public class BundleBuilderDefault : BundleBuilder
    {
        protected BundleBuilderInfos m_buildInfo;
        public BundleBuilderDefault(BundleBuilderInfos infos)
        {
            m_buildInfo = infos;
        }

        protected override List<string> OnFilter()
        {
            List<string> filList = new List<string>();
            XFolderTools.TraverseFiles(m_buildInfo.srcResFolder, (fullPath) =>
            {
                string relaPath = XFileTools.GetFileRelativePath(fullPath);
                filList.Add(relaPath);
            });
            return filList;
        }


        protected override void OnPreOnce(string assetPath)
        {

        }

        protected override void OnOnce(string assetPath)
        {

        }

    }
}

