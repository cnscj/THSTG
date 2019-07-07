using UnityEngine;
using UnityEditor;
using System.IO;
using THGame;

namespace THEditor
{
    public class ModelPostprocess : AssetPostprocessor
    {
        public static string curPreAnimationAsset = "";
        public static string curPosModelAsset = "";
        //注意:SaveAndReimport函数会重新触发回调,造成死循环,因此避免在回调中调用此函数
        //之前
        public void OnPreprocessModel()
        {
            
        }

        public void OnPreprocessAnimation()
        {
            return;
            if (!IsModelPath(assetPath))
                return;

            string fileName = Path.GetFileNameWithoutExtension(assetPath).ToLower();
            if (!"skin".Equals(fileName))
            {
                if (curPreAnimationAsset == null)
                {
                    //curPreAnimationAsset = assetPath;
                    return;
                }
                else if (curPreAnimationAsset == "")
                {
                    curPreAnimationAsset = assetPath;
                    string selectRootPath = Path.GetDirectoryName(assetPath);
                    string selectFileName = Path.GetFileNameWithoutExtension(assetPath);
                    string modelType = ModelMenu.GetModelType(assetPath);
                    bool isDefault = ModelConfig.isDefaultState(modelType, selectFileName);
                    var import = ModelTools.LoadImporterFromFbxFile(assetPath);
                    ModelTools.SetupAnimationFbxImporter(import, isDefault);
                    import.SaveAndReimport();
                    return;
                }
                else if (curPreAnimationAsset == assetPath)
                {
                    curPreAnimationAsset = "";
                }
                else
                {
                    return;
                }
                //////
                ModelMenu.GenModelAnimationClipAndController(assetPath);
            }
        }

        //之后
        public void OnPostprocessModel(GameObject go)
        {
            return;
            //OnPostprocessModel回调好像会执行多次,
            if (!IsModelPath(assetPath))
                return;

            string fileName = Path.GetFileNameWithoutExtension(assetPath).ToLower();
            if ("skin".Equals(fileName))
            {
                if (curPosModelAsset == null)
                {
                    //curPosModelAsset = assetPath;
                    return;
                }
                else if (curPosModelAsset == "")
                {
                    curPosModelAsset = assetPath;
                    var import = ModelTools.LoadImporterFromFbxFile(assetPath);
                    ModelTools.SetupSkinFbxImporter(import);
                    import.SaveAndReimport();
                    return;
                }
                else if (curPosModelAsset == assetPath)
                {
                    curPosModelAsset = "";
                }
                else
                {
                    return;
                }

                ////

                ModelMenu.GenModelPrefabAndMaterials(assetPath);
               
            }
        }

        public void OnPostprocessAnimation(GameObject go, AnimationClip clip)
        {
            
        }

        ///////////////////
        public bool IsModelPath(string assetPath)
        {
            assetPath = assetPath.Replace("\\", "/");
            return assetPath.Contains("Assets/Models");
        }

    }
}