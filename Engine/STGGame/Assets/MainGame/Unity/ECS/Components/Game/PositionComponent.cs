using Entitas;
using UnityEngine;

namespace STGU3D
{
    [Game]
    public class PositionComponent : IComponent
    {
        public PositionComponent parent;
        public Vector3 position;
        public Vector3 localPosition;
    }
}

