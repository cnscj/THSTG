using System.Collections.Generic;
using Entitas;
using UnityEngine;

namespace STGU3D
{
    public class ColliderContent
    {
        public ColliderObject owner;
        public ColliderObject other;
        public Dictionary<ColliderShape,List<ColliderShape>> collisions;
    }
}
