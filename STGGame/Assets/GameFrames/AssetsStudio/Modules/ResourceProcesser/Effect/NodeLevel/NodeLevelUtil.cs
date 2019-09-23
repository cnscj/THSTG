using UnityEngine;
using ASGame.Editor;

namespace ASGame
{
    public static class NodeLevelUtil
    {
        public static readonly int maxLevel = 10;
        public static readonly int defaultLv = 1;

        ///////////////特效分级////////////
        public static void SetEffectLevel(GameObject GO, int level)
        {
            if (GO)
            {
                //string srcGOName = GO.name;
                //int num = GetEffectLevel(GO);
                //if (num != -1)
                //{
                //    int lastIndex = srcGOName.LastIndexOf("_L", System.StringComparison.Ordinal);
                //    if (lastIndex != -1)
                //    {
                //        srcGOName = srcGOName.Remove(lastIndex);
                //    }
                //}
                //GO.name = string.Format("{0}_L{1:D2}", srcGOName, level);
                var levelEditor = GO.GetComponent<NodeLevelEditMono>();
                if (levelEditor == null)
                {
                    levelEditor = GO.AddComponent<NodeLevelEditMono>();
                }
                levelEditor.level = level;
            }
        }

        public static int GetEffectLevel(GameObject GO)
        {
            int level = -1;
            //if (GO)
            //{
            //    string nodeName = GO.name;
            //    int lastIndex = nodeName.LastIndexOf("_L", System.StringComparison.Ordinal);
            //    if (lastIndex != -1)
            //    {
            //        string lvStr = nodeName.Substring(lastIndex + 2, nodeName.Length - lastIndex - 2);
            //        bool ret = int.TryParse(lvStr, out level);
            //        level = ret ? level : -1;
            //    }
            //}

            var levelEditor = GO.GetComponent<NodeLevelEditMono>();
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
                //去掉后四位的等级("_L%02d")
                //node.gameObject.name = node.gameObject.name.Remove(node.gameObject.name.Length - 4);

                var levelEditor = GO.GetComponent<NodeLevelEditMono>();
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