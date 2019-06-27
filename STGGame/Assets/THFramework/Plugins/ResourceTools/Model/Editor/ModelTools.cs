using UnityEngine;
using UnityEditor;
using System.IO;
using THGame;
using System.Text;
using System.Collections.Generic;
using UnityEditor.Animations;

namespace THEditor
{
    public class ModelTools
    {
        public static readonly string controllerName = "controller.controller";

        /////////////////////////FBX文件设置///////////////////////////
        public static ModelImporter LoadImporterFromFbxFile(string assetPath)
        {
            ModelImporter modelImporter = AssetImporter.GetAtPath(assetPath) as ModelImporter;
            if (!modelImporter)
            {
                Debug.LogError(string.Format("{0}不是一个标准的FBX文件", Path.GetFileName(assetPath)));
                return null;
            }

            return modelImporter;
        }

        public static bool InitSkinFbxImporterFromFile(string assetPath)
        {
            var importer = LoadImporterFromFbxFile(assetPath);
            var ret = SetupSkinFbxImporter(importer);
            importer.SaveAndReimport();
            return ret;
        }

        public static bool InitAnimationFbxImporterFromFile(string assetPath,bool isLoop = false)
        {
            var importer = LoadImporterFromFbxFile(assetPath);
            var ret = SetupAnimationFbxImporter(importer, isLoop);
            importer.SaveAndReimport();
            return ret;
        }
        /////////////////////////生成文件设置///////////////////////////
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

