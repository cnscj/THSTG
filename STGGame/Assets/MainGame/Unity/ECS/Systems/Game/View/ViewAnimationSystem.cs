using System.Collections.Generic;
using Entitas;
using UnityEngine;

namespace STGU3D
{
    //TODO:非常消耗性能
    public class ViewAnimationSystem : ReactiveSystem<GameEntity>
    {

        public ViewAnimationSystem(Contexts contexts) : base(contexts.game)
        {

        }

        protected override ICollector<GameEntity> GetTrigger(IContext<GameEntity> context)
        {
            return context.CreateCollector(
                 GameMatcher.AllOf(
                      GameMatcher.View,
                      GameMatcher.Movement
                 ));
        }

        protected override bool Filter(GameEntity entity)
        {
            return entity.hasView && entity.hasMovement;
        }

        protected override void Execute(List<GameEntity> entities)
        {
            //移动动画

            foreach (var entity in entities)
            {
                if (entity.view.view != null)
                {
                    if (((UnityView)entity.view.view).bodyCom != null)
                    {
                        var renderer = ((UnityView)entity.view.view).bodyCom.renderer as SpriteRenderer;
                        var animator = ((UnityView)entity.view.view).bodyCom.animator as Animator;
                        if (renderer && animator)
                        {
                            if (entity.movement.moveSpeed.x > 0f) //右
                            {
                                animator.SetInteger("moveSpeed", 1);
                                renderer.flipX = true;

                            }
                            else if (entity.movement.moveSpeed.x < 0f) //左
                            {
                                animator.SetInteger("moveSpeed", -1);
                                renderer.flipX = false;
                            }
                            else
                            {
                                animator.SetInteger("moveSpeed", 0);
                            }
                        }
                    }
                    
                }
            }
        }
    }
}

