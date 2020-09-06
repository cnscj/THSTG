using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using XLibEditor;

namespace ASEditor
{
    public class AssetUpdatePackageEditor : WindowGUI<AssetUpdatePackageEditor>
    {
        private SearchTextField searchTextField = new SearchTextField();

        [MenuItem("AssetsStudio/资源工具/资源分包器")]
        static void ShowWnd()
        {
            ShowWindow("资源分包配置");
        }

        //参考PackagesWindow
        protected override void OnShow()
        {
            searchTextField.OnGUI();
        }
    }

}
