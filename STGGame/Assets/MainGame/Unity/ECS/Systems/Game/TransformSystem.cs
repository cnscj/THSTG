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
                //子变化,rotation会被父影响
                //应该取矩阵取运算
                if (e.transform.parent != null)
                {
                    e.transform.position = e.transform.parent.position + e.transform.localPosition;
                    e.transform.rotation = e.transform.parent.rotation + e.transform.localRotation;
                }
            }
        }
    }
}

