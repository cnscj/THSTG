using Entitas;
using UnityEngine;

namespace STGU3D
{
    [Game]
    public class BoundaryLimitationComponent : IComponent
    {
        public Rect movableArea;
    }
}
