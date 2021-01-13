using UnityEngine;

namespace THGame
{
    public class SkillController : MonoBehaviour
    {
        public SkillData skillData;                 //技能数据
        public SkillTrigger skillTrigger;           //技能行为触发器
        public SkillTrackPlayer skillTrackPlayer;   //轨道播放器

        //控制行为动作,碰撞检测应该是被动式的
        public void Start()
        {
            Init();
        }

        public void Init()
        {

        }


        //触发入口
        public void InputKey(SkillInputStateInfo stateInfo, SkillInputType inputType)
        {
            SkillType skillCastType = (SkillType)stateInfo.keyCode;
            Debug.LogFormat("{0}:{1}", skillCastType, inputType);
            //调用触发器,如果在某个时间段内进行操作才能进入相应状态,否则超过则进入默认退出状态(应该是等动画自然状态回归)
            //从对应太进入,到自然释放过程会有一个空窗期(冷却期(_空窗期)_触发期s(_空窗期)_回归期,回归期计时可能从冷却期结束开始计时


            //TODO:同一个技能会有多个阶段,并且可能会按照不同的技能时长决定长短按
            //获取触发的技能类型 
            //获取对应skillId
            //获取skillInfo,
            //检查CD
        }

    }

}
