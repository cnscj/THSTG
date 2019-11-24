using System;
using Entitas;
using UnityEngine;

namespace STGU3D
{
    [Game]
    public class PlayerDataComponent : IComponent
    {
        public EPlayerType playerType;
        public EHeroType heroType;

        public float moveSpeed;
    }

}
