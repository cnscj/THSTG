using UnityEngine;

namespace THGame
{
    //可能是技能触发,也可能是受击,冲刺等
    //触发技能,如果X秒没有触发下一个,则可能直接进入冷却

    //
    public class SkillCastTrigger : MonoBehaviour
    {
        //释放技能
        public void Cast(int skillId)
        {

        }

        //打断释放
        public void Stop()
        {

        }

    }
}