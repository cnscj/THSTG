using UnityEngine;
namespace THGame
{
    public class ScreenMask : BasePostEffect
    {
        // 遮罩大小
        [Range(0.01f, 10f), Tooltip("遮罩大小")]
        public float size = 0.3f;
        // 边缘模糊程度
        [Range(0.0001f, 0.1f), Tooltip("边缘模糊程度")]
        public float edgeBlurLength = 0.05f;
        // 遮罩中心位置
        public Vector2 pos = new Vector4(0.5f, 0.5f);

        protected override Shader OnShader()
        {
            return Shader.Find("Hidden/TH/PostProcessMaskEffect");
        }

        // 渲染屏幕
        void OnRenderImage(RenderTexture source, RenderTexture destination)
        {
            if (Material)
            {
                // 把鼠标坐标传递给shader
                Material.SetVector("_Pos", pos);
                // 遮罩大小
                Material.SetFloat("_Size", size);
                // 模糊程度
                Material.SetFloat("_EdgeBlurLength", edgeBlurLength);
                // 渲染
                Graphics.Blit(source, destination, Material);
            }
            else
            {
                Graphics.Blit(source, destination);
            }
        }

    }
}
