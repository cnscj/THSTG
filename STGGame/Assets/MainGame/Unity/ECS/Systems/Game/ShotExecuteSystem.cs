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

        public void Shot(GameEntity entity)
        {
            //应该根据
            PlayerShot(entity);
            
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

        private void PlayerShot(GameEntity shotEntity)
        {
            if (shotEntity.hasPlayerData)
            {
                var bulletFactory = EntityManager.GetInstance().GetOrNewEntityFactory(EEntityType.Bullet).AsBullet();
                var bulletEntity = bulletFactory.CreateBullet(ECampType.Hero, shotEntity.entityData.entityData["bulletCode"]);
                bulletEntity.transform.localPosition = shotEntity.transform.localPosition;
                bulletEntity.ReplaceComponent(GameComponentsLookup.Transform, bulletEntity.transform);
            }
        }
    }
}

