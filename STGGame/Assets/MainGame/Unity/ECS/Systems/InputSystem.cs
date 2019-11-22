using System.Collections.Generic;
using Entitas;
using UnityEngine;
using XLibGame;

namespace STGU3D
{
    public class InputSystem : IExecuteSystem
    {


        public InputSystem(Contexts contexts)
        {

        }

        public void Execute()
        {
            var moveGroup = Contexts.sharedInstance.game.GetGroup(
                GameMatcher.AllOf(
                     GameMatcher.PlayerData,
                     GameMatcher.Movement
                ));

            foreach (var entity in moveGroup.GetEntities())
            {
                Vector3 newMoveSpeed = Vector3.zero;
                Vector3 moveDirection = Vector3.zero;
                if (InputMapper.GetInstance().IsAtBehaviour((int)EPlayerBehavior.MoveLeft))
                {
                    moveDirection += Vector3.left;
                }
                else if (InputMapper.GetInstance().IsAtBehaviour((int)EPlayerBehavior.MoveRight))
                {
                    moveDirection += Vector3.right;
                }

                if (InputMapper.GetInstance().IsAtBehaviour((int)EPlayerBehavior.MoveUp))
                {
                    moveDirection += Vector3.up;
                }
                else if (InputMapper.GetInstance().IsAtBehaviour((int)EPlayerBehavior.MoveDown))
                {
                    moveDirection += Vector3.down;
                }

                newMoveSpeed = moveDirection * entity.playerData.speed;
                entity.ReplaceMovement(newMoveSpeed, entity.movement.rotationSpeed);
            }

            //开火
            var fireGroup = Contexts.sharedInstance.game.GetGroup(
                GameMatcher.AllOf(
                     GameMatcher.PlayerData,
                     GameMatcher.Shot
                ));

            foreach (var entity in fireGroup.GetEntities())
            {
                bool isFire = false;
                if (InputMapper.GetInstance().IsAtBehaviour((int)EPlayerBehavior.Attack))
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


            //丢B
            var bombGroup = Contexts.sharedInstance.game.GetGroup(
                GameMatcher.AllOf(
                     GameMatcher.PlayerData,
                     GameMatcher.Bomb
                ));

            foreach (var entity in bombGroup.GetEntities())
            {
                if (InputMapper.GetInstance().IsAtBehaviour((int)EPlayerBehavior.Bomb))
                {
                    var bombCom = entity.GetComponent(GameComponentsLookup.Bomb) as BombComponent;
                    bombCom.isBombing = true;

                    entity.ReplaceComponent(GameComponentsLookup.Bomb, bombCom);
                }
            }

            //丢B
            var eliminateGroup = Contexts.sharedInstance.game.GetGroup(
                GameMatcher.AllOf(
                     GameMatcher.PlayerData,
                     GameMatcher.Eliminate
                ));

            foreach (var entity in eliminateGroup.GetEntities())
            {
                bool isEliminate = false;
                if (InputMapper.GetInstance().IsAtBehaviour((int)EPlayerBehavior.Eliminate))
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
