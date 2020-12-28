using UnityEngine;

namespace THGame
{
    //可能是技能触发,也可能是受击,冲刺等
    //触发技能,如果X秒没有触发下一个,则可能直接进入冷却

    //一个状态机,有程序自行编写
    public class SkillCastTrigger : MonoBehaviour
    {

        //释放技能
        public void Cast(int skillId)
        {
            //如果在前摇时间被打断技能,则释放无效
            //有些属性可以增加抗打断的效果
        }

        private void Update()
        {
            
        }

        //打断释放
        public void Stop()
        {

        }

    }
}