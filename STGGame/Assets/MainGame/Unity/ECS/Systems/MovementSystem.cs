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
            // 该控制器只关心含有DebugMessage组件的实体 
            return context.CreateCollector(
                GameMatcher.AllOf(GameMatcher.Movement)
            );
        }

        protected override bool Filter(GameEntity entity)
        {
            // 只有hasDebugMessage为true的实体才会触发下面的Execute函数
            return entity.hasMovement &&
                    entity.hasPosition;
        }

        protected override void Execute(List<GameEntity> entities)
        {
            // 满足GetTrigger和Filter的实体保存在entities列表里
            foreach (var e in entities)
            {
                e.ReplacePosition(e.position.position + e.movement.moveSpeed);

            }
        }
    }
}

