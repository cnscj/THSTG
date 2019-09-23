using UnityEngine;
using UnityEditor;
using ASGame;
namespace ASEditor
{
    public static class EffectLengthMenu
    {
        [MenuItem("GameObject/ASEditor/资源扩展/特效/设置特效时长")]
        public static void MenuCalculateFxLength()
        {
            var selecteds = Selection.gameObjects;
            if (selecteds.Length > 0)
            {
                foreach (var fxGO in selecteds)
                {
                    var EffectLengthMono = fxGO.GetComponent<EffectLengthMono>();
                    if (!EffectLengthMono)
                    {
                        EffectLengthMono = fxGO.AddComponent<EffectLengthMono>();

                    }
                    float len = EffectLengthTools.CalculatePlayEndTime(fxGO);
                    EffectLengthMono.length = len;

                    Debug.Log(len);
                }
                AssetDatabase.Refresh();
            }
            else
            {
                Debug.LogError("没有选中特效Prefab");
            }
        }

    }


}
