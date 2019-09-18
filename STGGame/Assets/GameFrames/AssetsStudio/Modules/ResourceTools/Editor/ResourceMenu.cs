using UnityEditor;
using UnityEngine;

namespace ASEditor
{
    public static class ResourceMenu
    {
        [MenuItem("Assets/ASEditor/辅助菜单/取得文件(夹)路径", false, 12)]
        public static void MenuGenSpriteOneKey()
        {
            var selected = Selection.activeObject;
            string selectPath = AssetDatabase.GetAssetPath(selected);
            if (selectPath != "")
            {
                Debug.Log(selectPath);
            }
            else
            {
                Debug.LogError("没有选中文件或文件夹");
            }
        }
    }
}
