using Entitas;
using UnityEngine;

namespace STGU3D
{
    [Game]
    public class ViewComponent : IComponent
    {
        [System.Obsolete]
        public string viewCode;

        public GameObject viewGO;
        public Renderer renderer;
        public Animator animator;
    }

}
