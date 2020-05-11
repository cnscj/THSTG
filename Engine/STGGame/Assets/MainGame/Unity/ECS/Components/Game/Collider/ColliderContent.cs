using System.Collections.Generic;
using Entitas;
using UnityEngine;

namespace STGU3D
{
    public class ColliderContent
    {
        public ColliderObject owner;
        public Dictionary<ColliderObject, List<ColliderShape>> collisions;

        public void Clear()
        {
            owner = null;
            collisions?.Clear();
        }
    }
}
