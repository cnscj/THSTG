using Entitas;
using UnityEngine;

namespace STGU3D
{
    [Game]
    public class AutoPickPropComponent : IComponent
    {
        public float boundarY = 0.6f;               //Y边界-超过这个才自动拾取,范围(0-1)
        public Vector2 typeRange = Vector2.zero;     //道具范围,在这个范围的道具才会被拾取

    }
}
