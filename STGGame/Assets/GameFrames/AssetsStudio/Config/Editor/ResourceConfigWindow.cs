using UnityEditor;
using UnityEngine;
using XLibraryEditor;

namespace ASEditor
{
    public class ResourceConfigWindow : BaseResourceConfigWindow<ResourceConfigWindow>
    {

        [MenuItem("AssetsStudio/资源全局配置",false,1)]
        static void ShowWnd()
        {
            ShowWindow("资源配置");
        }
        protected override void OnObjs()
        {
            var obj = AddObject(ResourceConfig.GetInstance());
        }

        protected override void OnProps()
        {
            
        }

        protected override void OnShow()
        {
            ShowPropertys("Normal");
            ShowPathList();
        }


        protected void ShowPathList()
        {
            GUILayout.Label("资源配置路径:");
            foreach (var infos in ResourceConfig.GetInstance().resPathList)
            {
                infos.key = EditorGUILayout.TextField("资源名", infos.key);
                GUIUtil.ShowPathBar("源资源路径", ref infos.srcPath);

                if (GUILayout.Button("移除"))
                {
                    ResourceConfig.GetInstance().resPathList.Remove(infos);
                    return;
                }
                EditorGUILayout.Space();
            }
            if (GUILayout.Button("...."))
            {
                ResourcePathConfigInfos info = new ResourcePathConfigInfos();
                var pathList = ResourceConfig.GetInstance().resPathList;
                pathList.Add(info);
            }
        }
    }
}
