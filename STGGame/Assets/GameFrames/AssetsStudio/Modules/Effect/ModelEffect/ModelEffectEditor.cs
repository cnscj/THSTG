using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using XLibrary;

//NOTE:如果Unity不报错,打包机打包报找不到命名空间错,就用下面宏定义包住
#if UNITY_EDITOR
namespace ASGame.Editor
{
    public class ModelEffectEditor : MonoBehaviour
    {

        [HideInInspector]
        public bool isCommonModelEffect = false;
        public GameObject modelPrefab;
        [HideInInspector]
        public GameObject modelEffectPrefab;
        public int level = 0;   
        public List<ModelEffectMetadata> metadataList;
        [SerializeField]
        private GameObject m_lastModelPrafab;
        [SerializeField]
        private GameObject m_lastModelEffectPrefab;
        [HideInInspector]
        public int skeletonUID = -1;
        private const string s_modelTargetName = "_Template";
        [HideInInspector]
        public List<string> animStateList = new List<string>();

        [ContextMenu("刷新")]
        public void Refresh()
        {
            GameObject model = GetEditingModelGO();
            if (model != null)
            {
                ModelEffectUtil.RefreshModelEffectInfo(model, ref metadataList);
            }
        }

        [ContextMenu("清空")]
        public void Clear()
        {
            ClearAllEffects();
        }

        [ContextMenu("导出")]
        public bool Export()
        {
            string savePath = null;
            GameObject curModel = modelEffectPrefab;
            if (curModel != null) 
            {
                savePath = AssetDatabase.GetAssetPath(curModel);
            }
            else
            {
                string GOname = gameObject.name;
                string exportName = GOname.Replace("(Template)","");
                savePath = EditorUtility.SaveFilePanel(  
                    "Save ModelEffect as prefab",  
                    "Assets/ModelEffects/Common",  
                    exportName + ".prefab",  
                    "prefab");  
                savePath = XFileTools.GetFileRelativePath(savePath);
            }

            if (savePath != null) 
            {
                bool ret = ExportPrefab(savePath);
                if (ret)
                {
                    modelEffectPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(savePath);
                    Debug.Log(string.Format("导出成功!路径:{0}", savePath));
                    return true;
                }
            }
            return false;
        }
        public void PlayAnimation(int index)
        {
            if (animStateList.Count >= index)
            {
                string state = animStateList[index];
                GameObject target = GetEditingModelGO();
                if (target)
                {
                    var animator = target.GetComponent<Animator>();
                    if (animator)
                    {
                        animator.Play(state,0,0);
                    }
                }
               
            }
        }
        public bool ExportPrefab(string savePath)
        {
            Refresh();

            GameObject modelEffect = new GameObject();
            var info = modelEffect.AddComponent<ModelEffectInfo>();
            info.skeletonId = skeletonUID;
            info.metadataList = new List<ModelEffectMetadata>();
            info.totalLevel = level;

            foreach (var meatdata in metadataList) 
            {

                var newEffectGO = ModelEffectUtil.CopyGO(meatdata.effectGO);
                newEffectGO.transform.SetParent(modelEffect.transform,false);

                ModelEffectMetadata newMetadata = new ModelEffectMetadata();
                newMetadata.effectGO = newEffectGO;
                newMetadata.bonePath = meatdata.bonePath;
                newMetadata.level = meatdata.level;

                info.metadataList.Add(newMetadata);
            }

            PrefabUtility.SaveAsPrefabAsset(modelEffect, savePath);
            GameObject.DestroyImmediate(modelEffect);

            return true;
        }
        public void Init()
        {
            if (m_lastModelPrafab != modelPrefab)
            {
                m_lastModelPrafab = modelPrefab;
                UpdateModelTarget();
            }
        }

        private void Awake() 
        {
            Refresh();
        }

        private void OnValidate()
        {
            if (m_lastModelPrafab != modelPrefab)
            {
                m_lastModelPrafab = modelPrefab;
                Invoke("UpdateModelTarget", 0);
            }

            if (m_lastModelEffectPrefab != modelEffectPrefab)
            {
                if (modelEffectPrefab != null)
                {
                    if (IsModelEffect(modelEffectPrefab))
                    {
                        Invoke("UpdateModelEffectTarget", 0);
                        m_lastModelEffectPrefab = modelEffectPrefab;
                    }
                    else
                    {
                        modelEffectPrefab = m_lastModelEffectPrefab;
                    }
                }
                else
                {
                    m_lastModelEffectPrefab = modelEffectPrefab;
                }
            }

            if (modelPrefab)
            {
                GameObject target = GetEditingModelGO();
                if (target)
                {
                    UpdateModelAnimation(target);
                }
            }
        }

