﻿using System.Collections.Generic;
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

                GameObjectPoolManager.GetInstance().ReleaseGameObject(entity.view.viewGO);
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
                    continue;

                entity.destroyed.isDestroyed = false;
                entity.destroyed.action?.Invoke(entity);
                entity.Destroy();
            }
        }
    }
}
