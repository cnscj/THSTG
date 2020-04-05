using UnityEditor;
using UnityEngine;
using XLibEditor;

namespace ASEditor
{
    public class AssembliesToolsConfigWindow : BaseResourceConfigWindow<AssembliesToolsConfigWindow>
    {

        [MenuItem("AssetsStudio/资源工具/资源配置/程序集配置")]
        static void ShowWnd()
        {
            ShowWindow("程序集配置");
        }
        protected override void OnInit()
        {
            AddObject(AssembliesToolsConfig.GetInstance());
        }

        protected override void OnShow()
        {
            ShowPropertys();
            ShowPathList();
        }

        protected void ShowPathList()
        {
            GUILayout.Label("程序集路径配置:");
            foreach (var infos in AssembliesToolsConfig.GetInstance().buildPathList)
            {
                GUILayoutEx.ShowPathBar("源资源路径", ref infos.srcPath);
                GUILayoutEx.ShowPathBar("临时导出路径", ref infos.buildPath);
                GUILayoutEx.ShowPathBar("映射路径", ref infos.projectPath);
                if (GUILayout.Button("移除"))
                {
                    AssembliesToolsConfig.GetInstance().buildPathList.Remove(infos);
                    return;
                }
                EditorGUILayout.Space();
            }
            if (GUILayout.Button("...."))
            {
                AssembliesBuildInfos info = new AssembliesBuildInfos();
                var pathList = AssembliesToolsConfig.GetInstance().buildPathList;
                pathList.Add(info);
            }
        }


    }
}
