using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;
using THGame;
namespace THEditor
{
    public static class SpriteTools
    {
        public static readonly float frameRate = 12.0f;

        ///
        public static TextureImporter LoadImporterFromTextureFile(string assetPath)
        {
            TextureImporter textureImporter = AssetImporter.GetAtPath(assetPath) as TextureImporter;
            if (!textureImporter)
            {
                Debug.LogError(string.Format("{0}不是一张图片文件", Path.GetFileName(assetPath)));
                return null;
            }

            return textureImporter;
        }


        //精灵数据
        class SpriteSheetFrameData
        {
            public string groupName = "";
            public string actionName = "";
            public string idName = "";
            public Sprite sprite;

            public string GetOutName()
            {
                return groupName + "_" + actionName;
            }

            public static SpriteSheetFrameData TryCreate(Sprite sprite)
            {
                if (sprite)
                {
                    string sheetName = sprite.name;
                    string groupName = "";
                    string actionName = "";
                    string idName = "";
                    bool ret = GetSheetNameInfo(sheetName, ref groupName, ref actionName, ref idName);
                    if (ret)
                    {
                        SpriteSheetFrameData metadata = new SpriteSheetFrameData();
                        metadata.groupName = groupName;
                        metadata.actionName = actionName;
                        metadata.idName = idName;
                        metadata.sprite = sprite;
                        return metadata;
                    }
                }
                return null;
            }

            private static bool GetSheetNameInfo(string ori, ref string groupName, ref string actionName, ref string idName)
            {
                string[] segStrs = ori.Split(new char[] { '_' });
                if (segStrs.Length < 3)
                    return false;

                //组名
                groupName = segStrs[0];
                //动作名
                StringBuilder stringbuilder = new StringBuilder();
                for (int i = 1; i < segStrs.Length - 1; i++)
                {
                    stringbuilder.Append(segStrs[i]);
                    stringbuilder.Append("_");
                }
                stringbuilder.Remove(stringbuilder.Length - 1, 1);
                actionName = stringbuilder.ToString();
                //id名
                int id = -1;
                idName = segStrs[segStrs.Length - 1];
                bool ret = int.TryParse(idName, out id);
                idName = ret ? idName : "";

                if (!ret)
                    return false;

                return true;
            }
        }

        /// <summary>
        /// 生成动画及控制器
        /// sprite子项命名规则:组_动作名_编号,没有编号的一律不生成Anima
        /// </summary>
        /// <param name="assetPath"></param>
        public static void GenerateAnimationClipFromTextureFile(string assetPath, string saveRootPath = "", System.Action<AnimationClip> callback = null)
        {
            string assetFileNonExtName = Path.GetFileNameWithoutExtension(assetPath);
            string assetRootPath = Path.GetDirectoryName(assetPath);
            TextureImporter importer = LoadImporterFromTextureFile(assetPath);
            if (importer)
            {

                //判断是否是精灵图集
                if (!(importer.textureType == TextureImporterType.Sprite && importer.spriteImportMode == SpriteImportMode.Multiple))
                    return;

                //获取所有精灵帧
                Object[] sheetObjs = AssetDatabase.LoadAllAssetsAtPath(assetPath);
                Dictionary<string, SortedList<string, SpriteSheetFrameData>> dict = new Dictionary<string, SortedList<string, SpriteSheetFrameData>>();
                foreach (var obj in sheetObjs)
                {
                    string sheetName = obj.name;
                    SpriteSheetFrameData metadata = SpriteSheetFrameData.TryCreate(obj as Sprite);
                    if (metadata != null)
                    {
                        string fileName = metadata.GetOutName();
                        SortedList<string, SpriteSheetFrameData> sortlist = null;
                        if (dict.ContainsKey(fileName))
                        {
                            sortlist = dict[fileName];
                        }
                        else
                        {
                            sortlist = new SortedList<string, SpriteSheetFrameData>();
                            dict.Add(fileName, sortlist);
                        }
                        sortlist.Add(metadata.idName, metadata);

                    }
                }
                //TODO:没有动画帧???
                foreach(var sheetPair in dict)
                {
                    EditorCurveBinding curveBinding = new EditorCurveBinding();
                    curveBinding.type = typeof(SpriteRenderer);
                    curveBinding.path = "";
                    curveBinding.propertyName = "m_Sprite";
                    float frameTime = 1 / 12f;
                    int index = 0;
                    List<ObjectReferenceKeyframe> keyFrames = new List<ObjectReferenceKeyframe>();
                    foreach (var sheetData in sheetPair.Value)
                    {
                        ObjectReferenceKeyframe keyFrame = new ObjectReferenceKeyframe();
                        keyFrame.time = frameTime * index;
                        keyFrame.value = sheetData.Value.sprite;
                        index++;
                    }

                    AnimationClip clip = new AnimationClip();
                    clip.frameRate = 30;//动画帧率，30比较合适
                    //AnimationUtility.SetAnimationType(clip, ModelImporterAnimationType.Generic);
                    AnimationUtility.SetObjectReferenceCurve(clip, curveBinding, keyFrames.ToArray());
                    
                    //保存资源
                    if (saveRootPath == "")
                    {
                        saveRootPath = assetRootPath;
                    }
                    string outName = sheetPair.Key;
                    string saveName = string.Format("{0}.anim", outName);
                    string savePath = Path.Combine(saveRootPath, saveName);

                    AssetDatabase.CreateAsset(clip, savePath);
                    AssetDatabase.SaveAssets();

                    if (callback != null)
                    {
                        callback(clip);
                    }
                }
            }
        }
        public static AnimatorController GenerateAnimationControllerFromAnimationClipFile(string assetPath, string savePath, bool isDefault = false)
        {
            //没有就创建,有就添加
            AnimatorController ctrl = null;
            if (!XFileTools.Exists(savePath))
            {
                //不存在就创建
                ctrl = AnimatorController.CreateAnimatorControllerAtPath(savePath);
            }
            else
            {
                ctrl = AssetDatabase.LoadAssetAtPath<AnimatorController>(savePath);
            }
            if (assetPath != "")
            {
                AnimationClip animClip = AssetDatabase.LoadAssetAtPath<AnimationClip>(assetPath);
                if (animClip)
                {
                    string assetFileNonExtName = Path.GetFileNameWithoutExtension(assetPath);
                    string assetRootPath = Path.GetDirectoryName(assetPath);

                    //SetupAnimationState(ctrl, animClip, isDefault);
                }
            }

            AssetDatabase.SaveAssets(); //保存变更,不然没得内容
            return ctrl;
        }

        public static void GenerateSpriteFromJsonFile(string assetPath)
        {

        }
    }

}
