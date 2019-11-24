using Entitas;
using UnityEngine;

namespace STGU3D
{
    [Game]
    public class TransformComponent : IComponent
    {
        public TransformComponent parent;
        public Vector3 position;
        public Vector3 rotation;


    }
}

