using UnityEngine;

namespace ASGame
{
    public static class EffectLevelUtil
    {
        public static readonly int maxLevel = 10;
        public static readonly int defaultLv = 1;

        ///////////////特效分级////////////
        public static void SetEffectLevel(GameObject GO, int level)
        {
            if (GO)
            {
                var levelEditor = GO.GetComponent<EffectLevelMono>();
                if (levelEditor == null)
                {
                    levelEditor = GO.AddComponent<EffectLevelMono>();
                }
                levelEditor.level = level;
            }
        }

        public static int GetEffectLevel(GameObject GO)
        {
            int level = -1;

            var levelEditor = GO.GetComponent<EffectLevelMono>();
            if (levelEditor != null)
            {
                return levelEditor.level;
            }

            return level;
        }

        public static void ResetEffectLevel(GameObject GO)
        {
            if (GO)
            {
                var levelEditor = GO.GetComponent<EffectLevelMono>();
                if (levelEditor != null)
                {
                    Object.DestroyImmediate(levelEditor);
                }
            }
        }

        public static void ShowEffectLevel(GameObject GO, int showLv)
        {
            //如果父节点不显示,子节点也不显示(?

            if (GO)
            {
                foreach (var node in GO.GetComponentsInChildren<Transform>(true)) //隐藏的也要查找  
                {
                    if (node == GO)
                    {
                        continue;
                    }
                    int nodeLv = GetEffectLevel(node.gameObject);
                    nodeLv = nodeLv == -1 ? 1 : nodeLv;
                    node.gameObject.SetActive(nodeLv <= showLv);
                }
            }
        }

    }
}