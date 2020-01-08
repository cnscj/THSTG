using UnityEngine;
using System.Collections;

namespace THGame
{
    public class CameraRadialBlur : MonoBehaviour
    {
        static Material s_material; 

        public static void ApplyRadialBlur(float range, float shape, float alpha)
        {
            if (s_material)
            {
                s_material.SetFloat("_Range", range);
                s_material.SetFloat("_Shape", shape); 
                s_material.SetFloat("_Alpha", alpha);
            }
        }

        void Start()
        {
            Shader shader = Shader.Find("Hidden/TH/PostProcessRadialBlur");
            s_material = new Material(shader);
        }

        private void OnRenderImage(RenderTexture source, RenderTexture destination)
        {
            // 径向模糊
            Graphics.Blit(source, destination, s_material);
        }
    }
}