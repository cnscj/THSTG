using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace THGame
{
    public class OverDrawEffect : BasePostEffect
    {
        public Color sceneColor = Color.white;

        void OnRenderImage(RenderTexture source, RenderTexture destination)
        {
            if (_Material)
            {
                _Material.SetColor("_OverDrawColor", sceneColor);
                Graphics.Blit(source, destination, _Material);
            }
            else
            {
                Graphics.Blit(source, destination);
            }

        }

    }
}

