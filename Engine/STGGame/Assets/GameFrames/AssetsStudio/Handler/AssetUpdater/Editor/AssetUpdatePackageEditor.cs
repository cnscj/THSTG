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
            public int type;
            public int flag;
        }
        private string _saveFilePath;
        private string _manifestPath = "";
        private string _assetFolderPath = "";

        private List<string> _assetFolderList;
        private Dictionary<string, HashSet<string>> _manifestDict;

        private SearchTextField _srcSearchTextField = new SearchTextField();
        private SearchTextField _destSearchTextField = new SearchTextField();

        private List<AssetItem> _assetList = new List<AssetItem>();
        private List<ConfigItem> _configList = new List<ConfigItem>();

        private Queue<string> _addQueue = new Queue<string>();
        private Queue<string> _removeQueue = new Queue<string>();

        private Vector2 _scrollPos1 = Vector2.zero;
        private Vector2 _scrollPos2 = Vector2.zero;
        private Vector2 _scrollPos3 = Vector2.zero;

        private Dictionary<string, ConfigItem> _configDict = new Dictionary<string, ConfigItem>();

        private ConfigItem _selectedItem = null;

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
            if (GUILayout.Button("清理", GUILayout.Width(100)))
            {
                if(string.IsNullOrEmpty(_assetFolderPath))
                {
                    EditorUtility.DisplayDialog("警告", "请先填写资源路径在进行操作","知道了");
                    return;
                }
                else
                {
                    PurgeFileDict(_assetFolderPath);
                }
            }

            EditorGUILayout.EndHorizontal();

            //2个打开条
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

            //2个滚动的视图
            EditorGUILayout.BeginHorizontal();

            ShowAllAssetList();
            ShowConfigList();
            ShowConfigPanel();

            NeedRefresh();

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
                item.isSelected = GUILayout.Toggle(item.isSelected, fileName, GUILayout.Width(250));

                if (GUILayout.Button(">>", GUILayout.Width(20)))
                {
                    _addQueue.Enqueue(item.path);
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

            foreach(var item in _configList)
            {
                string fileName = Path.GetFileName(item.path);
                EditorGUILayout.BeginHorizontal();
                if (GUILayout.Button(fileName, GUILayout.Width(250)))
                {
                    _selectedItem = item;
                    //TODO:需要重绘界面
                }
                if (GUILayout.Button("<<", GUILayout.Width(20)))
                {
                    _removeQueue.Enqueue(item.path);
                }
                EditorGUILayout.EndHorizontal();
            }

            EditorGUILayout.EndScrollView();
            EditorGUILayout.EndVertical();
        }

        private void ShowConfigPanel()
        {
            EditorGUILayout.BeginVertical();
            _scrollPos3 = EditorGUILayout.BeginScrollView(_scrollPos3, (GUIStyle)"Config Panel");
            if (_selectedItem != null)
            {
                GUILayout.Label(string.Format("资源路径:{0}", _selectedItem.path));
                _selectedItem.package = EditorGUILayout.IntField("所在包:", _selectedItem.package);
                _selectedItem.type = EditorGUILayout.IntField("资源类型:", _selectedItem.type);
                _selectedItem.flag = EditorGUILayout.IntField("标志:", _selectedItem.flag);

            }
            EditorGUILayout.EndScrollView();

            EditorGUILayout.EndVertical();
        }

        private void NeedRefresh()
        {
            bool isNeedRefresh = false;
            if (_addQueue.Count > 0)
            {
                while (_addQueue.Count > 0)
                {
                    var patn = _addQueue.Dequeue();
                    AddConfig(patn);
                }
                isNeedRefresh = true;
            }

            if (_removeQueue.Count > 0)
            {
                while (_removeQueue.Count > 0)
                {
                    var patn = _removeQueue.Dequeue();
                    RemoveCofig(patn);
                }
                isNeedRefresh = true;
            }

           
            if (isNeedRefresh)
            {
                RefreshAssetList();
                RefreshConfigList();
            }
        }

        private void NewConfigFile()
        {
            _configDict.Clear();
            _configList.Clear();
            _saveFilePath = "";

            RefreshConfigList();
        }

        private void OpenConfigFile(string filePath)
        {
            var assetUpdateConfigList = new AssetUpdateConfigList();
            assetUpdateConfigList.Import(filePath);
            var configList = assetUpdateConfigList.GetItemList();
            _configDict.Clear();
            foreach (var item in configList)
            {
                var key = item.filePath;
                ConfigItem cfgItem = null;
                if (!_configDict.TryGetValue(key,out cfgItem))
                {
                    cfgItem = new ConfigItem();
                }
                cfgItem.path = item.filePath;
                cfgItem.package = item.packageId;
                cfgItem.type = item.resourceType;
                cfgItem.flag = item.flag;

                _configDict[key] = cfgItem;
            }

            RefreshConfigList();
        }

        private void SaveConfigFile(string filePath)
        {
            var assetUpdateConfigList = new AssetUpdateConfigList();
            var dict = assetUpdateConfigList.GetDict();
            foreach (var item in _configDict.Values)
            {
                if (!dict.ContainsKey(item.path))
                {
                    var cfgItem = new AssetUpdateConfigList.Item();
                    cfgItem.filePath = item.path;
                    cfgItem.resourceType = item.type;
                    cfgItem.flag = item.flag;
                    cfgItem.packageId = item.package;

                    dict.Add(cfgItem.filePath, cfgItem);
                }
            }
            assetUpdateConfigList.Export(filePath);
            Debug.Log(string.Format("导出成功:{0}", filePath));
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

        private void PurgeFileDict(string folderPath)
        {
            if (string.IsNullOrEmpty(folderPath))
                return;

            if (!Directory.Exists(folderPath))
                return;

            foreach(var pair in _configDict)
            {
                string fullPath = Path.Combine(folderPath, pair.Value.path);
                if (!File.Exists(fullPath))
                {
                    _configDict.Remove(pair.Key);
                }
            }
        }

        private void AddConfig(string fullPath)
        {
            if (string.IsNullOrEmpty(fullPath))
                return;

            string relaPath = XPathTools.SubRelativePath(_assetFolderPath,fullPath);

            _configDict = _configDict ?? new Dictionary<string, ConfigItem>();
            ConfigItem newItem = new ConfigItem();
            newItem.path = relaPath;

            _configDict[relaPath] = newItem;

        }

        private void RemoveCofig(string relaPath)
        {
            if (string.IsNullOrEmpty(relaPath))
                return;

            if (_configDict == null)
                return;

            ConfigItem item = null;
            if (_configDict.TryGetValue(relaPath, out item))
            {
                if (_selectedItem == item)
                {
                    _selectedItem = null;
                }
            }
            _configDict.Remove(relaPath);

        }

        private void RefreshAssetList()
        {
            _assetList.Clear();
            foreach(var path in _assetFolderList)
            {
                string key = XPathTools.SubRelativePath(_assetFolderPath,path);
                string fileName = Path.GetFileName(path);
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
                    if (fileName.IndexOf(searchKey) < 0)
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
            foreach (var item in _configDict.Values)
            {
                string key = item.path;
                string searchKey = _destSearchTextField.GetText();
                if (!string.IsNullOrEmpty(searchKey) && !string.IsNullOrEmpty(key))
                {
                    if (key.IndexOf(searchKey) < 0)
                    {
                        continue;
                    }
                }

                _configList.Add(item);
            }
        }
    }

}
