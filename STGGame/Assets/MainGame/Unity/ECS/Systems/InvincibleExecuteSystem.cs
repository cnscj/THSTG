using System.Collections.Generic;
using Entitas;
using UnityEngine;

namespace STGU3D
{
    public class InvincibleExecuteSystem : IExecuteSystem
    {
        private IGroup<GameEntity> __invincibleGroup;
        public InvincibleExecuteSystem(Contexts contexts)
        {
            __invincibleGroup = Contexts.sharedInstance.game.GetGroup(
                GameMatcher.AllOf(
                     GameMatcher.Invincible
            ));
        }

        public void Start()
        {
            Debug.Log("无敌启动");
        }

        public void End()
        {
            Debug.Log("无敌结束");
        }

        public void Execute()
        {
            foreach (var entity in __invincibleGroup.GetEntities())
            {
                if (entity.invincible.time > 0)
                {
                    entity.invincible.time -= Time.fixedTime;
                    if (entity.invincible.isInvincible)
                    {
                        if (entity.invincible.time <= 0)
                        {
                            End();
                            entity.invincible.time = 0;
                        }
                    }
                    else
                    {
                        Start();
                    }
                }

                entity.invincible.isInvincible = (entity.invincible.time > 0);
            }
        }
    }
}

