using System.Collections.Generic;
using Entitas;
using UnityEngine;

namespace STGU3D
{
    public class ViewSystem : ReactiveSystem<GameEntity>
    {
        public ViewSystem(Contexts contexts) : base(contexts.game)
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
            foreach (var e in entities)
            {
                //绑定GameObject
                if (e.view.viewGO != null)
                {
                    GameObject.Destroy(e.view.viewGO);
                    e.view.renderer = null;
                    e.view.animator = null;
                }
                GameObject view = new GameObject("view");



                e.view.viewGO = view;
            }
        }
    }
}

