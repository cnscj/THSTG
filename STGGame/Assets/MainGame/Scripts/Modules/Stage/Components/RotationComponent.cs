
using System;
using Unity.Entities;
using UnityEngine;

namespace STGGame
{
    [Serializable]
    public struct Rotation : IComponentData
    {
        public Vector3 rotation;
    }
    public class RotationComponent : ComponentDataWrapper<Rotation> { }

}
