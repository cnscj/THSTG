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

        public int life;
        public int armor;
        public float bomb;
        public float speed;

        public string modelCode;
        public string wingmanCode;
        public string bulletCode;
    }

}