                    SetupAnimationState(ctrl, animClip, isDefault);
                }
            }
            
            AssetDatabase.SaveAssets(); //保存变更,不然没得内容
            return ctrl;
        }

        //生成Skin.FBX的Prefab文件
        public static GameObject GenerateFbxPrefabFromFbxFile(string assetPath, string savePath = "", System.Action<GameObject> callback = null)
        {
            string assetFileNonExtName = Path.GetFileNameWithoutExtension(assetPath);
            string assetRootPath = Path.GetDirectoryName(assetPath);

            GameObject modelGO = AssetDatabase.LoadAssetAtPath<GameObject>(assetPath);
            GameObject newModelGO = Object.Instantiate(modelGO);

            //标准化,可以放到预处理那边做

            //额外操作
            if (callback != null)
            {
                callback(newModelGO);
            }

            //保存成预制体
            if (savePath == "")
            {
                string saveName = string.Format("{0}.prefab", assetFileNonExtName);
                savePath = Path.Combine(assetRootPath, saveName);
            }
            var outGo = PrefabUtility.SaveAsPrefabAsset(newModelGO, savePath);
            Object.DestroyImmediate(newModelGO);
            AssetDatabase.Refresh();
            
            return outGo;
        }

        //生成材质文件
        public static List<List<Material>> GenerateMaterialsFromFbxFile(string assetPath, string saveRootPath = "", System.Action<List<List<Material>>> callback = null)
        {
            string assetFileNonExtName = Path.GetFileNameWithoutExtension(assetPath);
            string assetRootPath = Path.GetDirectoryName(assetPath);
            if (saveRootPath == "")
            {
                saveRootPath = assetRootPath;
            }
            List<List<Material>> matsList = null;
           GameObject modelGO = AssetDatabase.LoadAssetAtPath<GameObject>(assetPath);
            if (modelGO)
            {
                //GameObject实例
                GameObject newPrefabGO = modelGO;

                //渲染组件
                matsList = new List<List<Material>>();
                foreach (var skm in newPrefabGO.GetComponentsInChildren<Renderer>())
                {
                    List<Material> mats = new List<Material>();
                    foreach (var materia in skm.sharedMaterials)
                    {
                        string matName = materia.name;
                        Texture mainTex = materia.mainTexture as Texture;                                                     //这里可能存在找不到贴图的情况
                        if(mainTex == null)
                        {
                            XFolderTools.TraverseFiles(assetRootPath, (fullPath) =>
                            {
                                if (mainTex != null) return;
                                string fileExt = Path.GetExtension(fullPath).ToLower();

                                if (fileExt.Contains("jpg") ||
                                    fileExt.Contains("tga") ||
                                    fileExt.Contains("png"))
                                {
                                    string texFile = XFileTools.GetFileRelativePath(fullPath);
                                    mainTex = AssetDatabase.LoadAssetAtPath<Texture>(texFile);
                                }
                            });
                        }
                        //Material mat = new Material(Shader.Find("GY/ModelBase"));                                             //默认材质,注意,这里有可能Shader没有加载
                        Material mat = new Material(Shader.Find("Standard"));
                        mat.mainTexture = mainTex;

                        if (matName.Contains("_tm"))                                                                          //这种是透贴(透明贴图)
                        {
                            //ModelBaseShaderGUI.SetupMaterialWithBlendMode(mat, ModelBaseShaderGUI.BlendMode.Transparent);     //设置函数因Shader而异
                        }
                        else
                        {
                            //如果贴图没有alpha就设置为Opaque
                            //否则为Cutout
                            string texPath = AssetDatabase.GetAssetPath(mainTex);
                            string fileExt = Path.GetExtension(texPath).ToLower();

                            if (fileExt.Contains("jpg"))
                            {
                                //ModelBaseShaderGUI.SetupMaterialWithBlendMode(mat, ModelBaseShaderGUI.BlendMode.Opaque);
                            }
                            else
                            {
                                //ModelBaseShaderGUI.SetupMaterialWithBlendMode(mat, ModelBaseShaderGUI.BlendMode.Cutout);
                            }
                        }

                        string matPath = Path.Combine(saveRootPath, string.Format("{0}.mat", matName));                        //沿用源材质名
                        AssetDatabase.CreateAsset(mat, matPath);
                        mats.Add(mat);
                    }
                    matsList.Add(mats);
                }
                if (callback != null)
                {
                    callback(matsList);
                }

            }
            return matsList;
        }

        //生成FBX动画文件文件
        public static AnimationClip GenerateAnimationClipFileFromFbxFile(string assetPath, string savePath = "", System.Action<AnimationClip> callback = null)
        {
            string assetFileNonExtName = Path.GetFileNameWithoutExtension(assetPath);
            string assetRootPath = Path.GetDirectoryName(assetPath);
            ModelImporter modelImporter = LoadImporterFromFbxFile(assetPath);
            AnimationClip outClip = null;
            if (modelImporter)
            {
                //查找动画
                Object[] objects = AssetDatabase.LoadAllAssetsAtPath(assetPath);
                foreach (var obj in objects)
                {
                    AnimationClip clip = obj as AnimationClip;
                    if (clip != null && clip.name.IndexOf("__preview__", System.StringComparison.Ordinal) == -1)
                    {
                        //拷贝一份
                        AnimationClip newClip = Object.Instantiate(clip);
                        
                        //设置循环
                        AnimationClipSettings clipSetting = AnimationUtility.GetAnimationClipSettings(newClip);
                        var chilImport = modelImporter.clipAnimations.Length > 0 ? modelImporter.clipAnimations[0] : null;
                        if (chilImport != null)
                        {
                            clipSetting.loopTime = chilImport.loopTime;
                            AnimationUtility.SetAnimationClipSettings(newClip, clipSetting);
                        }

                        //压缩精简
                        RidAnimationScale(newClip);
                        CompressionPrecision(newClip);

                        //保存资源
                        if (savePath == "")
                        {
                            string saveName = string.Format("{0}.anim", assetFileNonExtName);
                            savePath = Path.Combine(assetRootPath, saveName);
                        }
                        AssetDatabase.CreateAsset(newClip, savePath);
                        AssetDatabase.Refresh();


                        outClip = newClip;
                        break;                                  
                    }
                }
            }
            if (callback != null)
            {
                callback(outClip);
            }
            return outClip;
        }


        /////////////////////////Object设置///////////////////////////

        /// <summary>
        /// 设动画Fbx的一些默认参数,
        /// 1.设置为Generic模式
        /// 2.导入材质,动画
        /// 3.关闭骨骼优化  --这个在游戏中动态开启
        /// </summary>
        /// <param name="modelImporter">模型导入器</param>
        public static bool SetupSkinFbxImporter(ModelImporter modelImporter)
        {
            if (modelImporter == null)
                return false;

            modelImporter.meshCompression = ModelImporterMeshCompression.High;
            modelImporter.isReadable = false;
            modelImporter.importTangents = ModelImporterTangents.None;
            modelImporter.importMaterials = true;                                       //导入材质,这样就不用手动查找了
            modelImporter.importAnimation = true;                                       //同上
            modelImporter.animationType = ModelImporterAnimationType.Generic;           //使用Generice类型的动画
            modelImporter.optimizeGameObjects = false;                                  //动态骨骼优化

            //模型节点优化选项
            //modelImporter.optimizeGameObjects = false;                                //与AnimatorUtil.OptimizeTransformHierarchy配合使用
            //List<string> exposeBonesName = new List<string>();
            //foreach (var oldBonePath in modelImporter.extraExposedTransformPaths)     //以前的
            //{
            //    exposeBonesName.Add(oldBonePath);
            //}
            //foreach (var bonePath in modelImporter.transformPaths)                    //新增的
            //{
            //    string boneName = Path.GetFileName(bonePath);
            //    if (boneName != "")
            //    { 
            //        if (boneName.ToLower().Contains("g_"))
            //        {
            //            exposeBonesName.Add(bonePath);
            //        }
            //    }
            //}
            //modelImporter.extraExposedTransformPaths = exposeBonesName.ToArray();     //必须的,不然保存不上,理由参考⑴

            return true;
        }

        /// <summary>
        /// 设动画Fbx的一些默认参数,
        /// 1.设置容差值,不易调太高
        /// 2.不压缩动画,原因不明,最好关掉
        /// 3.动画类型,与模型匹配
        /// </summary>
        /// <param name="modelImporter">模型导入器</param>
        public static bool SetupAnimationFbxImporter(ModelImporter modelImporter, bool isLoop = false)
        {
            if (modelImporter == null)
                return false;

            //容差值调整
            modelImporter.animationPositionError = 10.0f;
            modelImporter.animationRotationError = 10.0f;
            modelImporter.animationScaleError = 10.0f;
            modelImporter.animationCompression = ModelImporterAnimationCompression.Off; //不压缩动画(非精度压缩)
            modelImporter.animationType = ModelImporterAnimationType.Generic;           //修改动画格式
            modelImporter.optimizeGameObjects = true;                                   //开不开都可以,

            //循环设置
            modelImporter.clipAnimations = modelImporter.defaultClipAnimations;         //必须的,不然一开始chip是空的
            ModelImporterClipAnimation[] clipAnimations = null;
            clipAnimations = modelImporter.clipAnimations;
            foreach (var animatorImporter in clipAnimations)
            {
                animatorImporter.loopTime = isLoop;
            }
            modelImporter.clipAnimations = clipAnimations;                              //⑴对于数组的设置,不能进行遍历单个的修改,必须这样赋整

            return true;
        }

        /// <summary>
        /// 生成一个最小的碰撞包围盒
        /// </summary>
        /// <param name="GO">模型GO</param>
        public static void SetupModelBoxCollider(GameObject GO)
        {
            if (GO != null)
            {
                var collider = GO.GetComponent<Collider>();
                if (collider == null)
                {
                    Vector3 transCenter = Vector3.zero;
                    Bounds transBounds = new Bounds(transCenter, Vector3.zero);
                    Transform[] transArray = GO.GetComponentsInChildren<Transform>();   //遍历所有点边界,找出一个最小的包围盒
                    foreach (var trans in transArray)
                    {
                        transCenter += trans.position;
                        transBounds.Encapsulate(trans.position);
                    }
                    if (transArray.Length > 0)
                    {
                        transCenter /= transArray.Length;
                    }

                    Vector3 meshCenter = Vector3.zero;
                    Bounds meshBounds = new Bounds(meshCenter, Vector3.zero);
                    SkinnedMeshRenderer[] SMRs = GO.GetComponentsInChildren<SkinnedMeshRenderer>();//遍历所有模型边界,找出一个最小的包围盒
                    foreach (var smr in SMRs)
                    {
                        meshCenter += smr.bounds.center;
                        meshBounds.Encapsulate(smr.bounds);
                    }
                    if (SMRs.Length > 0)
                    {
                        meshCenter /= SMRs.Length;
                    }

                    //没有找到任何参考中心点时
                    if (transArray.Length <= 0 || SMRs.Length <= 0)
                    {
                        Debug.LogWarning("找不到参考边界,无法设置碰撞盒子,请手动设置");
                        return;
                    }

                    //取两者最小
                    Vector3 offset = new Vector3(GO.transform.position.x, -GO.transform.position.y, GO.transform.position.z);
                    var finalSize = new Vector3(
                        Mathf.Min(transBounds.size.x, meshBounds.size.x),
                        Mathf.Min(transBounds.size.y, meshBounds.size.y),
                        Mathf.Min(transBounds.size.z, meshBounds.size.z)
                    );
                    Vector3 finalCenter = new Vector3(transCenter.x, meshCenter.y, transCenter.z);

                    var boxCollider = GO.AddComponent<BoxCollider>();
                    boxCollider.center = GO.transform.InverseTransformPoint(finalCenter);
                    boxCollider.size = (finalSize + offset);

                    boxCollider.isTrigger = true;
                }
            }
        }

        //设置材质文件
        public static bool SetupModelMaterials(GameObject modelGO, List<List<Material>> matsList)
        {
            if (modelGO)
            {
                //渲染组件
                var rendererArray = modelGO.GetComponentsInChildren<Renderer>();
                for (int i = 0; i < rendererArray.Length; i++)
                {
                    List<Material> mats = matsList[i];
                    rendererArray[i].sharedMaterials = mats.ToArray();
                }
                AssetDatabase.SaveAssets();
                return true;
            }

            return false;
        }

        //设置状态机
        public static bool SetupAnimationController(GameObject modelGO, AnimatorController ctrl)
        {   
            if (ctrl && modelGO)
            {
                var animator = modelGO.GetComponent<Animator>();
                if (animator == null)
                {
                    modelGO.AddComponent<Animator>();
                }
                animator.runtimeAnimatorController = ctrl;

                EditorUtility.SetDirty(modelGO);
                AssetDatabase.SaveAssets();                                     //保存变更,不然没得内容
                return true;
            }
            return false;
        }

        //设置状态
        public static bool SetupAnimationState(AnimatorController ctrl ,AnimationClip clip, bool isDefault = false)
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

        //////////////////////动画优化函数//////////////////
        /// <summary>
        /// 剔除AnimationClip中的Scale属性
        /// </summary>
        /// <param name="srcAniClip">源Clip对象</param>
        public static bool RidAnimationScale(AnimationClip srcAniClip)
        {
            if (srcAniClip == null)
            {
                return false;
            }

            var bindings = AnimationUtility.GetCurveBindings(srcAniClip);
            foreach (var binding in bindings)
            {
                string name = binding.propertyName.ToLower();
                if (name.Contains("scale"))
                {
                    AnimationUtility.SetEditorCurve(srcAniClip, binding, null);
                }
            }
            return true;
        }

        /// <summary>
        /// 压缩动画精度
        /// </summary>
        /// <param name="srcAniClip">源Clip对象</param>
        public static bool CompressionPrecision(AnimationClip srcAniClip)
        {
            if (srcAniClip == null)
            {
                return false;
            }
            //浮点数精度压缩到f3
            AnimationClipCurveData[] curves = AnimationUtility.GetAllCurves(srcAniClip);
            Keyframe key;
            Keyframe[] keyFrames;
            for (int ii = 0; ii < curves.Length; ++ii)
            {
                AnimationClipCurveData curveDate = curves[ii];
                if (curveDate.curve == null || curveDate.curve.keys == null)
                {
                    continue;
                }
                keyFrames = curveDate.curve.keys;
                for (int i = 0; i < keyFrames.Length; i++)
                {
                    key = keyFrames[i];
                    key.value = float.Parse(key.value.ToString("f3"));
                    key.inTangent = float.Parse(key.inTangent.ToString("f3"));
                    key.outTangent = float.Parse(key.outTangent.ToString("f3"));
                    keyFrames[i] = key;
                }
                curveDate.curve.keys = keyFrames;
                srcAniClip.SetCurve(curveDate.path, curveDate.type, curveDate.propertyName, curveDate.curve);
            }
            return true;
        }

        ////////////////////////////////////////////////////////////

    }
}