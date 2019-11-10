﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace STGU3D
{
    public class InvincibleComponent : MonoBehaviour
    {
        public float maxTime = 3.0f;        //最大无敌保护时间(s
        public float time = 0f;             //保护剩余时间
    }

}
