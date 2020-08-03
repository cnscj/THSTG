using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;
using XLibrary;

namespace ASEditor
{
    public static class SpriteEditorTools
    {
        public static readonly string controllerName = "controller.controller";                 //共用/私有控制器
        public static readonly string controllerTmplName = "controllerTmpl.controller";         //模板控制器
        public static readonly string overrideControllerName = "overrideController.overrideController";         //模板控制器

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
        public static Dictionary<string, Dictionary<string, AnimationClip>> GenerateAnimationClipFromTextureFile(string assetPath, string saveRootPath = "", System.Action<AnimationClip> callback = null)
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
                    for (int i = 0; i < subFolders.Length; i++)
                    {
                        saveOutRootPath = XPathTools.Combine(saveOutRootPath, subFolders[i]);
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
                        //保存动画Clip
                        string outName = actionPair.Key;
                        string saveName = string.Format("{0}.anim", outName);
                        string savePath = Path.Combine(saveOutRootPath, saveName);

                        List<Sprite> spriteList = new List<Sprite>();
                        foreach (var listPair in actionPair.Value)
                        {
                            spriteList.Add(listPair.Value.sprite);
                        }
                        AnimationClip clip = MakeAnimationClip(spriteList.ToArray(), SpriteToolsConfig.GetInstance().defaultFrameRate, savePath);

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
            AssetDatabase.Refresh();
            return ctrl;
        }

