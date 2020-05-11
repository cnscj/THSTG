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
        public Rect GetRect()
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
            var rt1 = this.m_rect;
            var rt2 = other.m_rect;


            if ((rt1.yMin > rt2.yMin && rt1.yMin < rt2.yMax) ||
                (rt1.yMax > rt2.yMin && rt1.yMax < rt2.yMax))
            {
                if ((rt1.xMin > rt2.xMin && rt1.xMin < rt2.xMax) ||
                    (rt1.xMax > rt2.xMin && rt1.xMax < rt2.xMax))
                {
                    return true;
                }
            }
            if ((rt2.yMin > rt1.yMin && rt2.yMin < rt1.yMax) ||
                (rt2.yMax > rt1.yMin && rt2.yMax < rt1.yMax))
            {
                if ((rt2.xMin > rt1.xMin && rt2.xMin < rt1.xMax) ||
                    (rt2.xMax > rt1.xMin && rt2.xMax < rt1.xMax))
                {
                    return true;
                }
            }
            return false;
        }
    }
}
