using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;
using THGame;
namespace THEditor
{
    public static class SpriteEditorTools
    {
        public static readonly string controllerName = "controller.controller";
        public static readonly float frameRate = 12.0f;//统一12帧

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

        public static bool InitTextureImporterFromFile(string assetPath)
        {
            var importer = LoadImporterFromTextureFile(assetPath);
            var ret = SetupTextureImporter(importer);
            if (ret)
            {
                importer.SaveAndReimport();
            }

            return ret;
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
            //组(上层@下层)#动作名#序列
            private static bool GetSheetNameInfo(string ori, ref string groupName, ref string actionName, ref string idName)
            {
                string[] segStrs = ori.Split(new char[] { '#' });
                if (segStrs.Length < 3)
                    return false;

                //组名
                groupName = segStrs[0];
                //动作名
                StringBuilder stringbuilder = new StringBuilder();
                for (int i = 1; i < segStrs.Length - 1; i++)
                {
                    stringbuilder.Append(segStrs[i]);
                    stringbuilder.Append("#");
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
        public static Dictionary<string, Dictionary<string,AnimationClip>> GenerateAnimationClipFromTextureFile(string assetPath, string saveRootPath = "", System.Action<AnimationClip> callback = null)
        {
            string assetFileNonExtName = Path.GetFileNameWithoutExtension(assetPath);
            string assetRootPath = Path.GetDirectoryName(assetPath);
            TextureImporter importer = LoadImporterFromTextureFile(assetPath);
            if (importer)
            {

                //判断是否是精灵图集
                if (!(importer.textureType == TextureImporterType.Sprite && importer.spriteImportMode == SpriteImportMode.Multiple))
                    return null;

                //获取所有精灵帧
                Object[] sheetObjs = AssetDatabase.LoadAllAssetsAtPath(assetPath);
                var sheetDict = new Dictionary<string, Dictionary<string, SortedList<string, SpriteSheetFrameData>>>();
                foreach (var obj in sheetObjs)
                {
                    SpriteSheetFrameData metadata = SpriteSheetFrameData.TryCreate(obj as Sprite);
                    if (metadata != null)
                    {
                        string groupName = metadata.groupName;
                        Dictionary<string, SortedList<string, SpriteSheetFrameData>> actionMaps;
                        if (sheetDict.ContainsKey(groupName))
                        {
                            actionMaps = sheetDict[groupName];
                        }
                        else
                        {
                            actionMaps = new Dictionary<string, SortedList<string, SpriteSheetFrameData>>();
                            sheetDict.Add(groupName, actionMaps);
                        }
                        //
                        string actionName = metadata.actionName;
                        SortedList<string, SpriteSheetFrameData> frameList;
                        if (actionMaps.ContainsKey(actionName))
                        {
                            frameList = actionMaps[actionName];
                        }
                        else
                        {
                            frameList = new SortedList<string, SpriteSheetFrameData>();
                            actionMaps.Add(actionName, frameList);
                        }

                        string idName = metadata.idName;
                        if (frameList.ContainsKey(idName))
                        {
                            Debug.LogWarning(string.Format("{0}_{1}重复ID:{2}", groupName, actionName, idName));

                            metadata.idName = string.Format("{0}_{1}", metadata.idName, frameList.Count);
                            idName = metadata.idName;

                        }
                        frameList.Add(idName, metadata);
                    }
                }

                if (saveRootPath == "")
                {
                    saveRootPath = assetRootPath;
                }
                Dictionary<string, Dictionary<string, AnimationClip>> outMap = new Dictionary<string, Dictionary<string, AnimationClip>>();
                foreach (var groupPair in sheetDict)
                {
                    //保存资源
                    string groupName = groupPair.Key;
                    string[] subFolders = groupName.Split(new char[] { '@' });
                    string saveOutRootPath = saveRootPath;
                    for(int i = 0; i< subFolders.Length; i++)
                    {
                        saveOutRootPath = PathUtil.Combine(saveOutRootPath, subFolders[i]);
                        if (!XFolderTools.Exists(saveOutRootPath))
                        {
                            XFolderTools.CreateDirectory(saveOutRootPath);
                        }
                    }
                    
                    Dictionary<string, AnimationClip> outActionMap;
                    if (outMap.ContainsKey(groupName))
                    {
                        outActionMap = outMap[groupName];
                    }
                    else
                    {
                        outActionMap = new Dictionary<string, AnimationClip>();
                        outMap.Add(groupName, outActionMap);
                    }
                    
                    foreach (var actionPair in groupPair.Value)
                    {
                        //动画曲线
                        EditorCurveBinding curveBinding = new EditorCurveBinding();
                        curveBinding.type = typeof(SpriteRenderer);
                        curveBinding.path = "";
                        curveBinding.propertyName = "m_Sprite";
                        float frameTime = 1 / frameRate;                  
                        int index = 0;
                        List<ObjectReferenceKeyframe> keyFrames = new List<ObjectReferenceKeyframe>();
                        foreach (var listPair in actionPair.Value)
                        {
                            ObjectReferenceKeyframe keyFrame = new ObjectReferenceKeyframe();
                            keyFrame.time = (float)(frameTime * index);
                            keyFrame.value = listPair.Value.sprite;
                            keyFrames.Add(keyFrame);
                            index++;
                        }

                        AnimationClip clip = new AnimationClip();
                        clip.frameRate = frameRate;//动画帧率，30比较合适
#if !UNITY_5
                        AnimationUtility.SetAnimationType(clip, ModelImporterAnimationType.Generic);
#endif
                        AnimationUtility.SetObjectReferenceCurve(clip, curveBinding, keyFrames.ToArray());


                        //保存动画Clip
                        string outName = actionPair.Key;
                        string saveName = string.Format("{0}.anim", outName);
                        string savePath = Path.Combine(saveOutRootPath, saveName);

                        AssetDatabase.CreateAsset(clip, savePath);
                        AssetDatabase.SaveAssets();

                        outActionMap.Add(actionPair.Key, clip);
                        if (callback != null)
                        {
                            callback(clip);
                        }
                    }
                }
                return outMap;
            }
            return null;
        }
        public static AnimatorController GenerateAnimationControllerFromAnimationClipFile(string assetPath, string savePath = "", bool isDefault = false)
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

                    SetupAnimationState(ctrl, animClip, isDefault);
                }
            }

            AssetDatabase.SaveAssets(); //保存变更,不然没得内容
            return ctrl;
        }

