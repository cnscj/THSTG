using System.Collections.Generic;
using System.IO;
using ASEditor;
using UnityEditor;
using UnityEngine;

namespace ASGame
{
    [System.Serializable]
    public class EffectLevelController : MonoBehaviour
    {
        public List<EffectLevelMetadata> metadataList;       //数据源

        public void SetupPacks(GameObject[] packs)
        {
            for (int i = 0; i < packs.Length; i++)
            {
                int level = EffectLevelUtil.GetEffectLevel(packs[i]);
                if (level == metadataList[i].level)
                {
                    GameObject pack = packs[i];
                    for(int j = 0; j< metadataList[i].effectList.Count; j++)
                    {
                        string path = metadataList[i].effectList[j];
                        Transform effectNode = pack.transform.GetChild(j);
                        if (effectNode)
                        {
                            string fatherPath = GetParentPath(path);
                            if (fatherPath != "")
                            {
                                Transform fatherNode = gameObject.transform.Find(fatherPath);
                                if (fatherNode)
                                {
                                    var go = GameObject.Instantiate(effectNode.gameObject, fatherNode, false);
                                    go.name = go.name.Replace("(Clone)", "");
                                }
                            }
                            else
                            {
                                var go = GameObject.Instantiate(effectNode.gameObject, gameObject.transform,false);
                                go.name = go.name.Replace("(Clone)", "");
                            }
                        }
                    }
                }
            }
          
        }

        public void UninstallPack(int limitLv)
        {
            for (int i = metadataList.Count - 1; i >= 0; i--)
            {
                int packLv = metadataList[i].level;
                if (packLv > limitLv)
                {
                    foreach(var childPath in metadataList[i].effectList)
                    {
                        Transform childNode = gameObject.transform.Find(childPath);
                        if (childNode)
                        {
                            Destroy(childNode.gameObject);
                        }
                    }
                }
            }
        }

        private void Start()
        {
            EffectLevelManager.instance.levelChangedCallback += OnLimitLevelChanged;
        }
        private void OnDestroy()
        {
            EffectLevelManager.instance.levelChangedCallback -= OnLimitLevelChanged;
        }

        private void OnLimitLevelChanged(int val)
        {

        }
        private string GetParentPath(string childPath)
        {
            string fatherPath = "";
            int index = childPath.LastIndexOf("/", System.StringComparison.Ordinal);
            if (index >= 0)
            {
                fatherPath = childPath.Remove(index);
            }
            return fatherPath;
        }
    }
}