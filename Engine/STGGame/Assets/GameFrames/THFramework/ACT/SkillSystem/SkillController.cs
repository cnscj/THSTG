using UnityEngine;

namespace THGame
{
    public class SkillController : MonoBehaviour
    {
        public SkillData skillData;                 //技能数据
        public SkillBaseBehaviour skillBehaviour;   //技能行为 
        public SkillTrackPlayer skillTrackPlayer;   //轨道播放器

        //控制行为动作,碰撞检测应该是被动式的
        public void Start()
        {
            Init();
        }

        public void Init()
        {
            skillBehaviour.BuildStateMachine(skillData.skillBeans);
            skillBehaviour.onStateChanged = OnSkillStateChanged;

        }

        //触发技能
        public void Cast(int skillId)
        {
            //判断技能是否在CD,不在CD的话就触发技能

        }

        //状态转移,播放对应的动作等
        protected void OnSkillStateChanged(SkillFSMState fromState, SkillFSMState toState)
        {

        }
    }

}
