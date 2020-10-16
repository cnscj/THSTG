
using UnityEditor;
using UnityEngine;

namespace ASEditor
{
    public static class EffectLevelMenu
    {
        [MenuItem("GameObject/ASEditor/资源扩展/特效/节点分级/Level_01 #1")]
        public static void MenuChangeLevelOne() { EffectLevelEditTools.ChangEffectLevel(1); }
        [MenuItem("GameObject/ASEditor/资源扩展/特效/节点分级/Level_02 #2")]
        public static void MenuChangeLevelTwo() { EffectLevelEditTools.ChangEffectLevel(2); }
        [MenuItem("GameObject/ASEditor/资源扩展/特效/节点分级/Level_03 #3")]
        public static void MenuChangeLevelThree() { EffectLevelEditTools.ChangEffectLevel(3); }
        [MenuItem("GameObject/ASEditor/资源扩展/特效/节点分级/Level_04 #4")]
        public static void MenuChangeLevelFour() { EffectLevelEditTools.ChangEffectLevel(4); }
        [MenuItem("GameObject/ASEditor/资源扩展/特效/节点分级/Level_UI #5")]
        public static void MenuChangeLevelFive() { EffectLevelEditTools.ChangEffectLevel(5); }
    }
}