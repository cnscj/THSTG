using System.Collections.Generic;
using Entitas;
using UnityEngine;

namespace STGU3D
{
    public class ViewReactiveSystem : ReactiveSystem<GameEntity>
    {
        public ViewReactiveSystem(Contexts contexts) : base(contexts.game)
        {

        }

        protected override ICollector<GameEntity> GetTrigger(IContext<GameEntity> context)
        {
            return context.CreateCollector(
                GameMatcher.AllOf(GameMatcher.View)
            );
        }

        protected override bool Filter(GameEntity entity)
        {
            return entity.hasView;
        }

        protected override void Execute(List<GameEntity> entities)
        {
            foreach (var entity in entities)
            {
                if (entity.view.viewGO)
                {
                    var viewNode = entity.view.viewGO.gameObject;
                    entity.view.animator = viewNode.GetComponentInChildren<Animator>();
                    entity.view.renderer = viewNode.GetComponentInChildren<Renderer>();
                }
            }
        }
    }
}

