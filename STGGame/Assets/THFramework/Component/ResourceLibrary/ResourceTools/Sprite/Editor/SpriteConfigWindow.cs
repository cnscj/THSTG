using UnityEngine;
using UnityEditor;
using THGame;
using System.Collections.Generic;

namespace THEditor
{
    public class SpriteConfigWindow : BaseResourceConfigWindow<SpriteConfigWindow>
    {
        [MenuItem("THFramework/资源工具/资源配置/精灵配置")]
        static void ShowWnd()
        {
            ShowWindow("精灵配置");
        }
        protected override void OnObjs()
        {
            AddObject(SpriteConfig.GetInstance());
        }

        protected override void OnProps()
        {
            AddProperty("defaultShader", "Normal", "默认Shader");
            AddProperty("defaultFrameRate", "Normal", "默认动画帧率");
            AddProperty("defaultStateList", "Normal", "默认动作列表");
            AddProperty("loopStateList", "Normal", "循环动作列表");
        }

        protected override void OnShow()
        {
            ShowPropertys("Normal");
        }
    }
}
