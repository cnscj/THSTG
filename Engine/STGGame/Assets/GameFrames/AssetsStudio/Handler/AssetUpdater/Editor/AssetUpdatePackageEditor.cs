using System.Collections;
using System.Collections.Generic;
using System.IO;
using ASGame;
using UnityEditor;
using UnityEngine;
using XLibEditor;
using XLibrary;

namespace ASEditor
{
    public class AssetUpdatePackageEditor : WindowGUI<AssetUpdatePackageEditor>
    {
        class AssetItem
        {
            public string path;
            public bool isSelected;
        }

        class ConfigItem
        {
            public string path;
            public bool isSelected;
            public int package;
            public int flag;
        }
        private string _saveFilePath;
        private string _manifestPath = "";
        private string _assetFolderPath = "";

        private List<string> _assetFolderList;
        private Dictionary<string, HashSet<string>> _manifestDict;
        private Dictionary<string, ConfigItem> _configDict;

        private SearchTextField _srcSearchTextField = new SearchTextField();
        private SearchTextField _destSearchTextField = new SearchTextField();

        private List<AssetItem> _assetList = new List<AssetItem>();
        private List<ConfigItem> _configList = new List<ConfigItem>();

        private Vector2 _scrollPos1 = Vector2.zero;
        private Vector2 _scrollPos2 = Vector2.zero;

        [MenuItem("AssetsStudio/资源工具/资源分包器")]
        static void ShowWnd()
        {
            ShowWindow("资源分包配置");
        }

        public AssetUpdatePackageEditor()
        {
            _srcSearchTextField.OnChanged(() =>
            {
                RefreshAssetList();
            });

            _destSearchTextField.OnChanged(() =>
            {
                RefreshConfigList();
            });
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
                    ImportExAsset(_assetFolderPath);//扫描目录
                }
            }
            EditorGUILayout.EndHorizontal();

            //2个滚动的视图
            EditorGUILayout.BeginHorizontal();

            ShowAllAssetList();
            ShowConfigList();
            //ShowConfigPanel();

            EditorGUILayout.EndHorizontal();
            ///
            EditorGUILayout.EndVertical();

            EditorGUILayout.EndHorizontal();
        }

        private void ShowAllAssetList()
        {
            EditorGUILayout.BeginVertical();
            _srcSearchTextField.OnGUI();
            _scrollPos1 = EditorGUILayout.BeginScrollView(_scrollPos1, (GUIStyle)"Asset List");

            foreach (var item in _assetList)
            {
                string fileName = Path.GetFileName(item.path);
                EditorGUILayout.BeginHorizontal();
                //item.isSelected = GUILayout.Toggle(item.isSelected,"", GUILayout.Width(15));
 
                if (GUILayout.Button(fileName, GUILayout.Width(250)))
                {
                    Debug.Log(item.path);
                }
                EditorGUILayout.EndHorizontal();
            }

            EditorGUILayout.EndScrollView();
            EditorGUILayout.EndVertical();
        }

        private void ShowConfigList()
        {
            EditorGUILayout.BeginVertical();
            _destSearchTextField.OnGUI();
            _scrollPos2 = EditorGUILayout.BeginScrollView(_scrollPos2, (GUIStyle)"Config Lis");

            EditorGUILayout.EndScrollView();
            EditorGUILayout.EndVertical();
        }

        private void ShowConfigPanel()
        {
            EditorGUILayout.BeginVertical();
  

            EditorGUILayout.EndScrollView();
            EditorGUILayout.EndVertical();
        }

        private void NewConfigFile()
        {
            _configDict = null;
            _configList.Clear();

            RefreshConfigList();
        }

        private void OpenConfigFile(string filePath)
        {
            var assetUpdateConfigList = new AssetUpdateConfigList();
            assetUpdateConfigList.Import(filePath);
            var configList = assetUpdateConfigList.GetItemList();
            foreach(var item in configList)
            {

            }

            RefreshConfigList();
        }

        private void SaveConfigFile(string filePath)
        {
            var assetUpdateConfigList = new AssetUpdateConfigList();
            var dict = assetUpdateConfigList.GetDict();
            foreach (var item in _configList)
            {
                if (!dict.ContainsKey(item.path))
                {
                    var cfgItem = new AssetUpdateConfigList.Item();
                    cfgItem.filePath = item.path;
                    cfgItem.flag = item.flag;
                    cfgItem.packageId = item.package;
                    dict.Add(cfgItem.filePath, cfgItem);
                }
            }
            assetUpdateConfigList.Export(filePath);
        }

        private void LoadManifestFile(string filePath)
        {
            AssetBundleManifest mainfest = AssetDatabase.LoadAssetAtPath<AssetBundleManifest>(filePath);
            _manifestDict = new Dictionary<string, HashSet<string>>();

            foreach (var bundle in mainfest.GetAllAssetBundles())
            {
                var deps = mainfest.GetAllDependencies(bundle);
                HashSet<string> depSet = null;
                if (!_manifestDict.TryGetValue(bundle,out depSet))
                {
                    depSet = new HashSet<string>();
                    _manifestDict.Add(bundle, depSet);
                }
                foreach(var dep in deps)
                {
                    if (!depSet.Contains(dep))
                        depSet.Add(dep);
                }
            }
            RefreshAssetList();
        }

        private void ImportExAsset(string folderPath)
        {
            _assetFolderPath = folderPath;
            _assetFolderList = new List<string>();

            XFolderTools.TraverseFiles(_assetFolderPath, (fullPath) =>
            {
                string exName = Path.GetExtension(fullPath);
                _assetFolderList.Add(fullPath);
            }, true);

            RefreshAssetList();
        }

        private void AddConfig(string key)
        {

        }

        private void RemoveCofig(string key)
        {

        }

        private void RefreshAssetList()
        {
            _assetList.Clear();
            foreach(var path in _assetFolderList)
            {
                string key = path;
                if (_configDict != null)
                {
                    if (_configDict.ContainsKey(key))
                    {
                        continue;
                    }
                }

                string searchKey = _srcSearchTextField.GetText();
                if (!string.IsNullOrEmpty(searchKey))
                {
                    if (key.IndexOf(searchKey) < 0)
                    {
                        continue;
                    }
                }
                var item = new AssetItem();
                item.path = path;
                _assetList.Add(item);
            }
        }

        private void RefreshConfigList()
        {
            _configList.Clear();
        }
    }

}
