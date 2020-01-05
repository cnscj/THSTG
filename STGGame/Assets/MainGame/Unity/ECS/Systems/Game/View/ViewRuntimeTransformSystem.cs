using System.Collections.Generic;
using Entitas;
using UnityEngine;

namespace STGU3D
{
    //TODO:超耗性能
    public class ViewRuntimeTransformSystem : ReactiveSystem<GameEntity>
    {
        public ViewRuntimeTransformSystem(Contexts contexts) : base(contexts.game)
        {

        }

        protected override ICollector<GameEntity> GetTrigger(IContext<GameEntity> context)
        {
            return context.CreateCollector(
                 GameMatcher.AllOf(
                      GameMatcher.View,
                      GameMatcher.Transform
                 ));
            //TODO:这里会把Editor的也加进来,不应该的
        }

        protected override bool Filter(GameEntity entity)
        {
            return  entity.hasTransform && entity.hasView && !entity.view.isEditor;
        }

        protected override void Execute(List<GameEntity> entities)
        {
            //移动
            foreach (var entity in entities)
            {
                //存在1帧的延误
                if (!entity.view.isEditor)
                {
                    if (entity.view.view != null)
                    {
                        //这里Reactive和Cleanup没有先后执行顺序,会产生问题:View被Destroy了,然而这里却被执行到了
                        entity.view.view.Position = new System.Numerics.Vector3(entity.transform.position.x, entity.transform.position.y, entity.transform.position.z);
                        entity.view.view.Rotation = new System.Numerics.Vector3(entity.transform.rotation.x, entity.transform.rotation.y, entity.transform.rotation.z);
                    }
                }
            }
        }
    }
}

