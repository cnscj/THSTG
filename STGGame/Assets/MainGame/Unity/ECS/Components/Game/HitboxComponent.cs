using Entitas;
using UnityEngine;

namespace STGU3D
{
    [Game]
    public class HitboxComponent : IComponent
    {
        public Vector2 box;
        public bool isUseRect;
    }
}
