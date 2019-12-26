﻿using System.Collections.Generic;
using Entitas;
using UnityEngine;

namespace STGU3D
{
    public class MovementSystem : IExecuteSystem
    {
        private IGroup<GameEntity> __moveGroup;
        public MovementSystem(Contexts contexts)
        {
            //移动
            __moveGroup = Contexts.sharedInstance.game.GetGroup(
                GameMatcher.AllOf(
                    GameMatcher.Transform,
                    GameMatcher.Movement
                ));
        }

        public void Execute()
        {
            // 满足GetTrigger和Filter的实体保存在entities列表里
            foreach (var entity in __moveGroup.GetEntities())
            {
                entity.transform.position += entity.movement.moveSpeed * Time.deltaTime;
               
                entity.transform.rotation += entity.movement.rotationSpeed * Time.deltaTime;


                entity.ReplaceComponent(GameComponentsLookup.Transform, entity.transform);
                
            }
        }
    }
}

