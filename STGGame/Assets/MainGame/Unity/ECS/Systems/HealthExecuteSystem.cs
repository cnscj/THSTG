using System.Collections.Generic;
using Entitas;
using UnityEngine;

namespace STGU3D
{
    public class HealthExecuteSystem : IExecuteSystem
    {
        private IGroup<GameEntity> __healthGroup;
        public HealthExecuteSystem(Contexts contexts)
        {
            __healthGroup = Contexts.sharedInstance.game.GetGroup(
                GameMatcher.AllOf(
                     GameMatcher.Health
            ));
        }

        public void Die(GameEntity entity)
        {
            Debug.Log("死了");
            entity.health.isTrueDied = true;
            //发送死亡消息
            //死亡特效啥的
            //TODO:不同类型死亡效果不一
        }

        public void Execute()
        {
            foreach (var entity in __healthGroup.GetEntities())
            {
                if (entity.hasHealth)
                {
                    //血量少于0,并且没有真正死亡
                    if (!entity.health.isTrueDied)
                    {
                        if (entity.health.blood <= 0f)
                        { 
                            if (Time.fixedTime >= entity.health.trueDeathTime)
                            {
                                Die(entity);
                            }
                        }
                    }
                }
            }
        }
    }
}

