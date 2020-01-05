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
                    var viewCtrl = ((UnityView)entity.view.view).viewCtrl;
                    if (viewCtrl != null)
                    {
                        var animatorCom = viewCtrl.animatorCom;
                        var rendererCom = viewCtrl.rendererCom;
                        if (animatorCom && rendererCom)
                        {
                            if (entity.movement.moveSpeed.x > 0f) //右
                            {
                                animatorCom.SetInteger("moveSpeed", 1);
                                rendererCom.SetFlipX(true);

                            }
                            else if (entity.movement.moveSpeed.x < 0f) //左
                            {
                                animatorCom.SetInteger("moveSpeed", -1);
                                rendererCom.SetFlipX(false);
                            }
                            else
                            {
                                animatorCom.SetInteger("moveSpeed", 0);
                            }
                        }
                    }
                    
                }
            }
        }
    }
}

