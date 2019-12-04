using Entitas;
using UnityEngine;

namespace STGU3D
{
    [Game]
    public class TraceComponent : IComponent
    {
        public GameEntity target;                   //跟随目标

        public float searchRadius;                  //索敌半径

    }
}
