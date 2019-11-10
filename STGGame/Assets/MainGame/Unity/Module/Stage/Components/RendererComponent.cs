
using UnityEngine;
namespace STGU3D
{
    public class RendererComponent : MonoBehaviour
    {
        public string spriteCode = "";

        [HideInInspector] public string curSpriteCode = "";         //当前精灵Code,更换sprite时用
        [HideInInspector] public new Renderer renderer = null;      //渲染器:可能是sprite或是mesh,或是skm
    }
}
