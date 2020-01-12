using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace STGU3D
{
    public class ViewShaderEffect : ViewBaseClass
    {
        public static readonly string KEY_GRAY = "GRAY_ON";

        public static readonly string NAME_COLOR = "_Color";
        public static readonly string NAME_GRAY = "_Gray";

        //所有渲染器
        public List<Material> materials ;

        public void Add(GameObject go)
        {
            if (go == null)
                return;

            materials = materials ?? new List<Material>();
            foreach(var renderer in go.GetComponentsInChildren<Renderer>())
            {
#if UNITY_EDITOR
                materials.AddRange(renderer.materials);
#else
                //编辑模式下这行代码会修改到源文件
                materials.AddRange(renderer.sharedMaterials);
#endif
            }

        }

        //颜色改变
        public Color GetColor()
        {
            if (materials != null)
            {
                foreach (var material in materials)
                {
                    return material.GetColor(NAME_COLOR);
                }
            }
            return Color.black;
        }
        public void SetColor(Color color)
        {
            if (materials != null)
            {
                foreach (var material in materials)
                {
                    material.SetColor(NAME_COLOR, color);
                }
            }
        }

        //透明度
        public float GetAlpha()
        {
            if (materials != null)
            {
                foreach (var material in materials)
                {
                    return material.color.a;
                }
            }
            return 0;
        }
        public void SetAlpha(float color)
        {
            if (materials != null)
            {
                foreach (var material in materials)
                {
                    var oldColor = material.color;
                    oldColor.a = color;
                    material.color = oldColor;
                }
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
                        material.SetFloat(NAME_GRAY, 1f);
                    }
                    else
                    {
                        material.SetFloat(NAME_GRAY, 0f);
                    } 
                }
            }
        }

        public bool GetGray()
        {
            if (materials != null)
            {
                foreach (var material in materials)
                {
                    return material.GetFloat(NAME_GRAY) >= 1 ? true : false;
                }
            }
            return false;
        }

        //外发光(轮廓光

        //内发光

        //石化

        //溶解
    }
}