        public static GameObject GeneratePrefabFromAnimationControllerFile(string assetPath, string savePath = "")
        {
            string assetFileNonExtName = Path.GetFileNameWithoutExtension(assetPath);
            string assetRootPath = Path.GetDirectoryName(assetPath);

            AnimatorController ctrl = AssetDatabase.LoadAssetAtPath<AnimatorController>(assetPath);
            if (ctrl)
            {
                GameObject spriteGO = new GameObject();

                var spriteRenderer = spriteGO.AddComponent<SpriteRenderer>();
                var animator = spriteGO.AddComponent<Animator>();

                animator.runtimeAnimatorController = ctrl;

                
                if (ctrl.animationClips.Length > 0)
                {
                    if (ctrl.layers.Length > 0)
                    {
                        var defaultState = ctrl.layers[0].stateMachine.defaultState;
                        var clip = defaultState.motion as AnimationClip;
                        if (clip)
                        {
                            var binds = AnimationUtility.GetObjectReferenceCurveBindings(clip);
                            if (binds.Length > 0)
                            {
                                var keyFrames = AnimationUtility.GetObjectReferenceCurve(clip, binds[0]);
                                if (keyFrames.Length > 0)
                                {
                                    spriteRenderer.sprite = keyFrames[0].value as Sprite;
                                }
                            }
                        }
                    }
                }
                

                if (savePath == "")
                {
                    string assetRootPathName = Path.GetFileNameWithoutExtension(assetRootPath);
                    savePath = Path.Combine(assetRootPath, string.Format("{0}.prefab", assetRootPathName));
                }
                
                GameObject outGO = PrefabUtility.SaveAsPrefabAsset(spriteGO,savePath);
                Object.DestroyImmediate(spriteGO);

                AssetDatabase.SaveAssets();
                return outGO;
            }

            return null;
        }

