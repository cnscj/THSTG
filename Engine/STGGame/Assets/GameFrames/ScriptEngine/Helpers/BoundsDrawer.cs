using UnityEngine;

namespace SEGame
{
    public class BoundsDrawer : MonoBehaviour
    {
        // When added to an object, draws colored rays from the
        // transform position.
        public int lineCount = 100;
        public float radius = 3.0f;

        static Material lineMaterial;
        static void CreateLineMaterial()
        {
            if (!lineMaterial)
            {
                // Unity has a built-in shader that is useful for drawing
                // simple colored things.
                Shader shader = Shader.Find("Hidden/Internal-Colored");
                lineMaterial = new Material(shader);
                lineMaterial.hideFlags = HideFlags.HideAndDontSave;
                // Turn on alpha blending
                lineMaterial.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
                lineMaterial.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
                // Turn backface culling off
                lineMaterial.SetInt("_Cull", (int)UnityEngine.Rendering.CullMode.Off);
                // Turn off depth writes
                lineMaterial.SetInt("_ZWrite", 0);
            }
        }

        // Will be called after all regular rendering is done
        public void OnRenderObject()
        {
            var renderer = GetComponent<SkinnedMeshRenderer>();
            if (renderer)
            {
                var rootBone = renderer.rootBone;
                if (rootBone)
                {
                    var bounds = renderer.localBounds;
                    DrawBounds(rootBone.localToWorldMatrix, bounds.center, bounds.size);
                }
            }
            else
            {
                foreach (var boxCollider in GetComponents<BoxCollider>())
                {
                    DrawBounds(transform.localToWorldMatrix, boxCollider.center, boxCollider.size);
                }
            }
        }

        void DrawBounds(Matrix4x4 localToWorldMatrix, Vector3 center, Vector3 size)
        {
            CreateLineMaterial();
            // Apply the line material
            lineMaterial.SetPass(0);

            GL.PushMatrix();
            // Set transformation matrix for drawing to
            // match our transform
            GL.MultMatrix(localToWorldMatrix);

            // Draw lines
            GL.Begin(GL.LINES);

            Vector3 min = center - size * 0.5f;
            Vector3 max = center + size * 0.5f;

            GL.Color(new Color(1, 1, 1, 0.2F)); GL.Vertex3(min.x, min.y, min.z); GL.Vertex3(min.x, min.y, max.z);
            GL.Color(new Color(1, 1, 1, 0.2F)); GL.Vertex3(min.x, min.y, min.z); GL.Vertex3(min.x, max.y, min.z);
            GL.Color(new Color(1, 1, 1, 0.2F)); GL.Vertex3(min.x, min.y, max.z); GL.Vertex3(min.x, max.y, max.z);
            GL.Color(new Color(1, 1, 1, 0.2F)); GL.Vertex3(min.x, max.y, min.z); GL.Vertex3(min.x, max.y, max.z);

            GL.Color(new Color(1, 1, 1, 0.2F)); GL.Vertex3(max.x, min.y, min.z); GL.Vertex3(max.x, min.y, max.z);
            GL.Color(new Color(1, 1, 1, 0.2F)); GL.Vertex3(max.x, min.y, min.z); GL.Vertex3(max.x, max.y, min.z);
            GL.Color(new Color(1, 1, 1, 0.2F)); GL.Vertex3(max.x, min.y, max.z); GL.Vertex3(max.x, max.y, max.z);
            GL.Color(new Color(1, 1, 1, 0.2F)); GL.Vertex3(max.x, max.y, min.z); GL.Vertex3(max.x, max.y, max.z);

            GL.Color(new Color(1, 1, 1, 0.2F)); GL.Vertex3(min.x, min.y, min.z); GL.Vertex3(max.x, min.y, min.z);
            GL.Color(new Color(1, 1, 1, 0.2F)); GL.Vertex3(min.x, min.y, max.z); GL.Vertex3(max.x, min.y, max.z);
            GL.Color(new Color(1, 1, 1, 0.2F)); GL.Vertex3(min.x, max.y, max.z); GL.Vertex3(max.x, max.y, max.z);
            GL.Color(new Color(1, 1, 1, 0.2F)); GL.Vertex3(min.x, max.y, min.z); GL.Vertex3(max.x, max.y, min.z);

            GL.End();
            GL.PopMatrix();
        }
    }
}
