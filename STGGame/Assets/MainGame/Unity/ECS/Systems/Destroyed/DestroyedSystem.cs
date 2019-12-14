using System.Collections.Generic;
using Entitas;
using UnityEngine;
using XLibGame;

namespace STGU3D
{
    public class DestroyedSystem : ICleanupSystem
    {
        private IGroup<GameEntity> __destroyedGroup;
        private IGroup<GameEntity> __viewGroup;
        private IGroup<GameEntity> __heroBulletGroup;
        private IGroup<GameEntity> __entityBulletGroup;
        public DestroyedSystem(Contexts contexts)
        {
            __destroyedGroup = Contexts.sharedInstance.game.GetGroup(
                GameMatcher.AllOf(
                     GameMatcher.Destroyed
             ));

            __viewGroup = Contexts.sharedInstance.game.GetGroup(
                GameMatcher.AllOf(
                     GameMatcher.Destroyed,
                     GameMatcher.View
             ));

            __heroBulletGroup = Contexts.sharedInstance.game.GetGroup(
                GameMatcher.AllOf(
                     GameMatcher.Destroyed,
                     GameMatcher.HeroBulletFlag
            ));

            __entityBulletGroup = Contexts.sharedInstance.game.GetGroup(
                GameMatcher.AllOf(
                     GameMatcher.Destroyed,
                     GameMatcher.EntityBulletFlag
            ));

        }

        public void Cleanup()
        {

            foreach (var entity in __viewGroup.GetEntities())
            {
                //移除View
                if (!entity.destroyed.isDestroyed)
                    continue;

                if (entity.view.view != null)
                {
                    entity.view.view.Clear();
                    entity.view.view = null;
                }
            }

            foreach (var entity in __heroBulletGroup.GetEntities())
            {
                if (!entity.destroyed.isDestroyed)
                    continue;

            }


            foreach (var entity in __entityBulletGroup.GetEntities())
            {
                if (!entity.destroyed.isDestroyed)
                    continue;

            }

            //处理完,移除所有消耗组件
            foreach (var entity in __destroyedGroup.GetEntities())
            {
                if (!entity.destroyed.isDestroyed)
                {
                    if (entity.destroyed.delayTime > 0f)
                    {
                        entity.destroyed.delayTime = entity.destroyed.delayTime - Time.deltaTime;
                        if (entity.destroyed.delayTime <= 0f)
                        {
                            entity.destroyed.isDestroyed = true;
                            entity.destroyed.delayTime = 0f;
                        }
                    }
                }
                else
                {
                    entity.destroyed.isDestroyed = false;
                    entity.destroyed.action?.Invoke(entity);
                    entity.Destroy();
                }
            }
        }
    }
}

