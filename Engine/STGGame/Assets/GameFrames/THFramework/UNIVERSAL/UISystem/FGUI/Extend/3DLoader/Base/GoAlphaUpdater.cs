using System.Collections;
using System.Collections.Generic;
using FairyGUI;
using UnityEngine;

namespace THGame.UI
{
    public class GoAlphaUpdater : IGoUpdate
    {
        private MaterialPropertyBlock _materialPropertyBlock;
        private Color _alphaColor = Color.white;
        public GoAlphaUpdater()
        {
            _materialPropertyBlock = new MaterialPropertyBlock();
        }
        public void Update(GoUpdateContext context)
        {
            if (context.wrapperTarget != null && context.wrapperContext != null)
            {
                var alpha = context.wrapperContext.alpha;
                var renderers = context.wrapperTarget.GetComponentsInChildren<Renderer>();
                foreach(var renderer in renderers)
                {
                    renderer.GetPropertyBlock(_materialPropertyBlock);

                    _alphaColor.a = alpha;
                    _materialPropertyBlock.SetColor("_FGUIColor", _alphaColor);

                    renderer.SetPropertyBlock(_materialPropertyBlock);

                }
            }

        }
    }
}

