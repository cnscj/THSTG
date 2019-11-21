using System.Collections.Generic;
using Entitas;
using UnityEngine;

namespace STGU3D
{
    public class ViewExecuteSystem : IExecuteSystem
    {
        public ViewExecuteSystem(Contexts contexts)
        {

        }

        public void Execute()
        {
            //移动
            var moveGroup = Contexts.sharedInstance.game.GetGroup(
                GameMatcher.AllOf(
                     GameMatcher.View,
                     GameMatcher.Transform
                ));

            foreach (var entity in moveGroup.GetEntities())
            {
                if (entity.view.viewGO)
                {
                    entity.view.viewGO.transform.localPosition = entity.transform.position;
                    entity.view.viewGO.transform.localEulerAngles = entity.transform.rotation;
                }
            }

            //移动动画
            var moveAnimGroup = Contexts.sharedInstance.game.GetGroup(
                GameMatcher.AllOf(
                     GameMatcher.View,
                     GameMatcher.Movement
                ));
            foreach (var entity in moveAnimGroup.GetEntities())
            {
                if (entity.view.animator)
                {

                    if (entity.movement.moveSpeed.x > 0f) //右
                    {
                        entity.view.viewGO.transform.localEulerAngles = new Vector3(0, 180, 0);
                        entity.view.animator.SetInteger("moveSpeed", 1);
                    }
                    else if (entity.movement.moveSpeed.x < 0f) //左
                    {
                        entity.view.viewGO.transform.localEulerAngles = new Vector3(0, 0, 0);
                        entity.view.animator.SetInteger("moveSpeed", -1);
                    }

                    if (System.Math.Abs(entity.movement.moveSpeed.x) < 0.000001f && System.Math.Abs(entity.movement.moveSpeed.y) < 0.000001f)
                    {
                        entity.view.animator.SetInteger("moveSpeed", 0);
                    }


                }
            }
        }
    }
}

