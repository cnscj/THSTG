using System.Collections;
using System.Collections.Generic;
using THGame;
using Unity.Entities;
using UnityEngine;

namespace STGU3D
{
    public class InputSystem : ComponentSystem
    {
        struct InputGroup
        {
            public InputComponent inputCom;
            public BehaviourMapper behaviourMapperCom;
        }

        struct InputPlayerMoveGroup
        {
            public InputComponent inputCom;
            public PlayerDataComponent entityDataCom;   //无法捕获
            public MovementComponent movementCom;
        }

        struct InputShotGroup
        {
            public InputComponent inputCom;
            public ShotComponent shotCom;
        }

        struct InputBombGroup
        {
            public InputComponent inputCom;
            public BombComponent bombCom;
        }


        protected override void OnUpdate()
        {
            foreach (var entity in GetEntities<InputGroup>())
            {
                if(entity.inputCom.keymaps == null)
                {
                    entity.inputCom.keymaps = entity.behaviourMapperCom;
                }
            }

            foreach (var entity in GetEntities<InputPlayerMoveGroup>())
            {
                entity.movementCom.moveSpeed = Vector3.zero;
                if (entity.inputCom.keymaps.IsAtBehaviour((int)EPlayerBehavior.MoveLeft))
                {
                    entity.movementCom.moveSpeed += Vector3.left * entity.entityDataCom.moveSpeed;
                    
                }
                else if (entity.inputCom.keymaps.IsAtBehaviour((int)EPlayerBehavior.MoveRight))
                {
                    entity.movementCom.moveSpeed += Vector3.right * entity.entityDataCom.moveSpeed;
                }
                if (entity.inputCom.keymaps.IsAtBehaviour((int)EPlayerBehavior.MoveUp))
                {
                    entity.movementCom.moveSpeed += Vector3.up * entity.entityDataCom.moveSpeed;
                }
                else if (entity.inputCom.keymaps.IsAtBehaviour((int)EPlayerBehavior.MoveDown))
                {
                    entity.movementCom.moveSpeed += Vector3.down * entity.entityDataCom.moveSpeed;
                }
            }

            foreach (var entity in GetEntities<InputShotGroup>())
            {

            }
        }
    }

}
