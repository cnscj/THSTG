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
            var vec3 = ((other.parent == null ? Vector3.zero : other.parent.center) + other.offset) - ((this.parent == null ? Vector3.zero : this.parent.center) + offset);
            if (vec3.magnitude < other.radius + this.radius)
            {
                return true;
            }
            return false;
        }
    }
}