        private void UpdateModelTarget()
        {
            if (!isCommonModelEffect)
            {
                return ;
            }

            if (modelPrefab)
            {   
                GameObject target = GetEditingModelGO();
                if (target)
                {
                    GameObject go = Instantiate(modelPrefab, transform, false);
                    TransferModelEffect(go);
                    UpdateModelAnimation(go);
                    skeletonUID = ModelEffectUtil.GetSkeletonUID(modelPrefab);

                    DestroyImmediate(target);

                    go.name = s_modelTargetName;
                }
                else
                {
                    GameObject go = Instantiate(modelPrefab, transform, false);
                    UpdateModelAnimation(go);
                    skeletonUID = ModelEffectUtil.GetSkeletonUID(modelPrefab);
                    go.name = s_modelTargetName;
                }
            }
        }

        private void UpdateModelEffectTarget()
        {
            if (!isCommonModelEffect)
            {
                return ;
            }
            //重新挂载到模型上
            if (modelEffectPrefab)
            {
                GameObject target = GetEditingModelGO();
                if (target)
                {
                    ClearAllEffects();
                    
                    GameObject modelEffectGO = Instantiate(modelEffectPrefab, transform, false);
                    BindModelEffectToModel(modelEffectGO, target);

                    DestroyImmediate(modelEffectGO);
              
                    Refresh();
                }
            }
        }

        private bool BindModelEffectToModel(GameObject modelEffectGO, GameObject model)
		{
            if (modelEffectGO)
            {
                //获取所有节点
                foreach (var metadata in modelEffectGO.GetComponent<ModelEffectInfo>().metadataList)
                {
                    string bonePath = metadata.bonePath;
                    GameObject leafNodeParent = ModelEffectUtil.GetGOByPath(gameObject, bonePath, true);
                    GameObject effectPrefab = metadata.effectGO;
                    GameObject effectGO = GameObject.Instantiate(effectPrefab, leafNodeParent.transform, false);

                    if (effectGO != null)
                    {
                        effectGO.name = metadata.effectGO.name;
                    }
                    return true;
                }
            }
            return false;
           
        }

        private GameObject GetEditingModelGO()
        {
            if (isCommonModelEffect)
            {
                Transform target = transform.Find(s_modelTargetName);
                if (target)
                {
                    return target.gameObject;
                }
            }
            else
            {
                return this.gameObject;
            }
            return null;
        }
        
        private bool IsModelEffect(GameObject GO)
        {
            if (GO != null)
            {
                var info = GO.GetComponent<ModelEffectInfo>();
                return info ? true : false;
            }
            return false;
        }
        private void ClearAllEffects()
        {
            Refresh();

            foreach (var metadata in metadataList)
            {
                DestroyImmediate(metadata.effectGO);
            }

            Refresh();
        }
        private void TransferModelEffect(GameObject newGo)
        {
            Refresh();  //刷新一下
            foreach (var metadata in metadataList)
            {
                string bonePath = metadata.bonePath;
                GameObject leafNodeParent = ModelEffectUtil.GetGOByPath(newGo, bonePath, true);
                if (leafNodeParent != null)
                {
                    GameObject effectGO = metadata.effectGO;
                    if (effectGO)
                    {
                        Vector3 oldLocalPos = effectGO.transform.localPosition;
                        effectGO.transform.SetParent(leafNodeParent.transform);
                        effectGO.transform.localPosition = oldLocalPos;
                    }
                    else
                    {
                        Debug.LogError(string.Format("特效节点丢失:{0}", bonePath));
                    }
                }
                else
                {
                    Debug.LogError(string.Format("特效文件数据丢失:{0}", bonePath));
                }
            }
        }
        private void UpdateModelAnimation(GameObject newGo)
        {
            animStateList.Clear();
            var animator = newGo.GetComponent<Animator>();
            if (animator)
            {
                var animaCtrl = animator.runtimeAnimatorController;
                if (animaCtrl)
                {
                    var clips = animaCtrl.animationClips;
                    foreach (var clip in clips)
                    {
                        animStateList.Add(clip.name);
                    }
                }
            }
        }

    }  
}

#endif