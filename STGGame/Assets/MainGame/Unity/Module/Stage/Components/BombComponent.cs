
using System;
using UnityEngine;

namespace STGU3D
{
    public class BombComponent : MonoBehaviour
    {
        public int maxTimes = 3;                //最大bomb次数
        public float maxCdTime = 5.0f;          //冷却时间
        public int dyingBombUse = 2;           //决死Bomb消耗数

        public int times = 3;                   //剩余bomb次数
        public float cdTime = 0f;               //剩余冷却时间

        public bool isBomb = false;             //触发
    }
}
