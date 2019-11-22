using System.Collections.Generic;
using Entitas;
using UnityEngine;

namespace STGU3D
{
    public class BombReactiveSystem : ReactiveSystem<GameEntity>
    {
        public BombReactiveSystem(Contexts contexts) : base(contexts.game)
        {

        }

        protected override ICollector<GameEntity> GetTrigger(IContext<GameEntity> context)
        {
            return context.CreateCollector(
                GameMatcher.AllOf(GameMatcher.Bomb)
            );
        }

        protected override bool Filter(GameEntity entity)
        {
            return entity.hasBomb;
        }

        protected override void Execute(List<GameEntity> entities)
        {
            foreach (var entity in entities)
            {
                if (entity.bomb.isBombing)
                {
                    if (entity.bomb.nextBombTime <= Time.fixedTime)
                    {
                        Bome(entity);
                        entity.bomb.nextBombTime = Time.fixedTime + entity.bomb.cdTime;
                    }
                    entity.bomb.isBombing = false;
                }
            }
        }

        public void Bome(GameEntity entity)
        {
            Debug.Log("Bomb");
        }
    }
}

