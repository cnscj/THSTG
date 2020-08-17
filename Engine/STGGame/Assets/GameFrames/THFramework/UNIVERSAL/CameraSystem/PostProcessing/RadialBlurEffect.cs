using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace THGame
{
    public class RadialBlurEffect : BasePostEffect
    {
        //模糊程度，不能过高
        [Range(0, 0.5f)]
        public float blurFactor = 0.045f;

        [Range(0.0f, 2.0f)]
        public float lerpFactor = 2.0f;//清晰图像与原图插值

        [Header("模糊中心（0-1）")]
        public Vector2 blurCenter = new Vector2(0.5f, 0.5f);//屏幕空间，默认为中心点

        private int downSampleFactor = 2;//降低分辨率

        void OnRenderImage(RenderTexture source, RenderTexture destination)
        {
            if (_Material)
            {
                //申请两块降低了分辨率的RT
                RenderTexture rt1 = RenderTexture.GetTemporary(source.width >> downSampleFactor, source.height >> downSampleFactor, 0, source.format);
                RenderTexture rt2 = RenderTexture.GetTemporary(source.width >> downSampleFactor, source.height >> downSampleFactor, 0, source.format);
                Graphics.Blit(source, rt1);

                //使用降低分辨率的rt进行模糊:pass0
                _Material.SetFloat("_BlurFactor", blurFactor);
                _Material.SetVector("_BlurCenter", blurCenter);
                Graphics.Blit(rt1, rt2, _Material, 0);

                //使用rt2和原始图像lerp:pass1
                _Material.SetTexture("_BlurTex", rt2);
                _Material.SetFloat("_LerpFactor", lerpFactor);
                Graphics.Blit(source, destination, _Material, 1);

                //释放RT
                RenderTexture.ReleaseTemporary(rt1);
                RenderTexture.ReleaseTemporary(rt2);
            }
            else
            {
                Graphics.Blit(source, destination);
            }

        }

    }
}

