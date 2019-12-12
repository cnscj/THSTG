using System.Collections.Generic;
using Entitas;
using UnityEngine;
using XLibGame;

namespace STGU3D
{
    public class DestroyedBaseSystem : ReactiveSystem<GameEntity>
    {

        public DestroyedBaseSystem(Contexts contexts) : base(contexts.game)
        {

        }

        protected override ICollector<GameEntity> GetTrigger(IContext<GameEntity> context)
        {
            return context.CreateCollector(
                 GameMatcher.AllOf(
                      GameMatcher.Destroyed
                 ));
        }

        protected override bool Filter(GameEntity entity)
        {
            return entity.hasDestroyed;
        }

        protected override void Execute(List<GameEntity> entities)
        {
            foreach (var entity in entities)
            {
                if (!entity.destroyed.isDestroyed)
                    continue;

                entity.destroyed.isDestroyed = false;
                entity.destroyed.action?.Invoke(entity);
                entity.Destroy();
            }
        }
    }
}

