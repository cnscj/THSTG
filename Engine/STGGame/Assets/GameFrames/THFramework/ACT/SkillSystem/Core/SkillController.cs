using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace THGame
{
    public class SkillController : MonoBehaviour
    {
        public Animator animator;       //动作状态机
        public SkillData data;          //技能数据

        public void Awake()
        {
            Init();
        }

        //施展技能
        public void Cast(string skill)
        {
            
        }

        //中断技能
        public void Break()
        {

        }

        public void Load()
        {

        }

        //初始化,包括加载
        private void Init()
        {
            animator = animator ?? GetComponentInChildren<Animator>();
        }

        //////////////////////////////

        private void PlayAction(string name)
        {
            if (animator = null)
                return;

        }

        private void PlayEffect(string name)
        {

        }
    }

}
