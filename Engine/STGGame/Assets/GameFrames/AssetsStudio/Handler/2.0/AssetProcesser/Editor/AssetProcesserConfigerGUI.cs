using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using XLibEditor;

namespace ASEditor
{
    public class AssetProcesserConfigerGUI : WindowGUI<AssetProcesserConfigerGUI>
    {
        [MenuItem("AssetsStudio/资源后处理配置", false, 4)]
        static void ShowWnd()
        {
            ShowWindow("资源后处理配置");
        }

        protected override void OnInit()
        {
        }

        protected override void OnShow()
        {
            GUILayout.BeginVertical();
            if (GUILayout.Button("处理")) { AssetProcesserManager.GetInstance().Do(); }

            AssetProcesserConfiger.GetInstance().outputFolderPath = GUILayoutEx.ShowPathBar("后处理输出目录", AssetProcesserConfiger.GetInstance().outputFolderPath);
            AssetProcesserConfiger.GetInstance().processFolderName = EditorGUILayout.TextField("处理文件目录名", AssetProcesserConfiger.GetInstance().processFolderName);
            AssetProcesserConfiger.GetInstance().checkfileFolderName = EditorGUILayout.TextField("检查文件目录名", AssetProcesserConfiger.GetInstance().checkfileFolderName);
            AssetProcesserConfiger.GetInstance().createFolderOrAddSuffix = EditorGUILayout.Toggle("为检查文件创建存放目录", AssetProcesserConfiger.GetInstance().createFolderOrAddSuffix);
            AssetProcesserConfiger.GetInstance().useGUID4SaveCheckfileName = EditorGUILayout.Toggle("用GUID作为检查文件名", AssetProcesserConfiger.GetInstance().useGUID4SaveCheckfileName);

            GUILayout.EndVertical();
        }
    }
}
