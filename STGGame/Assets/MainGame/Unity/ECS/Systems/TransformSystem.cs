using System.Collections.Generic;
using Entitas;
using UnityEngine;

namespace STGU3D
{
    public class TransformSystem : ReactiveSystem<GameEntity>
    {
        public TransformSystem(Contexts contexts) : base(contexts.game)
        {
        }

        protected override ICollector<GameEntity> GetTrigger(IContext<GameEntity> context)
        {
            return context.CreateCollector(
                GameMatcher.AllOf(GameMatcher.Transform)
            );
        }

        protected override bool Filter(GameEntity entity)
        {
            return entity.hasTransform;
        }

        protected override void Execute(List<GameEntity> entities)
        {
            foreach (var e in entities)
            {
                
            }
        }
    }
}

