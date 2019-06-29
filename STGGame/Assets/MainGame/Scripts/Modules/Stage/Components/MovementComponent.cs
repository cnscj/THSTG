
using System;
using Unity.Entities;
namespace STGGame
{
    [Serializable]
    public struct Movement : IComponentData
    {
        public float moveSpeed;
        public float jumpSpeed;
        public float rotateSpeed;
    }
    public class MovementComponent : ComponentDataWrapper<Movement> { }
}
