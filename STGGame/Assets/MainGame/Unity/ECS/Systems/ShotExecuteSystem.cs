using System.Collections.Generic;
using Entitas;
using UnityEngine;

namespace STGU3D
{
    public class ShotExecuteSystem : IExecuteSystem
    {
        public ShotExecuteSystem(Contexts contexts)
        {

        }

        private void Shot(GameEntity entity)
        {
            Debug.Log("fire!!!");
            //生成子弹实体
        }

        public void Execute()
        {
            //移动
            var shotGroup = Contexts.sharedInstance.game.GetGroup(
                GameMatcher.AllOf(
                     GameMatcher.Shot
                ));

            foreach (var entity in shotGroup.GetEntities())
            {
                if (entity.shot.isFire)
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

