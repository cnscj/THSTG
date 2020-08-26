using System.Collections;
using UnityEngine;

namespace THGame
{
    //编辑状态下也运行  
    [ExecuteInEditMode]
    public class ScreenBloomEffect : BasePostEffect
    {
        //迭代次数
        [Range(0, 4)]
        public int iterations = 3;
        //模糊扩散范围
        [Range(0.2f, 3.0f)]
        public float blurSpread = 0.6f;
        // 亮度阙值
        [Range(-1.0f, 1.0f)]
        public float luminanceThreshold = 0.6f;
        // bloom 强度
        [Range(0.0f, 5.0f)]
        public float bloomFactor = 1;
        // bloom 颜色值
        public Color bloomColor = new Color(1, 1, 1, 1);

         // 降频
        private int downSample = 1;


        protected override Shader OnShader()
        {
            return Shader.Find("Hidden/TH/PostProcessBloom");
        }

        private void OnRenderImage(RenderTexture source, RenderTexture destination)
        {
            if (Material)
            {
                int rtW = source.width >> downSample;
                int rtH = source.height >> downSample;
                RenderTexture texture1 = RenderTexture.GetTemporary(rtW, rtH, 0);
                RenderTexture texture2 = RenderTexture.GetTemporary(rtW, rtH, 0);
                // 亮度提取 - 通道0
                Material.SetFloat("_LuminanceThreshold", luminanceThreshold);
                Graphics.Blit(source, texture1, Material, 0);

                // 高斯模糊 - 通道1
                for (int i = 0; i < iterations; i++)
                {
                    //垂直高斯模糊
                    Material.SetVector("_offsets", new Vector4(0, 1.0f + i * blurSpread, 0, 0));
                    Graphics.Blit(texture1, texture2, Material, 1);
                    //水平高斯模糊
                    Material.SetVector("_offsets", new Vector4(1.0f + i * blurSpread, 0, 0, 0));
                    Graphics.Blit(texture2, texture1, Material, 1);
                }
                //用模糊图和原始图计算出轮廓图  - 通道2
                Material.SetColor("_BloomColor", bloomColor);
                Material.SetFloat("_BloomFactor", bloomFactor);
                Material.SetTexture("_BlurTex", texture1);
                Graphics.Blit(source, destination, Material, 2);
            }
            else
            {
                Graphics.Blit(source, destination);
            }
        }
    }
}