        public static AnimatorOverrideController GenerateAnimationOverrideControllerFromAnimationClipFile(string assetPath, string ctrlTmplPath, string savePath = "")
        {
            //没有就创建,有就添加
            AnimatorOverrideController ovrrideCtrl = null;
            if (XFileTools.Exists(ctrlTmplPath))
            {
                AnimatorController ctrlTmpl = AssetDatabase.LoadAssetAtPath<AnimatorController>(ctrlTmplPath);
                if (ctrlTmpl != null)
                {
                    if (!XFileTools.Exists(savePath))
                    {
                        //不存在就创建
                        ovrrideCtrl = new AnimatorOverrideController();
                        AssetDatabase.CreateAsset(ovrrideCtrl, savePath);
                    }
                    else
                    {
                        ovrrideCtrl = AssetDatabase.LoadAssetAtPath<AnimatorOverrideController>(savePath);
                    }
                    //赋予模板
                    ovrrideCtrl.runtimeAnimatorController = ovrrideCtrl.runtimeAnimatorController != null ? ovrrideCtrl.runtimeAnimatorController : ctrlTmpl;

                    if (assetPath != "")
                    {
                        AnimationClip animClip = AssetDatabase.LoadAssetAtPath<AnimationClip>(assetPath);
                        if (animClip)
                        {
                            SetupOverrideMotion(ovrrideCtrl, animClip);
                        }
                    }

                    AssetDatabase.SaveAssets(); //保存变更,不然没得内容
                }
            }
            AssetDatabase.Refresh();
            return ovrrideCtrl;
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

                GameObject outGO = PrefabUtility.SaveAsPrefabAsset(spriteGO, savePath);
                Object.DestroyImmediate(spriteGO);

                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
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
            Material mat = new Material(SpriteToolsConfig.GetInstance().defaultShader);
            if (XFileTools.Exists(savePath))
            {
                mat = AssetDatabase.LoadAssetAtPath<Material>(savePath);
            }
            else
            {
                AssetDatabase.CreateAsset(mat, savePath);
            }
            AssetDatabase.Refresh();
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
                    return;

                Texture texture = AssetDatabase.LoadAssetAtPath<Texture>(assetPath);

                DBSheet sheet = new DBSheet();
                sheet.name = assetFileNonExtName;
                sheet.imagePath = Path.GetFileName(assetPath);
                sheet.width = texture.width;
                sheet.height = texture.height;


                //获取所有精灵帧
                Object[] sheetObjs = AssetDatabase.LoadAllAssetsAtPath(assetPath);
                for (int i = 1; i < sheetObjs.Length; i++)   //第一张不是精灵帧
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

                var jsonStr = JsonUtility.ToJson(sheet,true);

                if (savePath == "")
                {
                    savePath = XPathTools.Combine(assetRootPath, string.Format("{0}.json", assetFileNonExtName));
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

            string jsonFilePath = XPathTools.Combine(assetRootPath, string.Format("{0}.json", assetFileNonExtName));

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
        //
        public static bool SetupOverrideMotion(AnimatorOverrideController overrideCtrl, AnimationClip clip)
        {
            if (overrideCtrl && clip)
            {
                if (overrideCtrl[clip.name])
                {
                    overrideCtrl[clip.name] = clip;
                    return true;
                }
            }
            return false;
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
                if (!Go.GetComponent<BoxCollider2D>())
                {
                    var box = Go.AddComponent<BoxCollider2D>();
                    box.isTrigger = true;
                    return true;
                }
            }
            return false;
        }

        public static bool SetupMaterial(GameObject Go, Material material)
        {
            if (Go && material)
            {
                var spriteRenderer = Go.GetComponent<SpriteRenderer>();
                if (spriteRenderer)
                {
                    spriteRenderer.material = material;
                }
                return true;
            }
            return false;
        }

        ////
        public static void TexturePackage(Texture2D [] textures, string savePath)
        {
            if (textures != null && textures.Length > 0)
            {
                Dictionary<string, bool> importDict = new Dictionary<string, bool>();
                //需要设置Import属性
                {
                    foreach(var tex in textures)
                    {
                        var texPath = AssetDatabase.GetAssetPath(tex);
                        if (!string.IsNullOrEmpty(texPath))
                        {
                            var texImport = AssetImporter.GetAtPath(texPath) as TextureImporter;
                            importDict[texPath] = texImport.isReadable;

                            texImport.isReadable = true;
                            texImport.textureCompression = TextureImporterCompression.Uncompressed; //不压缩ARGB32
                            texImport.SaveAndReimport();
                        }
                    }

                }

                //XXX:计算所有纹理大小,选择最接近2次幂打包
                //如果太大,分页
                Texture2D altas = new Texture2D(2048, 2048);
                Rect[] rects = altas.PackTextures(textures, 0, 2048);   //这里输出UV坐标,要转换

                if (rects != null && rects.Length > 0)
                {
                    byte[] buffer = altas.EncodeToPNG();
                    File.WriteAllBytes(savePath, buffer);
                    AssetDatabase.Refresh();
                
                    //设置图集sprite
                    {
                        var altasImport = AssetImporter.GetAtPath(savePath) as TextureImporter;
                        altasImport.textureType = TextureImporterType.Sprite;
                        altasImport.spriteImportMode = SpriteImportMode.Multiple;

                        var altasTex = AssetDatabase.LoadAssetAtPath<Texture2D>(savePath);
                        List<SpriteMetaData> spriteDataList = new List<SpriteMetaData>();
                        for(int i = 0 ;i < rects.Length; i++)
                        {
                            Rect rect = rects[i];
                            Texture2D texture2D = textures[i];
                            var texPath = AssetDatabase.GetAssetPath(texture2D);
                            var texImport = AssetImporter.GetAtPath(texPath) as TextureImporter;

                            SpriteMetaData md = new SpriteMetaData();

                            int width = (int)(rect.width * altasTex.width);
                            int height = (int)(rect.height * altasTex.height);
                            int x = (int)(rect.x * altasTex.width);
                            int y = (int)(rect.y * altasTex.height);

                            md.rect = new Rect(x, y, width, height);
                            md.pivot = texImport != null ? texImport.spritePivot : md.rect.center;
                            md.name = texture2D != null ? texture2D.name : string.Format("sprite_{0}",i);

                            spriteDataList.Add(md);
                        }


                        altasImport.spritesheet = spriteDataList.ToArray();
                        altasImport.SaveAndReimport();

                    }
                }

                //还原设置Import属性
                foreach (var tex in textures)
                {
                    var texPath = AssetDatabase.GetAssetPath(tex);
                    if (!string.IsNullOrEmpty(texPath))
                    {
                        var texImport = AssetImporter.GetAtPath(texPath) as TextureImporter;
                        texImport.isReadable = importDict[texPath];
                        texImport.SaveAndReimport();
                    }
                }

                Debug.LogFormat("Make Altas Success : {0}", savePath);
            }
        }


        public static void TextureUnpackage(Texture2D altsTexture,string saveFolder = null)
        {
            if (altsTexture == null)
                return;

            string texturePath = AssetDatabase.GetAssetPath(altsTexture);
            string textureRootPath = Path.GetDirectoryName(texturePath);
            saveFolder = string.IsNullOrEmpty(saveFolder) ? textureRootPath : saveFolder;
            var altasImport = AssetImporter.GetAtPath(texturePath) as TextureImporter;

            if (altasImport.spriteImportMode == SpriteImportMode.Multiple)
            {
                var oldReadable = altasImport.isReadable;
                altasImport.isReadable = true;
                altasImport.SaveAndReimport();

                var spritesheets = altasImport.spritesheet;
                foreach(var metaData in spritesheets)
                {
                    var spriteRect = metaData.rect;
                    var targetTex = new Texture2D((int)spriteRect.width, (int)spriteRect.height);
                    var pixels = altsTexture.GetPixels(
                        (int)spriteRect.x,
                        (int)spriteRect.y,
                        (int)spriteRect.width,
                        (int)spriteRect.height);
                    targetTex.SetPixels(pixels);
                    targetTex.Apply();

                    string savePath = Path.Combine(saveFolder, string.Format("{0}.png", metaData.name));
                    byte[] buffer = targetTex.EncodeToPNG();
                    File.WriteAllBytes(savePath, buffer);

                    AssetDatabase.Refresh();

                    var spriteTextureImport = AssetImporter.GetAtPath(savePath) as TextureImporter;
                    spriteTextureImport.textureType = TextureImporterType.Sprite;
                    spriteTextureImport.SaveAndReimport();
                }

                altasImport.isReadable = oldReadable;
                altasImport.SaveAndReimport();

                Debug.LogFormat("Make Sprite Success : {0}", saveFolder);
            }


        }

        public static AnimationClip MakeAnimationClip(Object []keyframes, float frameRate = 12.0f , string savePath = null)
        {
            //动画曲线
            EditorCurveBinding curveBinding = new EditorCurveBinding();
            curveBinding.type = typeof(SpriteRenderer);
            curveBinding.path = "";
            curveBinding.propertyName = "m_Sprite";
            float frameTime = 1 / frameRate;
            int index = 0;
            List<ObjectReferenceKeyframe> keyFrames = new List<ObjectReferenceKeyframe>();
            foreach (var keyframe in keyframes)
            {
                ObjectReferenceKeyframe keyFrame = new ObjectReferenceKeyframe();
                keyFrame.time = (float)(frameTime * index);
                keyFrame.value = keyframe;
                keyFrames.Add(keyFrame);
                index++;
            }

            AnimationClip clip = new AnimationClip();
            clip.frameRate = SpriteToolsConfig.GetInstance().defaultFrameRate;//动画帧率，30比较合适
#if !UNITY_5
            AnimationUtility.SetAnimationType(clip, ModelImporterAnimationType.Generic);
#endif
            AnimationUtility.SetObjectReferenceCurve(clip, curveBinding, keyFrames.ToArray());


            if (!string.IsNullOrEmpty(savePath))
            {
                //保存动画Clip
                AssetDatabase.CreateAsset(clip, savePath);
                AssetDatabase.SaveAssets();
            }

            return clip;
        }

        ///

        public static string GroupName2Path(string groupName)
        {
            string newPath = groupName.Replace("@", "/");
            return newPath;
        }
    }

}
