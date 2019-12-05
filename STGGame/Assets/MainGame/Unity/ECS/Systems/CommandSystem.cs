using System.Collections.Generic;
using Entitas;
using UnityEngine;
using XLibGame;

namespace STGU3D
{
    public class CommandSystem : IExecuteSystem
    {
        private IGroup<GameEntity> __moveGroup;
        private IGroup<GameEntity> __fireGroup;
        private IGroup<GameEntity> __bombGroup;
        private IGroup<GameEntity> __eliminateGroup;
        private IGroup<GameEntity> __shiftGroup;

        public CommandSystem(Contexts contexts)
        {
            __moveGroup = Contexts.sharedInstance.game.GetGroup(
                GameMatcher.AllOf(
                     GameMatcher.Command,
                     GameMatcher.Movement
                ));

            __fireGroup = Contexts.sharedInstance.game.GetGroup(
                GameMatcher.AllOf(
                     GameMatcher.Command,
                     GameMatcher.Shot
                ));

            __bombGroup = Contexts.sharedInstance.game.GetGroup(
                GameMatcher.AllOf(
                     GameMatcher.Command,
                     GameMatcher.Bomb
                ));

            __eliminateGroup = Contexts.sharedInstance.game.GetGroup(
                GameMatcher.AllOf(
                     GameMatcher.Command,
                     GameMatcher.Eliminate
                ));

            __shiftGroup = Contexts.sharedInstance.game.GetGroup(
                GameMatcher.AllOf(
                     GameMatcher.Command,
                     GameMatcher.Shift
                ));

        }

        public void Execute()
        {
            //减速
            foreach (var entity in __shiftGroup.GetEntities())
            {
                bool isShifting = false;
                if (InputMapper.GetInstance().IsAtBehaviour((int)EBehaviorType.Shift))
                {
                    isShifting = true;
                }

                var shiftCom = entity.GetComponent(GameComponentsLookup.Shift) as ShiftComponent;
                shiftCom.isShifting = isShifting;

                entity.ReplaceComponent(GameComponentsLookup.Shift, shiftCom);
            }

            //移动
            foreach (var entity in __moveGroup.GetEntities())
            {
                Vector3 newMoveSpeed = Vector3.zero;
                Vector3 moveDirection = Vector3.zero;
                if (InputMapper.GetInstance().IsAtBehaviour((int)EBehaviorType.MoveLeft))
                {
                    moveDirection += Vector3.left;
                }
                else if (InputMapper.GetInstance().IsAtBehaviour((int)EBehaviorType.MoveRight))
                {
                    moveDirection += Vector3.right;
                }

                if (InputMapper.GetInstance().IsAtBehaviour((int)EBehaviorType.MoveUp))
                {
                    moveDirection += Vector3.up;
                }
                else if (InputMapper.GetInstance().IsAtBehaviour((int)EBehaviorType.MoveDown))
                {
                    moveDirection += Vector3.down;
                }
                if (entity.hasDecelerate && entity.decelerate.isDecelerating)
                {
                    newMoveSpeed = moveDirection * entity.playerData.moveSpeed * entity.decelerate.speedRate;
                }
                else
                {
                    newMoveSpeed = moveDirection * entity.playerData.moveSpeed;
                }
                entity.movement.moveSpeed = newMoveSpeed;
                entity.ReplaceComponent(GameComponentsLookup.Movement, entity.movement);
            }

            //开火
            foreach (var entity in __fireGroup.GetEntities())
            {
                bool isFire = false;
                if (InputMapper.GetInstance().IsAtBehaviour((int)EBehaviorType.Attack))
                {
                    isFire = true;
                }

                if (entity.shot.isFiring != isFire)
                {
                    var shotCom = entity.GetComponent(GameComponentsLookup.Shot) as ShotComponent;
                    shotCom.isFiring = isFire;

                    entity.ReplaceComponent(GameComponentsLookup.Shot, shotCom);
                }
            }

            //Bomb
            foreach (var entity in __bombGroup.GetEntities())
            {
                if (InputMapper.GetInstance().IsAtBehaviour((int)EBehaviorType.Bomb))
                {
                    var bombCom = entity.GetComponent(GameComponentsLookup.Bomb) as BombComponent;
                    bombCom.isBombing = true;

                    entity.ReplaceComponent(GameComponentsLookup.Bomb, bombCom);
                }
            }

            //消弹
            foreach (var entity in __eliminateGroup.GetEntities())
            {
                bool isEliminate = false;
                if (InputMapper.GetInstance().IsAtBehaviour((int)EBehaviorType.Eliminate))
                {
                    isEliminate = true;
                }

                var eliminateCom = entity.GetComponent(GameComponentsLookup.Eliminate) as EliminateComponent;
                eliminateCom.isEliminating = isEliminate;

                entity.ReplaceComponent(GameComponentsLookup.Eliminate, eliminateCom);
                
            }

        }
    }

}
