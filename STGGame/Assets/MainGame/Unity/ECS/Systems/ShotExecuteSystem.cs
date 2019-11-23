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
            Debug.Log("fire!!!");
            //生成子弹实体
            EntityManager.GetInstance().CreateBullet(ECampType.Hero, EBulletType.AmuletRed);

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

