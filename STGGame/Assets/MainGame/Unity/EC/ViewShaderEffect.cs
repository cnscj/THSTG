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
        private List<Material> m_materials;
        private Queue<Renderer> m_prepareRenderer;

        public void Add(GameObject go)
        {
            if (go == null)
                return;

            m_prepareRenderer = m_prepareRenderer ?? new Queue<Renderer>();
            foreach (var renderer in go.GetComponentsInChildren<Renderer>())
            {
                m_prepareRenderer.Enqueue(renderer);
            }

        }

        public List<Material> GetMaterials()
        {
            if (m_prepareRenderer == null)
                return m_materials;

            if (m_prepareRenderer.Count <= 0)
                return m_materials;

            m_materials = m_materials ?? new List<Material>();
            while (m_prepareRenderer.Count > 0)
            {
                var renderer = m_prepareRenderer.Dequeue();
                var tempMaterials = new List<Material>();
                foreach (var material in renderer.sharedMaterials)
                {
                    var newMaterial = Object.Instantiate(material);
                    tempMaterials.Add(newMaterial);
                }
                renderer.sharedMaterials = tempMaterials.ToArray();    //实例化一份新的Material
                m_materials.AddRange(renderer.sharedMaterials);
            }

            return m_materials;
        }

        //颜色改变
        public Color GetColor()
        {
            if (GetMaterials() != null)
            {
                foreach (var material in GetMaterials())
                {
                    return material.GetColor(NAME_COLOR);
                }
            }
            return Color.black;
        }
        public void SetColor(Color color)
        {
            if (GetMaterials() != null)
            {
                foreach (var material in GetMaterials())
                {
                    material.SetColor(NAME_COLOR, color);
                }
            }
        }

        //透明度
        public float GetAlpha()
        {
            if (GetMaterials() != null)
            {
                foreach (var material in GetMaterials())
                {
                    return material.color.a;
                }
            }
            return 0;
        }
        public void SetAlpha(float color)
        {
            if (GetMaterials() != null)
            {
                foreach (var material in GetMaterials())
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
            if (GetMaterials() != null)
            {
                foreach(var material in GetMaterials())
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
            if (GetMaterials() != null)
            {
                foreach (var material in GetMaterials())
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