        public static Material GenerateMaterialFromAnimationControllerFile(string assetPath, string savePath = "")
        {
            string assetFileNonExtName = Path.GetFileNameWithoutExtension(assetPath);
            string assetRootPath = Path.GetDirectoryName(assetPath);

            if (savePath == "")
            {
                string assetRootPathName = Path.GetFileNameWithoutExtension(assetRootPath);
                savePath = Path.Combine(assetRootPath, string.Format("{0}.mat", assetRootPathName));
            }
            Material mat = new Material(ResourceConfig.GetInstance().defaultSpriteShader);
            if (XFileTools.Exists(savePath))
            {
                mat = AssetDatabase.LoadAssetAtPath<Material>(savePath);
            }
            else
            {
                AssetDatabase.CreateAsset(mat, savePath);
            }
            return mat;
        }

        [System.Serializable]
        class DBSheet
        {
            [System.Serializable]
            public class DBFrame
            {
                public string name;
                public int x;
                public int y;
                public int width;
                public int height;
            }
            public string name;
            public string imagePath;
            public int width;
            public int height;
            public List<DBFrame> SubTexture = new List<DBFrame>();
        }

        public static void GenerateDBJsonFromDBTextureFile(string assetPath, string savePath = "")
        {
            string assetFileNonExtName = Path.GetFileNameWithoutExtension(assetPath);
            string assetRootPath = Path.GetDirectoryName(assetPath);
            TextureImporter importer = LoadImporterFromTextureFile(assetPath);
            if (importer)
            {
                //判断是否是精灵图集
                if (!(importer.textureType == TextureImporterType.Sprite && importer.spriteImportMode == SpriteImportMode.Multiple))
                    return ;

                Texture texture = AssetDatabase.LoadAssetAtPath<Texture>(assetPath);

                DBSheet sheet = new DBSheet();
                sheet.name = assetFileNonExtName;
                sheet.imagePath = Path.GetFileName(assetPath);
                sheet.width = texture.width;
                sheet.height = texture.height;


                //获取所有精灵帧
                Object[] sheetObjs = AssetDatabase.LoadAllAssetsAtPath(assetPath);
                for(int i = 1; i < sheetObjs.Length; i++)   //第一张不是精灵帧
                {
                    Sprite sprite = sheetObjs[i] as Sprite;
                    Rect rect = sprite.rect;
                    DBSheet.DBFrame frame = new DBSheet.DBFrame();
                    frame.name = sprite.name;
                    frame.width = (int)rect.width;
                    frame.height = (int)rect.height;
                    frame.x = (int)rect.x;
                    frame.y = (int)(sheet.height - frame.height - rect.y);

                    sheet.SubTexture.Add(frame);
                }

                var jsonStr = JsonUtility.ToJson(sheet);

                if (savePath == "")
                {
                    savePath = PathUtil.Combine(assetRootPath, string.Format("{0}.json", assetFileNonExtName));
                }
                File.WriteAllText(savePath, jsonStr, Encoding.UTF8);
                AssetDatabase.Refresh();
            }

        }

