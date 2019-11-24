using System.Collections.Generic;
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
                GameMatcher.AllOf(GameMatcher.Health)
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
            entity.health.life--;
        }

        public void Dying(GameEntity entity)
        {
            //将死
            Debug.Log("快要死了");
            entity.health.blood = 0;
            entity.health.trueDeathTime = Time.fixedTime + entity.health.maxNearDeathTime;
        }

        protected override void Execute(List<GameEntity> entities)
        {
            foreach (var entity in entities)
            {
                if (entity.health.isTrueDied)
                {
                    if (entity.health.blood > 0f)
                    {
                        if (entity.health.life != 0)
                        {
                            Revive(entity);
                        }
                    }
                }
                else
                {
                    if (entity.health.blood <= 0f)
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

            }
        }
    }
}

