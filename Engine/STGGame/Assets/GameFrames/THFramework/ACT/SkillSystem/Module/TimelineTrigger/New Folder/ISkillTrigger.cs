using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace THGame
{
    public interface ISkillTrigger
    {
        void Start();                   //开始

        void Update(int tickTime);      //更新

        void End();                     //结束

        void Reset();                   //重置
    }
}
