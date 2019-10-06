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
        protected override void OnObjs()
        {
            AddObject(AssembliesToolsConfig.GetInstance());
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
            GUILayout.Label("程序集路径配置:");
            foreach (var infos in AssembliesToolsConfig.GetInstance().buildPathList)
            {
                GUIUtil.ShowPathBar("源资源路径", ref infos.srcPath);
                GUIUtil.ShowPathBar("临时导出路径", ref infos.buildPath);
                GUIUtil.ShowPathBar("映射路径", ref infos.projectPath);
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
