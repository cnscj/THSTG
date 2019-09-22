using System.Collections.Generic;
using UnityEngine;
namespace ASGame
{
    [System.Serializable]
    public class AutoModelEffect : MonoBehaviour
    {    
        [SerializeField] 
        public GameObject modelEffect;
        [SerializeField]
        public List<GameObject> effectGOs = new List<GameObject>();
        public void CreateEffects()
        {
            if (modelEffect != null)
            {
                //获取所有节点
                int limitLevel = ModelEffectManager.instance.limitLevel;
                foreach (var metadata in modelEffect.GetComponent<ModelEffectInfo>().metadataList)
                {
                    if (metadata.level <= limitLevel)
                    {
                        string bonePath = metadata.bonePath;
                        GameObject leafNodeParent = ModelEffectUtil.GetGOByPath(gameObject, bonePath, true);
                        GameObject nodePrefab = metadata.effectGO;
                        GameObject effectGO = GameObject.Instantiate(nodePrefab, leafNodeParent.transform, false);
                        if (effectGO != null)
                        {
                            effectGO.name = metadata.effectGO.name;
                            effectGOs.Add(effectGO);
                        }
                    }
 
                }
            }
        }
        public void DestroyEffects()
        {
            foreach (var GO in effectGOs)
            {
                if (GO != null)
                {
                     #if UNITY_EDITOR
                        DestroyImmediate(GO);
                    #else
                        Destroy(GO);
                    #endif
                }
            }
            effectGOs.Clear();
        }

        private void Awake() 
        {
            ModelEffectManager.instance.levelChangedCallback += OnLimitLevelChanged;
        }
        private void OnDestroy()
        {
            DestroyEffects();
            ModelEffectManager.instance.levelChangedCallback -= OnLimitLevelChanged;
        }

        public void OnLimitLevelChanged(int val)
        {

        }
    }
}