using UnityEngine;

namespace THGame
{
    [System.Serializable]
    public class SkillCastInfo
    {
        //TODO:时长应该取读配置,或按实际情况来
        public SkillInputType inputType;            //输入类型

        public SkillCdTrigger cdTrigger;            //cd触发类型
        public float timeoutToCd;                   //超时进入CD

        
        public float cdTime = 1f;                   //一次冷却CD时长
        public int maxTimes = 1;                    //最大使用次数
        public float maxDuration = 90;              //技能最大持续时间,时间到将进入冷却
        public float interruptedPrec = 20;          //抗打断能力


        public SkillFloatCondition[] conditions;    //触发条件




    }

}
