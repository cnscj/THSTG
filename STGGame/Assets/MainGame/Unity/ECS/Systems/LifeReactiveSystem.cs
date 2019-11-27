using System.Collections.Generic;
using Entitas;
using UnityEngine;

namespace STGU3D
{
    public class LifeReactiveSystem : ReactiveSystem<GameEntity>
    {
        public LifeReactiveSystem(Contexts contexts) : base(contexts.game)
        {

        }

        protected override ICollector<GameEntity> GetTrigger(IContext<GameEntity> context)
        {
            return context.CreateCollector(
                GameMatcher.AllOf(
                    GameMatcher.Life
                 )
            );
        }

        protected override bool Filter(GameEntity entity)
        {
            return entity.hasLife;
        }

        public void AddLife(GameEntity entity,int cur,int prev)
        {
            Debug.Log("奖命");
        }

        public void SubLife(GameEntity entity, int cur, int prev)
        {
            Debug.Log("减命");
        }

        protected override void Execute(List<GameEntity> entities)
        {
            foreach (var entity in entities)
            {
                if (entity.life.life > entity.life.lastLife)
                {
                    AddLife(entity, entity.life.life, entity.life.lastLife);
                }
                else if(entity.life.life < entity.life.lastLife)
                {
                    SubLife(entity, entity.life.life, entity.life.lastLife);
                }

                entity.life.lastLife = entity.life.life;
            }
        }
    }
}

