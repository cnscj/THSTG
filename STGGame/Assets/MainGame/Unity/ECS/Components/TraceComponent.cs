using Entitas;
using UnityEngine;

namespace STGU3D
{
    [Game]
    public class TraceComponent : IComponent
    {
        public GameEntity target;                   //跟随目标

        public bool isAutoSearch;                   //是否开启自动索敌
        public float searchRadius;                  //索敌半径
        

    }
}
