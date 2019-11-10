using THGame;
using Unity.Entities;
using Unity.Jobs;
using UnityEngine;
namespace STGU3D
{

    public class PlayerInputMovementSystem : ComponentSystem
    {
        struct InputMovementGroup
        {
            public PlayerDataComponent playerDataCom;
            public MovementComponent movementCom;
            public BehaviourMapper inputCom;
        }

        protected override void OnUpdate()
        {
            foreach (var entity in GetEntities<InputMovementGroup>())
            {
                entity.movementCom.moveSpeed = Vector3.zero;
                if (entity.inputCom.IsAtBehaviour((int)EPlayerBehavior.MoveLeft))
                {
                    entity.movementCom.moveSpeed += Vector3.left * entity.playerDataCom.moveSpeed;
                }
                else if (entity.inputCom.IsAtBehaviour((int)EPlayerBehavior.MoveRight))
                {
                    entity.movementCom.moveSpeed += Vector3.right * entity.playerDataCom.moveSpeed;
                }
                if (entity.inputCom.IsAtBehaviour((int)EPlayerBehavior.MoveUp))
                {
                    entity.movementCom.moveSpeed += Vector3.up * entity.playerDataCom.moveSpeed;
                }
                else if (entity.inputCom.IsAtBehaviour((int)EPlayerBehavior.MoveDown))
                {
                    entity.movementCom.moveSpeed += Vector3.down * entity.playerDataCom.moveSpeed;
                }
            }
        }

    }

}