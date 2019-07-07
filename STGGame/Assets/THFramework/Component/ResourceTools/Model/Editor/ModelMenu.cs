using System.IO;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using THGame;
using UnityEditor.Animations;
using System.Text;

namespace THEditor
{
    public class ModelMenu
    {
        [MenuItem("Assets/Guangyv/模型菜单/整理组合最终生成", false, 12)]
        public static void MenuOneKeyAll()
        {
            var selected = Selection.activeObject;
            string selectPath = AssetDatabase.GetAssetPath(selected);
            if (selectPath != "")
            {
                string srcSelectPath = selectPath;
              
                //生成所有动画文件
                MenuGenModelAnimationClipAndController();

                //生成prefab
                MenuGenModelPrefabAndMaterials();

            }
            else
            {
                Debug.LogError("没有选中文件或文件夹");
            }

        }
        //生成模型源文件
        [MenuItem("Assets/Guangyv/模型菜单/模型/生成模型预制及材质")]
        public static void MenuGenModelPrefabAndMaterials()
        {
            var selected = Selection.activeObject;
            string selectPath = AssetDatabase.GetAssetPath(selected);
            if (selectPath != "")
            {
                if (Directory.Exists(selectPath))    //是文件夹
                {
                    selectPath = CombinePath(selectPath, string.Format("{0}", "skin.fbx")); 
                }
                ModelPostprocess.curPosModelAsset = null;
                string selectRootPath = Path.GetDirectoryName(selectPath);
                ModelTools.InitSkinFbxImporterFromFile(selectPath);    //初始化FBX
                GenModelPrefabAndMaterials(selectPath);

                ModelPostprocess.curPosModelAsset = "";
            }
            else
            {
                Debug.LogError("没有选中文件或文件夹");
            }
        }

        //生成动作文件
        [MenuItem("Assets/Guangyv/模型菜单/动作/生成动作文件及状态机")]
        public static void MenuGenModelAnimationClipAndController()
        {
            var selected = Selection.activeObject;
            string selectPath = AssetDatabase.GetAssetPath(selected);
            if (selectPath != "")
            {
                List<string> assetsList = new List<string>();
                if (Directory.Exists(selectPath))    //是文件夹
                {
                    //对所有操作
                    XFolderTools.TraverseFiles(selectPath, (fullPath) =>
                    {
                        string fileNameNExt = Path.GetFileNameWithoutExtension(fullPath).ToLower();
                        string fileExt = Path.GetExtension(fullPath).ToLower();
                        if (fileNameNExt.StartsWith("."))
                        {
                            return;
                        }

                        if (!fileExt.Contains("fbx"))
                        {
                            return;
                        }

                        if (("skin").Equals(fileNameNExt))
                        {
                            return;
                        }
                        string assetPath = XFileTools.GetFileRelativePath(fullPath);
                        assetsList.Add(assetPath);
                    });
                }
                else
                {
                    assetsList.Add(selectPath);
                }
                foreach (var assetPath in assetsList)
                {
                    ModelPostprocess.curPreAnimationAsset = null;

                    string selectFileName = Path.GetFileNameWithoutExtension(assetPath);
                    string modelType = GetModelType(assetPath);
                    bool isLoop = ModelConfig.IsNeedLoop(modelType, selectFileName);

                    ModelTools.InitAnimationFbxImporterFromFile(assetPath, isLoop);
                    GenModelAnimationClipAndController(assetPath);
                    ModelPostprocess.curPreAnimationAsset = "";
                }
            }
            else
            {
                Debug.LogError("没有选中文件或文件夹");
            }
        }

        [MenuItem("GameObject/Guangyv/辅助菜单/取得骨骼路径")]
        static void MenuGetBonePath()
        {
            if (Selection.activeGameObject != null)
            {
                GameObject GO = Selection.activeGameObject;
                Debug.Log(XMiscTools.GetGameObjectPath(GO));
            }
            else
            {
                Debug.LogError("没有选中对象");
            }
        }

        [MenuItem("GameObject/Guangyv/辅助菜单/取得所有挂点")]
        static void MenuGetAllHangNodePath()
        {
            if (Selection.activeGameObject != null)
            {
                GameObject modelGO = Selection.activeGameObject;
                if (modelGO)
                {
                    var meshNode = modelGO.transform.Find("mesh");
                    if (meshNode)
                    {
                        var smrComp = meshNode.GetComponent<SkinnedMeshRenderer>();
                        if (smrComp)
                        {
                            var bones = smrComp.bones;
                            Dictionary<string, bool> srBone = new Dictionary<string, bool>();
                            StringBuilder stringBuilder = new StringBuilder();
                            foreach (var bone in bones)
                            {
                                string path = XMiscTools.GetGameObjectPath(bone.gameObject, modelGO);
                                srBone.Add(path, true);
                            }
                            foreach (var node in modelGO.GetComponentsInChildren<Transform>())
                            {
                                string path = XMiscTools.GetGameObjectPath(node.gameObject, modelGO);
                                if (!srBone.ContainsKey(path))
                                {
                                    stringBuilder.Append(path);
                                    stringBuilder.Append("\r\n");
                                }
                            }
                            Debug.Log(stringBuilder.ToString());
                        }
                    }
                }

            }
            else
            {
                Debug.LogError("没有选中对象");
            }
        }
        ///////////////////////////////////////

        public static string CombinePath(string a1, string a2)
        {
            string path = Path.Combine(a1, a2);//在Win下用这个函数会把/换成\,
            path = path.Replace("\\", "/");
            return path;
        }

