using Entitas;
using UnityEngine;

namespace STGU3D
{
    public abstract class ColliderShape
    {
        public enum ShapeType
        {
            Box,
            Circle,
        };
        
        public ShapeType type { get; protected set; }
        public Vector3 offset = Vector3.zero;
        public string tag;
        public ColliderObject parent;

        public bool Check(ColliderShape other)
        {
            if (this.type == other.type)
            {
                return OnCheck(other);
            }
            return OnCheck(this, other);
        }

        //公共的
        public bool OnCheck(ColliderShape a, ColliderShape b)
        {
            if (a.type == ShapeType.Box && b.type == ShapeType.Circle)
                return CheckBoxCircle(a as BoxCollider, b as CircleCollider);
            else if (a.type == ShapeType.Circle && b.type == ShapeType.Box)
                return CheckBoxCircle(b as BoxCollider, a as CircleCollider);


            return false;
        }

        private bool CheckBoxCircle(BoxCollider a, CircleCollider b)
        {
            // 圆与矩形的碰撞检测(适用于矩形无旋转的情况下)
            float radius = b.radius;

            float width = a.box.x;
            float height = a.box.y;
            // 圆心与矩形中心的相对距离
            float relativeX = b.GetPosition().x - (a.GetPosition().x + width * 0.5f);
            float relativeY = b.GetPosition().y - (a.GetPosition().y + height * 0.5f);

            float dx = Mathf.Min(relativeX, (width * 0.5f));
            float dx1 = Mathf.Max(dx, (-width * 0.5f));
            float dy = Mathf.Min(relativeY, (height * 0.5f));
            float dy1 = Mathf.Max(dy, (-height * 0.5f));

            bool isCollision = (dx1 - relativeX) * (dx1 - relativeX) + (dy1 - relativeY) * (dy1 - relativeY) <= radius * radius;

            return isCollision;
        }


        ///
        public Vector3 GetPosition()
        {
           return ((parent == null ? Vector3.zero : parent.center) + offset);
        }
        protected abstract bool OnCheck(ColliderShape shape);

    }
}
