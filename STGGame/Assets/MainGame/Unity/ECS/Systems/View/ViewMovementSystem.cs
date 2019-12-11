using System.Collections.Generic;
using Entitas;
using UnityEngine;

namespace STGU3D
{
    public class ViewMovementSystem : IExecuteSystem
    {
        private IGroup<GameEntity> __viewMovementGroup;
        public ViewMovementSystem(Contexts contexts)
        {
            __viewMovementGroup = Contexts.sharedInstance.game.GetGroup(
                GameMatcher.AllOf(
                        GameMatcher.View,
                        GameMatcher.Transform
            ));
        }

        public void Execute()
        {
            //移动
            foreach (var entity in __viewMovementGroup.GetEntities())
            {
                //存在1帧的延误
                if (entity.view.view != null)
                {
                    entity.view.view.SetPosition(entity.transform.position.x, entity.transform.position.y, entity.transform.position.z);
                    entity.view.view.SetRotation(entity.transform.rotation.x, entity.transform.rotation.y, entity.transform.rotation.z);
                }
            }
        }
    }
}

