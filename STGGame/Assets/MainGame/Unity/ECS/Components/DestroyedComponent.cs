﻿using System;
using Entitas;

namespace STGU3D
{
    [Game, Input, UI]
    public class DestroyedComponent : IComponent
    {
        public Action<GameEntity> action;   //回调
        public int what;                    //移除原因        
        public bool isDestroyed;            //是否移除
        public float delayTime;             //延迟死亡
    }
}