        //获取上层目录
        public static string GetPathPrevPath(string path)
        {
            path = path.Replace("\\", "/");
            int lastIndex = path.LastIndexOf("/", System.StringComparison.Ordinal);
            if (lastIndex >= 0 )
            {
                string lastPath = path.Substring(0, lastIndex);
                return lastPath;
            }
            else
            {
                return "/";
            }

        }
        //获取模型类型
        public static string GetModelType(string assetPath)
        {
            assetPath = assetPath.Replace("\\", "/");
            string[] pathList = assetPath.Split(new string[] { "/" }, System.StringSplitOptions.RemoveEmptyEntries);
            string modelFolder = pathList.Length > 2 ? pathList[2] : "";
            return modelFolder;
        }
        // 获取模型id，即下划线前面的那串数字
        public static string GetModelId(string assetPath)
        {
            assetPath = assetPath.Replace("\\", "/");
            string fileName = Path.GetFileNameWithoutExtension(assetPath);
            int indexOf_ = fileName.IndexOf('_');
            string modelId = (indexOf_ == -1) ? fileName : fileName.Remove(indexOf_);
            int iModelId;
            return !int.TryParse(modelId, out iModelId) ? "" : modelId;
        }


        public static GameObject GenModelPrefabAndMaterials(string assetPath)
        {
            string selectRootPath = Path.GetDirectoryName(assetPath);
            string selectFileName = Path.GetFileName(assetPath);
            string modelType = GetModelType(assetPath);

            string modelId = GetModelId(selectRootPath);
            if (modelId == "") modelId = "skin";
            string prefabSavePath = CombinePath(selectRootPath, string.Format("{0}.prefab", modelId));

            var materials = ModelTools.GenerateMaterialsFromFbxFile(assetPath);
            var outGO = ModelTools.GenerateFbxPrefabFromFbxFile(assetPath, prefabSavePath, (modelGO) =>
            {
                //1.outGO有问题,2.在OnPostprocessModel中无法用AssetDatabase.LoadAssetAtPath
                string modelPrefabPath = CombinePath(selectRootPath, string.Format("{0}.prefab", modelId));
                SetupAnimationController(selectRootPath, modelGO);

                ModelTools.SetupModelMaterials(modelGO, materials);

                if (ModelConfig.isNeedCollider(modelType))
                {
                    ModelTools.SetupModelBoxCollider(modelGO);
                }
            });

            return outGO;

        }

        public static AnimatorController GenModelAnimationClipAndController(string assetPath)
        {
            string selectRootPath = Path.GetDirectoryName(assetPath);
            string selectFileName = Path.GetFileNameWithoutExtension(assetPath);
            string modelType = GetModelType(assetPath);
            bool isDefault = ModelConfig.isDefaultState(modelType, selectFileName);

            bool isAddAll = false;
            string ctrlPath = CombinePath(selectRootPath, ModelTools.controllerName);
            string lastPath = GetPathPrevPath(selectRootPath); //没有就上层
            string lastCtrlPath = CombinePath(lastPath, ModelTools.controllerName);
            if (!XFileTools.Exists(ctrlPath) && XFileTools.Exists(lastCtrlPath))//如果当前目录没有,上层目录有,则需要把以前的复制回来
            {
                isAddAll = true;
            }

            //对单个
            string ctrlSavePath = CombinePath(selectRootPath, ModelTools.controllerName);
            var ctrl = ModelTools.GenerateAnimationControllerFromAnimationClipFile("", ctrlSavePath);

            var clip = ModelTools.GenerateAnimationClipFileFromFbxFile(assetPath);
            ModelTools.SetupAnimationState(ctrl, clip, isDefault);

            if (isAddAll)//把外层的全部添加回去
            {
                XFolderTools.TraverseFiles(lastPath, (fullPath) => {
                    string fileExt = Path.GetExtension(fullPath);
                    if (fileExt.Contains("anim"))
                    {
                        string prevAssetPath = XFileTools.GetFileRelativePath(fullPath);
                        string prevAssetName = Path.GetFileNameWithoutExtension(prevAssetPath);
                        var prevClip = AssetDatabase.LoadAssetAtPath<AnimationClip>(prevAssetPath);
                        bool isPrevDefault = ModelConfig.isDefaultState(modelType, prevAssetName);
                        ModelTools.SetupAnimationState(ctrl, prevClip, isPrevDefault);
                    }
                });

                //设置下
                string modelId = GetModelId(selectRootPath);
                string modelPrefabPath = CombinePath(selectRootPath, string.Format("{0}.prefab", modelId));
                GameObject modelGO = AssetDatabase.LoadAssetAtPath<GameObject>(modelPrefabPath);
                ModelTools.SetupAnimationController(modelGO, ctrl);                                          //强制设置
            }
            return ctrl;
        }

        public static void SetupAnimationController(string path, GameObject modelGO)
        {
            if (modelGO)
            {
                AnimatorController ctrl = null;
                string rootPath = path;
                string ctrlPath = CombinePath(rootPath, ModelTools.controllerName);
                if(!XFileTools.Exists(ctrlPath))//本层是否有
                {
                    string lastPath = GetPathPrevPath(rootPath); //没有就上层
                    ctrlPath = CombinePath(lastPath, ModelTools.controllerName);
                }
                ctrl = AssetDatabase.LoadAssetAtPath<AnimatorController>(ctrlPath);
                ModelTools.SetupAnimationController(modelGO, ctrl);
            }
        }
    }

}