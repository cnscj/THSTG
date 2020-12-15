using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace THGame
{
    public class SkillController : MonoBehaviour
    {
        public SkillData skillData;
 
        private void Start()
        {
            
        }

        //触发技能
        public void Cast(int skillId)
        {
            //判断技能是否在CD,不在CD的话就触发技能

        }
    }

}
