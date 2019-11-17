using Entitas;
using UnityEngine;

namespace STGU3D
{
    [Game]
    public class ViewComponent : IComponent
    {
        public GameObject view;

        public Renderer renderer;
        public Animator animator;
    }

}
