using Entitas;
using UnityEngine;

namespace STGU3D
{
    [Game]
    public class ViewComponent : IComponent
    {
        public GameObject viewGO;
        public Renderer renderer;
        public Animator animator;
        public Collider collider;
    }

}
