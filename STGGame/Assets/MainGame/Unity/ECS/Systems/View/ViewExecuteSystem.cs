using System.Collections.Generic;
using Entitas;
using UnityEngine;

namespace STGU3D
{
    public class ViewExecuteSystem : IExecuteSystem
    {
        private IGroup<GameEntity> __moveGroup;
        private IGroup<GameEntity> __moveAnimGroup;
        public ViewExecuteSystem(Contexts contexts)
        {
            __moveGroup = Contexts.sharedInstance.game.GetGroup(
               GameMatcher.AllOf(
                    GameMatcher.View,
                    GameMatcher.Transform
               ));

            __moveAnimGroup = Contexts.sharedInstance.game.GetGroup(
                GameMatcher.AllOf(
                     GameMatcher.View,
                     GameMatcher.Transform,
                     GameMatcher.Movement
                ));
        }

        public void Execute()
        {
            //移动
            foreach (var entity in __moveGroup.GetEntities())
            {
                //存在1帧的延误
                if (entity.view.view != null)
                {
                    entity.view.view.SetPosition(entity.transform.position.x, entity.transform.position.y, entity.transform.position.z);
                    entity.view.view.SetRotation(entity.transform.rotation.x, entity.transform.rotation.y, entity.transform.rotation.z);
                }
            }

            //移动动画

            foreach (var entity in __moveAnimGroup.GetEntities())
            {
                //if (entity.view.animator)
                //{

                //    if (entity.movement.moveSpeed.x > 0f) //右
                //    {
                //        entity.view.animator.SetInteger("moveSpeed", 1);
                //        (entity.view.renderer as SpriteRenderer).flipX = true;

                //        entity.ReplaceComponent(GameComponentsLookup.Transform, entity.transform);
                //    }
                //    else if (entity.movement.moveSpeed.x < 0f) //左
                //    {
                //        entity.view.animator.SetInteger("moveSpeed", -1);
                //        (entity.view.renderer as SpriteRenderer).flipX = false;

                //        entity.ReplaceComponent(GameComponentsLookup.Transform, entity.transform);
                //    }
                //    else
                //    {
                //        entity.view.animator.SetInteger("moveSpeed", 0);
                //    }


                //}
            }
        }
    }
}

