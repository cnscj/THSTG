using System.Collections.Generic;
using Entitas;
using UnityEngine;

namespace STGU3D
{
    public class CageSystem : ICleanupSystem
    {
        private IGroup<GameEntity> __cageGroup;
        public CageSystem(Contexts contexts)
        {
            __cageGroup = Contexts.sharedInstance.game.GetGroup(
               GameMatcher.AllOf(
                    GameMatcher.Cage,
                    GameMatcher.Transform
               ));
        }

        public void Cleanup()
        {
            //移动
            foreach (var entity in __cageGroup.GetEntities())
            {
                //边界判断,不能出边界
                //注意锚点
                if (entity.transform.position.x - entity.cage.bodySize.x / 2 < entity.cage.movableArea.x)
                {
                    entity.transform.position.x = entity.cage.movableArea.x + entity.cage.bodySize.x / 2;
                    entity.ReplaceComponent(GameComponentsLookup.Transform, entity.transform);
                }
                else if(entity.transform.position.x + entity.cage.bodySize.x / 2 > entity.cage.movableArea.width + entity.cage.movableArea.x)
                {
                    entity.transform.position.x = entity.cage.movableArea.width + entity.cage.movableArea.x - entity.cage.bodySize.x / 2;
                    entity.ReplaceComponent(GameComponentsLookup.Transform, entity.transform);
                }

                if (entity.transform.position.y + entity.cage.bodySize.y / 2 >  -entity.cage.movableArea.y)
                {
                    entity.transform.position.y = -entity.cage.movableArea.y - entity.cage.bodySize.y / 2;
                    entity.ReplaceComponent(GameComponentsLookup.Transform, entity.transform);
                }
                else if (entity.transform.position.y - entity.cage.bodySize.y / 2 < entity.cage.movableArea.y)
                {
                    entity.transform.position.y = entity.cage.movableArea.y + entity.cage.bodySize.y / 2;
                    entity.ReplaceComponent(GameComponentsLookup.Transform, entity.transform);
                }
            }
        }
    }
}

