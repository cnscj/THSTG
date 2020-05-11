using System;
using Entitas;
using UnityEngine;

namespace STGU3D
{
    [Game]
    public class ShotComponent : IComponent
    {
        public float interval = 0.3f;                           //射击间隔

        public float nextFireTime;                              //下次开火时间

        public bool isFiring;                                   //开火状态
    }
}
