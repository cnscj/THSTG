using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using THGame;

namespace THEditor
{
    public class BundleBuilder
    {
        private string m_outFolder = "";
        public BundleBuilder(string outFolder)
        {
            m_outFolder = outFolder;
            if (m_outFolder != "")
            {
                if (!XFolderTools.Exists(m_outFolder))
                {
                    XFolderTools.CreateDirectory(m_outFolder);
                }
            }
        }
        public virtual void Build()
        {
            BuildBegin();

            var fileList = OnFilter();
            foreach (var assetPath in fileList)
            {
                BuildOnce(assetPath);
            }

            BuildEnd();
        }

        protected virtual List<string> OnFilter()
        {
            List<string> filList = new List<string>();
            return filList;
        }


        protected virtual void OnPreOnce(string assetPath)
        {

        }

        protected virtual void OnOnce(string assetPath)
        {
            //TODO:默认处理方式
        }

        void BuildBegin()
        {

        }

        void BuildEnd()
        {

        }

        void BuildOnce(string assetPath)
        {

            OnPreOnce(assetPath);

            OnOnce(assetPath);
        }
    }
}

