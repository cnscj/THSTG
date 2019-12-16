using Entitas;
using UnityEngine;

namespace STGU3D
{
    [Game]
    [System.Serializable]
    public class TransformComponent : IComponent
    {
        public TransformComponent parent;
        public Vector3 position;
        public Vector3 rotation;

        public Vector3 localPosition;
        public Vector3 localRotation;

    }
}

