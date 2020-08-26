using UnityEngine;
using System.Collections;
using THGame;

//设置在编辑模式下也执行该脚本
[ExecuteInEditMode]

public class GaussianBlurEffect : BasePostEffect
{
   
    //几个用于调节参数的中间变量
    public static int ChangeValue;
    public static float ChangeValue2;
    public static int ChangeValue3;

    //模糊扩散度
    [Range(0.0f, 20.0f), Tooltip("进行高斯模糊时，相邻像素点的间隔。此值越大相邻像素间隔越远，图像越模糊。但过大的值会导致失真。")]
    [Header("模糊扩散度")]
    public float blurSpreadSize = 8.0f;

    //降采样次数
    [Range(0, 6), Tooltip("向下采样的次数。此值越大,则采样间隔越大,需要处理的像素点越少,运行速度越快。")]
    [Header("降采样次数")]
    public int downSampleNum = 3;

    //迭代次数
    [Range(0, 8), Tooltip("此值越大,则模糊操作的迭代次数越多，模糊效果越好，但消耗越大。")]
    [Header("迭代次数")]
    public int blurIterations = 2;

    protected override Shader OnShader()
    {
        return Shader.Find("Hidden/TH/PostProcessGaussianBlur");
    }

    void OnRenderImage(RenderTexture sourceTexture, RenderTexture destTexture)
    {
        
        if (Material != null)
        {
           
            float widthMod = 1.0f / (1.0f * (1 << downSampleNum));
            Material.SetFloat("_DownSampleValue", blurSpreadSize * widthMod);
            sourceTexture.filterMode = FilterMode.Bilinear;
           
            int renderWidth = sourceTexture.width >> downSampleNum;
            int renderHeight = sourceTexture.height >> downSampleNum;

           
            RenderTexture renderBuffer = RenderTexture.GetTemporary(renderWidth, renderHeight, 0, sourceTexture.format);
            renderBuffer.filterMode = FilterMode.Bilinear;
            Graphics.Blit(sourceTexture, renderBuffer, Material, 0);
            
            for (int i = 0; i < blurIterations; i++)
            {
                float iterationOffs = (i * 1.0f);

                Material.SetFloat("_DownSampleValue", blurSpreadSize * widthMod + iterationOffs);
                RenderTexture tempBuffer = RenderTexture.GetTemporary(renderWidth, renderHeight, 0, sourceTexture.format);
                Graphics.Blit(renderBuffer, tempBuffer, Material, 1);
                RenderTexture.ReleaseTemporary(renderBuffer);
                renderBuffer = tempBuffer;
                tempBuffer = RenderTexture.GetTemporary(renderWidth, renderHeight, 0, sourceTexture.format);
                Graphics.Blit(renderBuffer, tempBuffer, Material, 2);

             
                RenderTexture.ReleaseTemporary(renderBuffer);
                renderBuffer = tempBuffer;
            }

            Graphics.Blit(renderBuffer, destTexture);
            RenderTexture.ReleaseTemporary(renderBuffer);
        }
        else
        {
            Graphics.Blit(sourceTexture, destTexture);
        }
    }
}