using Entitas;
using UnityEngine;

namespace STGU3D
{
    public class BoxCollider : ColliderShape
    {
        public Vector3 box = Vector3.zero;
        private Rect m_rect = Rect.zero;

        public BoxCollider()
        {
            type = ShapeType.Box;
        }
        protected Rect GetRect()
        {
            var pos = GetPosition();
            m_rect.x = pos.x;
            m_rect.y = pos.y;
            m_rect.width = box.x;
            m_rect.height = box.y;

            return m_rect;
        }
        protected override bool OnCheck(ColliderShape shape)
        {
            var other = shape as BoxCollider;
            //TODO:矩形相交算法
            return false;
        }
    }
}
