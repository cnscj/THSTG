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
            //圆和矩形的碰撞

            return false;
        }



        ///
        protected Vector3 GetPosition()
        {
           return ((parent == null ? Vector3.zero : parent.center) + offset);
        }
        protected abstract bool OnCheck(ColliderShape shape);

    }
}
