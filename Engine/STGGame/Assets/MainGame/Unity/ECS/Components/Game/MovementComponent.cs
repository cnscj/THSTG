using System;
using Entitas;
using UnityEngine;

namespace STGU3D
{
    [Game]
    public class MovementComponent : IComponent
    {
        public Vector3 moveSpeed = Vector3.zero;
        public Vector3 rotationSpeed = Vector3.zero;
    }
}
