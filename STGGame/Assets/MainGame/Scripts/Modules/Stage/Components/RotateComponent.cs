﻿
using UnityEngine;
using Unity.Entities;
namespace STGGame
{
    [RequireComponent(typeof(GameObjectEntity))]
    public class RotateComponent : MonoBehaviour
    {
        //旋转速度
        public int speed;
    }

}
