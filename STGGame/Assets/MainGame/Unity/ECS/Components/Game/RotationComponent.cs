using Entitas;
using UnityEngine;

namespace STGU3D
{
    [Game]
    public class RotationComponent : IComponent
    {
        public RotationComponent parent;
        public Vector3 rotation;
        public Vector3 localRotation;
    }
}

