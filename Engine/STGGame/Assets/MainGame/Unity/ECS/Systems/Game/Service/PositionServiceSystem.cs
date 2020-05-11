using System.Collections.Generic;
using Entitas;
using UnityEngine;

namespace STGU3D
{
    public class PositionServiceSystem : ReactiveSystem<GameEntity>
    {
        public PositionServiceSystem(Contexts contexts) : base(contexts.game)
        {
        }

        protected override ICollector<GameEntity> GetTrigger(IContext<GameEntity> context)
        {
            return context.CreateCollector(
                GameMatcher.AllOf(GameMatcher.Position).NoneOf(GameMatcher.EditorEntity)
            );
        }

        protected override bool Filter(GameEntity entity)
        {
            return entity.hasPosition;
        }

        protected override void Execute(List<GameEntity> entities)
        {
            //子变化,rotation会被父影响
            foreach (var e in entities)
            {
                if (e.position.parent != null)
                {
                    e.position.position = e.position.parent.position + e.position.localPosition;
                }
                else
                {
                    e.position.position = e.position.localPosition;
                }
            }
        }
    }
}

