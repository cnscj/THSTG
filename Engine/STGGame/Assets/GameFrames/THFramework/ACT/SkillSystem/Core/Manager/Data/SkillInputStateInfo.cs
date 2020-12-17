using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace THGame
{
    public class SkillInputStateInfo
    {
        public IComparable keyCode;
        public int state;
        public float timeStamp;

        public Action onKeyUp;
        public Action onKeyDown;
        public Action onShotAction;
        public Action onLongAction;
        public bool callbackEnabled;
    }
}

