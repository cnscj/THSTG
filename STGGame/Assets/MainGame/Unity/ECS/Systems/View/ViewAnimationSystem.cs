using System.Collections.Generic;
using Entitas;
using UnityEngine;

namespace STGU3D
{
    public class ViewAnimationSystem : IExecuteSystem
    {
        private IGroup<GameEntity> __moveAnimGroup;
        public ViewAnimationSystem(Contexts contexts)
        {
            __moveAnimGroup = Contexts.sharedInstance.game.GetGroup(
                 GameMatcher.AllOf(
                      GameMatcher.View,
                      GameMatcher.Transform,
                      GameMatcher.Movement
                 ));
        }

        public void Execute()
        {
            //移动动画

            foreach (var entity in __moveAnimGroup.GetEntities())
            {
                if (entity.view.view != null)
                {
                    if (((UnityView)entity.view.view).body != null)
                    {
                        var renderer = ((UnityView)entity.view.view).body.renderer as SpriteRenderer;
                        var animator = ((UnityView)entity.view.view).body.animator as Animator;
                        if (renderer && animator)
                        {
                            if (entity.movement.moveSpeed.x > 0f) //右
                            {
                                animator.SetInteger("moveSpeed", 1);
                                renderer.flipX = true;

                                entity.ReplaceComponent(GameComponentsLookup.Transform, entity.transform);
                            }
                            else if (entity.movement.moveSpeed.x < 0f) //左
                            {
                                animator.SetInteger("moveSpeed", -1);
                                renderer.flipX = false;

                                entity.ReplaceComponent(GameComponentsLookup.Transform, entity.transform);
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

