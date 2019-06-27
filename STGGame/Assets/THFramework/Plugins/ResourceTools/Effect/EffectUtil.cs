
using UnityEngine;
using UnityEditor;
using System.IO;

namespace THGame
{
    public static class EffectUtil
    {

        public static void SetEffectLevel(GameObject GO, int level)
        {
            if (GO)
            {
                string srcGOName = GO.name;
                int num = GetEffectLevel(GO);
                if (num != -1)
                {
                    int lastIndex = GO.name.LastIndexOf("_L");
                    if (lastIndex != -1)
                    {
                        srcGOName = srcGOName.Remove(lastIndex);
                    }
                }
                GO.name = string.Format("{0}_L{1:D2}", srcGOName, level);
            }
        }

        public static int GetEffectLevel(GameObject GO)
        {
            int level = -1;
            if (GO)
            {
                string nodeName = GO.name;
                int lastIndex = nodeName.LastIndexOf("_L");
                if (lastIndex != -1)
                {
                    string lvStr = nodeName.Substring(lastIndex + 2, nodeName.Length - lastIndex - 2);
                    bool ret = int.TryParse(lvStr, out level);
                    level = ret ? level : -1;
                }
            }
            return level;
        }
    }   
}