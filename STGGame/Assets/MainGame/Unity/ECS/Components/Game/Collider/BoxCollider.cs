using Entitas;
using UnityEngine;

namespace STGU3D
{
    public class BoxCollider : ColliderShape
    {
        public Vector2 box;
        public BoxCollider()
        {
            type = ShapeType.Box;
        }
        protected override bool OnCheck(ColliderShape shape)
        {

            return false;
        }
    }
}
