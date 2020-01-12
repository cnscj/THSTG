using System.Collections.Generic;
using DG.Tweening;
using Entitas;
using UnityEngine;

namespace STGU3D
{
    public class HealthReactiveSystem : ReactiveSystem<GameEntity>
    {
        public HealthReactiveSystem(Contexts contexts) : base(contexts.game)
        {

        }

        protected override ICollector<GameEntity> GetTrigger(IContext<GameEntity> context)
        {
            return context.CreateCollector(
                GameMatcher.AllOf(
                    GameMatcher.Health
                 )
            );
        }

        protected override bool Filter(GameEntity entity)
        {
            return entity.hasHealth;
        }
        
        public void DyingRevive(GameEntity entity)
        {
            Debug.Log("决死复活-不会消耗生命值?");
            entity.health.isTrueDied = false;
            entity.health.trueDeathTime = -1f;
        }

        public void Revive(GameEntity entity)
        {
            //复活
            Debug.Log("复活");
            DyingRevive(entity);
            if (entity.hasLife)
            {
                var lifeCom = entity.life;
                lifeCom.life--;
                entity.ReplaceComponent(GameComponentsLookup.Life, lifeCom);
            }
            
        }

        public void Dying(GameEntity entity)
        {
            //将死
            Debug.Log("快要死了");
            entity.health.blood = 0;
            entity.health.trueDeathTime = Time.fixedTime + entity.health.maxNearDeathTime;
        }

        //治疗
        public void Treat(GameEntity entity)
        {
            Debug.Log("治疗");
        }

        //受伤
        public void Hurt(GameEntity entity)
        {
            MobHurt(entity);
            BossHurt(entity);
        }

        protected override void Execute(List<GameEntity> entities)
        {
            foreach (var entity in entities)
            {
                if (entity.health.isTrueDied)
                {
                    if (entity.health.blood > 0)
                    {
                        if (entity.hasLife)
                        {
                            if (entity.life.life > 0)
                            {
                                Revive(entity);
                            }
                        }
                        else
                        {
                            Revive(entity);
                        }
                    }
                }
                else
                {
                    if (entity.health.blood <= 0)
                    {
                        Dying(entity);
                    }
                    else
                    {
                        //频死复活
                        if (entity.health.trueDeathTime > -1f)
                        {
                            if (Time.fixedTime < entity.health.trueDeathTime)
                            {
                                DyingRevive(entity);
                            }
                        }
                    }
                }

                if (entity.health.blood > entity.health.maxBlood) entity.health.blood = entity.health.maxBlood;

                //
                if (entity.health.blood - entity.health.prevBlood > 0)
                {
                    //复活不算治疗
                    //if (entity.health.prevBlood > 0)
                    //{
                        Treat(entity);
                    //}
                }
                else if (entity.health.blood - entity.health.prevBlood < 0)
                {
                    //死亡不算受伤
                    //if (entity.health.blood > 0)
                    //{
                        Hurt(entity);
                    //}
                }
                entity.health.prevBlood = entity.health.blood;

            }
        }

        //////////////
        public void MobHurt(GameEntity entity)
        {
            if (entity.hasMobData)
            {
                //只保留1个
                string sUID = string.Format("flash_%s", entity.creationIndex);
                DotweenManager.GetInstance().Kill(sUID);
                DotweenManager.GetInstance().PlayFlash(entity, 8, 0.5f).SetId(sUID);
                
            }
        }

        public void BossHurt(GameEntity entity)
        {
            if (entity.hasBossData)
            {
                DotweenManager.GetInstance().PlayFlash(entity, 10, 2);
            }
        }
    }
}

