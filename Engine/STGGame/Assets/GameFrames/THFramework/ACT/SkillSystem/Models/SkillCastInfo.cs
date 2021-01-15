using UnityEngine;

namespace THGame
{
    [System.Serializable]
    public class SkillCastInfo
    {
        public SkillInputType inputType;            //输入类型
        public float castResponseTime;              //长短按响应时间(只限长短按
        public float autoReleaseTime = -1;          //自动释放时间,超过这个值自动释放(

        //TODO:时长应该取读配置,或按实际情况来
        public SkillCdTrigger cdTrigger;            //cd触发类型
        public float cdTime = 1f;                   //一次冷却CD时长
        public int maxTimes = 1;                    //最大使用次数
        public float timeoutToCd;                   //超时进入CD的时间

        public SkillTriggableConditions conditions;   //触发条件

    }
}
