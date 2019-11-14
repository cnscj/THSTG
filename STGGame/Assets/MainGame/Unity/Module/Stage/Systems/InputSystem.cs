using System.Collections;
using System.Collections.Generic;
using THGame;
using Unity.Entities;
using UnityEngine;

namespace STGU3D
{
    public class InputSystem : ComponentSystem
    {
        struct InputMoveGroup
        {
            //public InputComponent inputCom;
            public BehaviourMapper keymapsCom;
            public PlayerDataComponent playerDataCom;
            public MoveComponent moveCom;
        }

        struct InputShotGroup
        {
            //public InputComponent inputCom;
            public BehaviourMapper keymapsCom;
            public ShotComponent shotCom;
        }

        struct InputBombGroup
        {
            //public InputComponent inputCom;
            public BehaviourMapper keymapsCom;
            public BombComponent bombCom;
        }


        protected override void OnUpdate()
        {
            foreach (var entity in GetEntities<InputMoveGroup>())
            {
                entity.moveCom.speed = Vector3.zero;
                if (entity.keymapsCom.IsAtBehaviour((int)EPlayerBehavior.MoveLeft))
                {
                    entity.moveCom.speed += Vector3.left * entity.playerDataCom.moveSpeed;
                }
                else if (entity.keymapsCom.IsAtBehaviour((int)EPlayerBehavior.MoveRight))
                {
                    entity.moveCom.speed += Vector3.right * entity.playerDataCom.moveSpeed;
                }
                if (entity.keymapsCom.IsAtBehaviour((int)EPlayerBehavior.MoveUp))
                {
                    entity.moveCom.speed += Vector3.up * entity.playerDataCom.moveSpeed;
                }
                else if (entity.keymapsCom.IsAtBehaviour((int)EPlayerBehavior.MoveDown))
                {
                    entity.moveCom.speed += Vector3.down * entity.playerDataCom.moveSpeed;
                }
            }

            foreach (var entity in GetEntities<InputShotGroup>())
            {

            }
        }
    }

}
