using UnityEngine;
using UnityEditor;
using THGame;
namespace THEditor
{
    public class ResourceConfigWindow : BaseResourceConfigWindow<ResourceConfigWindow>
    {

        //[MenuItem("THFramework/资源工具/资源配置/全局配置")]
        static void ShowWnd()
        {
            ShowWindow("资源配置");
        }

    }
}
