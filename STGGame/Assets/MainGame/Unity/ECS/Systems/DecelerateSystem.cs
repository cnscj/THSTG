using System.Collections.Generic;
using Entitas;
using UnityEngine;

namespace STGU3D
{
    public class DecelerateSystem : IExecuteSystem
    {
        private IGroup<GameEntity> __decelerateGroup;
        public DecelerateSystem(Contexts contexts)
        {
            __decelerateGroup = Contexts.sharedInstance.game.GetGroup(
               GameMatcher.AllOf(
                    GameMatcher.Decelerate,
                    GameMatcher.Movement
               ));
        }

        public void Execute()
        {
            //移动
            foreach (var entity in __decelerateGroup.GetEntities())
            {
                if (entity.decelerate.isDecelerating)
                {
                    entity.movement.moveSpeed *= entity.decelerate.speedRate;
                }
                
            }
        }
    }
}

