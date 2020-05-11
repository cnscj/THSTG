using System.Collections.Generic;
using Entitas;
using UnityEngine;

namespace STGU3D
{
    public class RotationServiceSystem : ReactiveSystem<GameEntity>
    {
        public RotationServiceSystem(Contexts contexts) : base(contexts.game)
        {
        }

        protected override ICollector<GameEntity> GetTrigger(IContext<GameEntity> context)
        {
            return context.CreateCollector(
                GameMatcher.AllOf(GameMatcher.Rotation).NoneOf(GameMatcher.EditorEntity)
            );
        }

        protected override bool Filter(GameEntity entity)
        {
            return entity.hasRotation;
        }

        protected override void Execute(List<GameEntity> entities)
        {
            foreach (var e in entities)
            {
                if (e.rotation.parent != null)
                {
                    e.rotation.rotation = e.rotation.parent.rotation + e.rotation.localRotation;
                }
                else
                {
                    e.rotation.rotation = e.rotation.localRotation;
                }
            }
        }
    }
}

