using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using THGame;
using UnityEditor;
using System.IO;

namespace THEditor
{
    public class BundleBuilder
    {
        private string m_outFolder = "";
        protected BuildAssetBundleOptions m_bundleOptions;
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

            //打包设置
            m_bundleOptions = BuildAssetBundleOptions.None;
            m_bundleOptions |= BuildAssetBundleOptions.ChunkBasedCompression;
            m_bundleOptions |= BuildAssetBundleOptions.DisableLoadAssetByFileName;
            m_bundleOptions |= BuildAssetBundleOptions.DisableLoadAssetByFileNameWithExtension;
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

        protected void SetBundleName(string assetPath,string name)
        {
            AssetImporter assetImporter = AssetImporter.GetAtPath(assetPath);
            if (assetImporter != null)
            {
                if (assetImporter.assetBundleName == "")
                {
                    string assetBundleName = name;
                    assetImporter.assetBundleName = assetBundleName;
                }
            }
        }

        protected virtual List<string> OnFilter()
        {
            List<string> filList = new List<string>();
            return filList;
        }

         
        protected virtual void OnOnce(string assetPath)
        {
            string fileNameNotEx = Path.GetFileNameWithoutExtension(assetPath);
            SetBundleName(assetPath, fileNameNotEx);
        }

        void BuildBegin()
        {

        }

        void BuildEnd()
        {

        }

        void BuildOnce(string assetPath)
        {
            OnOnce(assetPath);

            AssetImporter assetImporter = AssetImporter.GetAtPath(assetPath);
            if (assetImporter != null)
            {
                if (assetImporter.assetBundleName != "")
                {
                    //BuildPipeline.BuildAssetBundles(assetPath, m_bundleOptions, BundleBuilderConfig.GetInstance().GetBuildType());
                }
            }
            

        }

        
       
    }
}

