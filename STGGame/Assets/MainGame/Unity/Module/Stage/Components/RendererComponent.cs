
using UnityEngine;
namespace STGU3D
{
    public class RendererComponent : MonoBehaviour
    {
        public string rendererCode = "";

        [HideInInspector] public string curRendererCode = "";         //当前精灵Code,更换sprite时用
        [HideInInspector] public new Renderer renderer = null;      //渲染器:可能是sprite或是mesh,或是skm
    }
}
