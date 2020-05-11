
using System;
using Entitas;

namespace STGU3D
{
    [Game]
    public class ColliderComponent : IComponent
    {
        public long tag;
        public long mask = long.MaxValue;
        public ColliderObject obj = new ColliderObject();
    }
}
