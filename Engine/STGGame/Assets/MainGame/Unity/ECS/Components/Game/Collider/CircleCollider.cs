using Entitas;
using UnityEngine;

namespace STGU3D
{
    public class CircleCollider : ColliderShape
    {
        public float radius;
        public CircleCollider()
        {
            type = ShapeType.Circle;
        }
        protected override bool OnCheck(ColliderShape shape)
        {
            var other = shape as CircleCollider;
            var vec3 = other.GetPosition() - this.GetPosition();
            if (vec3.magnitude < other.radius + this.radius)
            {
                return true;
            }
            return false;
        }
    }
}
