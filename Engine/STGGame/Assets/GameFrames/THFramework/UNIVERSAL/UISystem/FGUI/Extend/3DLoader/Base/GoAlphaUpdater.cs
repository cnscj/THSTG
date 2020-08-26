using System.Collections;
using System.Collections.Generic;
using FairyGUI;
using UnityEngine;

namespace THGame.UI
{
    public class GoAlphaUpdater : GoBaseUpdater
    {
        private MaterialPropertyBlock _materialPropertyBlock;
        private Color _fguiColor = Color.white;
        public GoAlphaUpdater()
        {
            _materialPropertyBlock = new MaterialPropertyBlock();
        }

        public override void OnUpdate()
        {
            if (context.wrapperTarget != null && context.wrapperContext != null)
            {
                //要区别,模型,特效,模型特效这些情况
                var alpha = context.wrapperContext.alpha;
                var renderers = context.wrapperTarget.GetComponentsInChildren<Renderer>();
                foreach(var renderer in renderers)
                {
                    renderer.GetPropertyBlock(_materialPropertyBlock);

                    _fguiColor.a = alpha;
                    _materialPropertyBlock.SetColor("_FGUIColor", _fguiColor);

                    renderer.SetPropertyBlock(_materialPropertyBlock);

                }
            }

        }

    }
}

