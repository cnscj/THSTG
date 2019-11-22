﻿using System.Collections.Generic;
using Entitas;
using UnityEngine;

namespace STGU3D
{
    public class EliminateExecuteSystem : IExecuteSystem
    {
        public EliminateExecuteSystem(Contexts contexts)
        {

        }

        public void StartElinate()
        {

        }

        public void EndElinate()
        {

        }

        public void Execute()
        {
            //移动
            var eliminateGroup = Contexts.sharedInstance.game.GetGroup(
                GameMatcher.AllOf(
                     GameMatcher.Eliminate
                ));

            foreach (var entity in eliminateGroup.GetEntities())
            {
                if (entity.eliminate.isEliminating)
                {
                    if (entity.eliminate.holdTime > 0)
                    {
                        entity.eliminate.holdTime -= entity.eliminate.consumeRate * Time.fixedDeltaTime;
                        entity.eliminate.nextChargeTime = entity.eliminate.cdTime + Time.fixedTime;
                        if (entity.eliminate.holdTime < 0f) entity.eliminate.holdTime = 0f;
                    }
                }
                else
                {
                    if (entity.eliminate.holdTime < entity.eliminate.maxHoldTime)
                    {
                        //到达充能时间
                        if (Time.fixedTime >= entity.eliminate.nextChargeTime)
                        {
                            entity.eliminate.holdTime += entity.eliminate.chargeRate * Time.fixedDeltaTime;
                            if (entity.eliminate.holdTime > entity.eliminate.maxHoldTime) entity.eliminate.holdTime = entity.eliminate.maxHoldTime;
                        }
                        
                    }
                }
            }

        }
    }
}