        /// <summary>
        /// 从对应的json文件生成精灵帧(DragonBones 5.5)
        /// </summary>
        /// <param name="assetPath">图集路径</param>
        public static void SetupSpriteFrameFromDBJsonFile(string assetPath)
        {
            string assetFileNonExtName = Path.GetFileNameWithoutExtension(assetPath);
            string assetRootPath = Path.GetDirectoryName(assetPath);

            //查看是否有对应的json文件
           
            string jsonFilePath = PathUtil.Combine(assetRootPath, string.Format("{0}.json", assetFileNonExtName));
            
            if (!XFileTools.Exists(jsonFilePath))
            {
                Debug.LogWarning("找不到DragonBones 5.5图集Json文件");
                return;
            }
            TextureImporter importer = LoadImporterFromTextureFile(assetPath);
            if (importer)
            {
                var jsonFile = AssetDatabase.LoadAssetAtPath(jsonFilePath, typeof(TextAsset)) as TextAsset;
                if (jsonFile == null)
                    return;

                var atlas = JsonUtility.FromJson<DBSheet>(jsonFile.text);
                if (atlas != null)
                {
                    if (atlas.SubTexture.Count > 0)
                    {
                        List<SpriteMetaData> spriteDataList = new List<SpriteMetaData>();
                        foreach (var frameData in atlas.SubTexture)
                        {
                            SpriteMetaData md = new SpriteMetaData();

                            int width = frameData.width;
                            int height = frameData.height;
                            int x = frameData.x;
                            int y = atlas.height - height - frameData.y;//TexturePacker以左上为原点，Unity以左下为原点

                            md.rect = new Rect(x, y, width, height);
                            md.pivot = md.rect.center;
                            md.name = frameData.name;

                            spriteDataList.Add(md);
                        }
                        importer.textureType = TextureImporterType.Sprite;          //设置为精灵图集
                        importer.spriteImportMode = SpriteImportMode.Multiple;      //设置为多个
                        importer.spritesheet = spriteDataList.ToArray();
                        importer.SaveAndReimport();
                    }
                    Debug.Log(string.Format("图集:{0},共生成{1}帧", atlas.name, atlas.SubTexture.Count));
                }
               
            }
        }

        /////////////////////////Object设置///////////////////////////
        public static bool SetupTextureImporter(TextureImporter textureImporter)
        {
            if (textureImporter == null)
                return false;

            textureImporter.textureType = TextureImporterType.Sprite;           //精灵
            textureImporter.spriteImportMode = SpriteImportMode.Multiple;       //图集

            return true;
        }
        //设置状态
        public static bool SetupAnimationState(AnimatorController ctrl, AnimationClip clip, bool isDefault = false)
        {
            if (ctrl && clip)
            {
                //是否存在,没有就添加,有就覆盖
                var stateMachine = ctrl.layers[0].stateMachine;
                foreach (var stateInfo in stateMachine.states)
                {
                    if (stateInfo.state.name == clip.name)
                    {
                        stateInfo.state.motion = clip;
                        if (isDefault)                      //默认状态也改下
                        {
                            stateMachine.defaultState = stateInfo.state;
                        }
                        AssetDatabase.SaveAssets(); //保存变更
                        return true;
                    }
                }
                var state = ctrl.AddMotion(clip);
                if (isDefault)
                {
                    stateMachine.defaultState = state;
                }
                AssetDatabase.SaveAssets(); //保存变更
                return true;
            }
            return false;
        }

        public static bool SetupAnimationClipLoop(AnimationClip clip, bool isLoop)
        {
            if (clip)
            {
                AnimationClipSettings clipSetting = AnimationUtility.GetAnimationClipSettings(clip);
                clipSetting.loopTime = isLoop;
                AnimationUtility.SetAnimationClipSettings(clip, clipSetting);
                return true;
            }
            return false;
        }
        public static bool SetupBoxCollider(GameObject Go)
        {
            if (Go)
            {
                if(!Go.GetComponent<BoxCollider2D>())
                {
                    var box = Go.AddComponent<BoxCollider2D>();
                    box.isTrigger = true;
                    return true;
                }
            }
            return false;
        }

        public static bool SetupMaterial(GameObject Go,Material material)
        {
            if (Go && material)
            {
                var spriteRenderer = Go.GetComponent<SpriteRenderer>();
                if(spriteRenderer)
                {
                    spriteRenderer.material = material;
                }
                return true;
            }
            return false;
        }

        ////
        ///

        public static string GroupName2Path(string groupName)
        {
            string newPath = groupName.Replace("@", "/");
            return newPath;
        }
    }

}
