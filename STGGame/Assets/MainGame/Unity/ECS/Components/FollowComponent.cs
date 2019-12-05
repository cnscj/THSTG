﻿using Entitas;
using UnityEngine;

namespace STGU3D
{
    [Game]
    public class FollowComponent : IComponent
    {
        public Vector3 destination;                 //跟随目标点
        public float followRadius;                  //跟随半径
        public float followSpeed = 1f;              //跟随速度

        public bool isFollowing;                    //是否正在跟随
    }
}