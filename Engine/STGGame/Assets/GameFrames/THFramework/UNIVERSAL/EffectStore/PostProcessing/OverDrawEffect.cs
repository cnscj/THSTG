using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace THGame
{
    public class OverDrawEffect : BasePostEffect
    {
        public Color sceneColor = Color.white;

        protected override Shader OnShader()
        {
            return Shader.Find("Hidden/TH/PostProcessOverDraw");
        }

        void OnRenderImage(RenderTexture source, RenderTexture destination)
        {
            if (Material)
            {
                Material.SetColor("_OverDrawColor", sceneColor);
                Graphics.Blit(source, destination, Material);
            }
            else
            {
                Graphics.Blit(source, destination);
            }

        }

    }
}

