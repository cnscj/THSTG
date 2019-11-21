using System.Collections.Generic;
using Entitas;
using UnityEngine;

namespace STGU3D
{
    public class DestroyedSystem : ICleanupSystem
    {
        public DestroyedSystem(Contexts contexts)
        {

        }

        public void Cleanup()
        {

            var viewGroup = Contexts.sharedInstance.game.GetGroup(
                GameMatcher.AllOf(
                     GameMatcher.Destroyed,
                     GameMatcher.View
                ));

            foreach (var entity in viewGroup.GetEntities())
            {
                //移除View
                GameObject.Destroy(entity.view.viewGO);
            }

            var heroBulletGroup = Contexts.sharedInstance.game.GetGroup(
                GameMatcher.AllOf(
                     GameMatcher.Destroyed,
                     GameMatcher.HeroBulletFlag
                ));

            foreach (var entity in heroBulletGroup.GetEntities())
            {


            }

            var entityBulletGroup = Contexts.sharedInstance.game.GetGroup(
                GameMatcher.AllOf(
                     GameMatcher.Destroyed,
                     GameMatcher.EntityBulletFlag
                ));

            foreach (var entity in entityBulletGroup.GetEntities())
            {


            }

            //处理完,移除所有消耗组件
            var destroyGroup = Contexts.sharedInstance.game.GetGroup(
                GameMatcher.AllOf(
                     GameMatcher.Destroyed
                ));

            foreach (var entity in viewGroup.GetEntities())
            {
                entity.RemoveComponent(GameComponentsLookup.Destroyed);
            }
        }
    }
}

