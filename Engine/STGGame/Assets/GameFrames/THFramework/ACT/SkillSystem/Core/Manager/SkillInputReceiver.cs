using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace THGame
{
    //指令输入,状态转移
    //长按,短按,按下,弹起
    //这里长按如果超过某个阈值应该自动触发技能
    public class SkillInputReceiver : MonoBehaviour
    {
        public class StateInfo
        {
            public bool isDown;
            public float timeStamp;
        }

        public Dictionary<IComparer, StateInfo> _keyStateDict;      //指令状态
        public Queue<IComparer> _keyCommmandQueue;                  //指令集

        public void Cast(IComparer keyCode)  
        {

        }

    }

}