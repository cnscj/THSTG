
using System;
using Unity.Entities;
using UnityEngine;

namespace STGGame
{
    [Serializable]
    public struct Position : IComponentData
    {
        public Vector3 position;
    }
    public class PositionComponent : ComponentDataWrapper<Position> { }
}
