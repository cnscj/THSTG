using System.Collections;
using System.Collections.Generic;
using ASGame;
using UnityEditor;
using UnityEngine;
using XLibEditor;

namespace ASEditor
{
    public class AssetUpdatePackageEditor : WindowGUI<AssetUpdatePackageEditor>
    {
        private string _saveFilePath;
        private string _manifestPath = "";
        private string _assetFolderPath = "";

        private SearchTextField _srcSearchTextField = new SearchTextField();
        private SearchTextField _destSearchTextField = new SearchTextField();
        private AssetUpdateConfigList _assetUpdateConfigList = new AssetUpdateConfigList();


        [MenuItem("AssetsStudio/资源工具/资源分包器")]
        static void ShowWnd()
        {
            ShowWindow("资源分包配置");
        }

        //参考PackagesWindow
        protected override void OnShow()
        {
            EditorGUILayout.BeginHorizontal();

            GUILayout.Space(5);

            EditorGUILayout.BeginVertical();

            //打开文件
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("新建", GUILayout.Width(100)))
            {
                NewConfigFile();
            }

            if (GUILayout.Button("打开", GUILayout.Width(100)))
            {
                string openFilePath = EditorUtility.OpenFilePanel("OpenFile", "Assets", "txt");
                if (!string.IsNullOrEmpty(openFilePath))
                {
                    OpenConfigFile(openFilePath);
                }
            }
            //保存
            if (GUILayout.Button("保存", GUILayout.Width(100)))
            {
                if (string.IsNullOrEmpty(_saveFilePath))
                {
                    _saveFilePath = EditorUtility.SaveFilePanel("SaveFile", "Assets", "configList", "txt");
                }

                if (!string.IsNullOrEmpty(_saveFilePath))
                {
                    SaveConfigFile(_saveFilePath);
                }
            }
            EditorGUILayout.EndHorizontal();

            //2个打开条
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.TextField("依赖文件", _manifestPath);
            if (GUILayout.Button("打开", GUILayout.Width(100)))
            {
                _manifestPath = EditorUtility.OpenFilePanel("OpenFile", "Assets", "manifest");
                if (!string.IsNullOrEmpty(_manifestPath))
                {
                    //扫描依赖关系
                    LoadManifestFile(_manifestPath);
                }
            }
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.TextField("资源目录", _assetFolderPath);
            if (GUILayout.Button("浏览", GUILayout.Width(100)))
            {
                _assetFolderPath = EditorUtility.OpenFolderPanel("OpenFolder", _assetFolderPath,"");
                if (!string.IsNullOrEmpty(_assetFolderPath))
                {
                    //扫描目录
                    ImportExAsset(_assetFolderPath);
                }
            }
            EditorGUILayout.EndHorizontal();

            //2个滚动的视图
            EditorGUILayout.BeginHorizontal();
            //TODO:

           
            EditorGUILayout.EndHorizontal();
            ///
            EditorGUILayout.EndVertical();

            EditorGUILayout.EndHorizontal();
        }

        private void NewConfigFile()
        {
            _assetUpdateConfigList = new AssetUpdateConfigList();
        }

        private void OpenConfigFile(string filePath)
        {
            NewConfigFile();
            _assetUpdateConfigList.Import(filePath);
        }

        private void SaveConfigFile(string filePath)
        {
            _assetUpdateConfigList.Export(filePath);
        }

        private void LoadManifestFile(string filePath)
        {
            AssetBundleManifest mainfest = AssetDatabase.LoadAssetAtPath<AssetBundleManifest>(filePath);

        }

        private void ImportExAsset(string folderPath)
        {

        }
    }

}
