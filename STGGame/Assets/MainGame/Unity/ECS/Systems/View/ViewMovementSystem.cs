using System.Collections.Generic;
using Entitas;
using UnityEngine;

namespace STGU3D
{
    public class ViewMovementSystem : ReactiveSystem<GameEntity>
    {
        public ViewMovementSystem(Contexts contexts) : base(contexts.game)
        {

        }

        protected override ICollector<GameEntity> GetTrigger(IContext<GameEntity> context)
        {
            return context.CreateCollector(
                 GameMatcher.AllOf(
                      GameMatcher.View,
                      GameMatcher.Transform
                 ));
        }

        protected override bool Filter(GameEntity entity)
        {
            return entity.hasView && entity.hasTransform;
        }

        protected override void Execute(List<GameEntity> entities)
        {
            //移动
            foreach (var entity in entities)
            {
                //存在1帧的延误
                if (entity.view.view != null)
                {
                    //这里Reactive和Cleanup没有先后执行顺序,会产生问题:View被Destroy了,然而这里却被执行到了
                    entity.view.view.SetPosition(entity.transform.position.x, entity.transform.position.y, entity.transform.position.z);
                    entity.view.view.SetRotation(entity.transform.rotation.x, entity.transform.rotation.y, entity.transform.rotation.z);
                }
            }
        }
    }
}

