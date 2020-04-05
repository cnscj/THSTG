using UnityEditor;
using UnityEngine;
using XLibEditor;

namespace ASEditor
{
    public class ResourceConfigWindow : BaseResourceConfigWindow<ResourceConfigWindow>
    {

        [MenuItem("AssetsStudio/资源全局配置",false,1)]
        static void ShowWnd()
        {
            ShowWindow("资源配置");
        }
        protected override void OnInit()
        {
            AddObject(ResourceConfig.GetInstance());
        }

        protected override void OnShow()
        {
            ShowPropertys("Normal");
            ShowModulePath();
            ShowPathList();
        }
        protected void ShowModulePath()
        {
            GUILayoutEx.ShowPathBar("工具模块路径", ref ResourceConfig.GetInstance().resModulePath);
        }

        protected void ShowPathList()
        {
            GUILayout.Label("资源配置路径:");
            foreach (var infos in ResourceConfig.GetInstance().resPathList)
            {
                infos.key = EditorGUILayout.TextField("资源名(Key)", infos.key);
                GUILayoutEx.ShowPathBar("源资源路径", ref infos.srcPath);

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
