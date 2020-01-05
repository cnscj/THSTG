using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace STGU3D
{
    public class ViewShaderEffect : MonoBehaviour
    {
        public static readonly string KEY_GRAY = "GRAY_ON";
        //所有渲染器
        public List<Material> materials ;

        public void Add(GameObject go)
        {
            if (go == null)
                return;

            materials = materials ?? new List<Material>();
            foreach(var renderer in go.GetComponentsInChildren<Renderer>())
            {
                materials.AddRange(renderer.sharedMaterials);
            }
            
        }

        //灰显
        public void SetGray(bool val)
        {
            if (materials != null)
            {
                foreach(var material in materials)
                {
                    if (val)
                    {
                        material.EnableKeyword(KEY_GRAY);
                    }
                    else
                    {
                        material.DisableKeyword(KEY_GRAY);
                    }
                    
                }
            }
        }

        //外发光(轮廓光

        //内发光

        //石化

        //溶解

        //闪烁
    }
}
