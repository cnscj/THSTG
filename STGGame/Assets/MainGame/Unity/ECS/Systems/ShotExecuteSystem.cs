using System.Collections.Generic;
using Entitas;
using UnityEngine;

namespace STGU3D
{
    public class ShotExecuteSystem : IExecuteSystem
    {
        private IGroup<GameEntity> __shotGroup;
        public ShotExecuteSystem(Contexts contexts)
        {
            //移动
            __shotGroup = Contexts.sharedInstance.game.GetGroup(
                GameMatcher.AllOf(
                     GameMatcher.Shot
                ));
        }

        private void Shot(GameEntity entity)
        {
            //应该根据
            var bulletEntity = entity.shot.action?.Invoke(entity);
            
           
        }

        public void Execute()
        {
            foreach (var entity in __shotGroup.GetEntities())
            {
                if (entity.shot.isFiring)
                {
                    if (entity.shot.nextFireTime <= Time.fixedTime)
                    {
                        Shot(entity);
                        entity.shot.nextFireTime = Time.fixedTime + entity.shot.interval;
                    }
                }
            }

        }
    }
}

