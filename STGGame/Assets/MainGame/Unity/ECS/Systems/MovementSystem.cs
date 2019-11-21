using System.Collections.Generic;
using Entitas;
using UnityEngine;

namespace STGU3D
{
    public class MovementSystem : ReactiveSystem<GameEntity>
    {
        public MovementSystem(Contexts contexts) : base(contexts.game)
        {
        }

        protected override ICollector<GameEntity> GetTrigger(IContext<GameEntity> context)
        {
            return context.CreateCollector(
                GameMatcher.AllOf(GameMatcher.Movement, GameMatcher.Transform)
            );
        }

        protected override bool Filter(GameEntity entity)
        {
            return entity.hasMovement && entity.hasTransform;
        }

        protected override void Execute(List<GameEntity> entities)
        {
            // 满足GetTrigger和Filter的实体保存在entities列表里
            foreach (var e in entities)
            {
                e.ReplaceTransform(e.transform.position + e.movement.moveSpeed * Time.deltaTime, e.transform.rotation + e.movement.rotationSpeed * Time.deltaTime);
            }
        }
    }
}

