using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace STGU3D
{
    public class ViewRenderer : MonoBehaviour
    {
        public List<Renderer> renderers;
        public void Add(GameObject go)
        {
            if (go == null)
                return;

            renderers = renderers ?? new List<Renderer>();
            renderers.AddRange(go.GetComponentsInChildren<Renderer>());
        }

        public void SetFlipX(bool val)
        {
            if (renderers != null)
            {
                foreach(var renderer in renderers)
                {
                    var spriteRenderer = renderer as SpriteRenderer;
                    spriteRenderer.flipX = val;
                }
            }
        }

        public void SetFlipY(bool val)
        {
            if (renderers != null)
            {
                foreach (var renderer in renderers)
                {
                    var spriteRenderer = renderer as SpriteRenderer;
                    spriteRenderer.flipY = val;
                }
            }
        }
    }
}