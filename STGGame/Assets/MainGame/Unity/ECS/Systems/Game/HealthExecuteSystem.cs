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
            entity.health.isTrueDied = true;

            PlayerDie(entity);
            PlayerBulletDie(entity);
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

        private void PlayerDie(GameEntity entity)
        {
            if (entity.hasPlayerData)
            {
                //玩家死亡,播放特效,并移除实体
                EffectManager.GetInstance().PlayOnce("60100002", null, entity.transform.position);
                EntityManager.GetInstance().DestroyEntity(entity);

            }
        }

        private void PlayerBulletDie(GameEntity entity)
        {
            if (entity.isHeroBulletFlag)
            {
                //玩家子弹死亡
                DotweenManager.GetInstance().PlayRotatingNarrow(entity,()=>
                {
                    EntityManager.GetInstance().DestroyEntity(entity);
                });
                
            }
        }
    }
}

