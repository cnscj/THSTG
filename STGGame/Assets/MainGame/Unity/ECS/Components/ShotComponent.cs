using Entitas;
using UnityEngine;

namespace STGU3D
{
    [Game]
    public class ShotComponent : IComponent
    {
        public GameObject bullet;           //子弹实体
        public float interval = 0.3f;       //射击间隔

        public float nextFireTime;          //下次开火时间

        public bool isFire = false;         //开火状态
    }
}
