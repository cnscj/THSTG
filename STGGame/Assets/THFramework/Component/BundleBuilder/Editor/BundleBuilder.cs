﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using THGame;
using UnityEditor;
using System.IO;

namespace THEditor
{
    public class BundleBuilder
    {
       
        protected Dictionary<string, int> m_dependencies;
        public BundleBuilder()
        {
     
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

        protected void SetBundleName(string assetPath,string name,string variant = null)
        {
            AssetImporter assetImporter = AssetImporter.GetAtPath(assetPath);
            if (assetImporter != null)
            {
                if (assetImporter.assetBundleName == "")
                {
                    string bundleName = BundleBuilderConfig.GetInstance().isUseLower ? name.ToLower() : name;
                    assetImporter.assetBundleName = bundleName;         //包名
                    if (variant != null)
                    {
                        assetImporter.assetBundleVariant = variant;     //设置扩展名
                        assetImporter.SaveAndReimport();
                    }
                    

                }
            }
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
            string fileNameNotEx = Path.GetFileNameWithoutExtension(assetPath);
            SetBundleName(assetPath, string.Format("{0}.ab", fileNameNotEx));
        }

        protected virtual void OnShareOnce(string assetPath,int dependCount)
        {
            string defShareBundleName = BundleBuilderConfig.GetInstance().shareBundleName;
            SetBundleName(assetPath, string.Format("{0}", defShareBundleName == "" ? "share.ab" : defShareBundleName));
        }

        void BuildBegin()
        {
            Clear();
            if (BundleBuilderConfig.GetInstance().isBuildShare)
            {
                m_dependencies = new Dictionary<string, int>();
            }
        }

        void BuildEnd()
        {
            //公共打包
            if (m_dependencies != null)
            {
                foreach(var pair in m_dependencies)
                {
                    if (pair.Value > 1)
                    {
                        string assetPath = pair.Key;
                        OnShareOnce(assetPath, pair.Value);
                    }
                }
            }

            //单独打包

 
        }

        void BuildOnce(string assetPath)
        {
            OnPreOnce(assetPath);

            //依赖收集
            if (m_dependencies != null)
            {
                string[] dps = AssetDatabase.GetDependencies(assetPath);
                foreach (var dp in dps)
                {
                    string fileEx = Path.GetExtension(dp).ToLower();
                    if (fileEx.Contains("cs")|| fileEx.Contains("prefab"))
                    {
                        continue;
                    }

                    if (m_dependencies.ContainsKey(dp))
                    {
                        m_dependencies[dp]++;
                    }
                    else
                    {
                        m_dependencies.Add(dp, 1);
                    }

                }
            }

            OnOnce(assetPath);
        }

        void Clear()
        {
            m_dependencies = null;
        }
       
    }
}

