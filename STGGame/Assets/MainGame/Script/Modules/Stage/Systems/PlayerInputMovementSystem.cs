using THGame;
using Unity.Entities;
using Unity.Jobs;
using UnityEngine;
namespace STGGame
{

    public class PlayerInputMovementSystem : ComponentSystem
    {
        struct InputMovementGroup
        {
            public MovementComponent movement;
            public BehaviourMapper input;
        }

        protected override void OnUpdate()
        {
            foreach (var entity in GetEntities<InputMovementGroup>())
            {
                entity.movement.moveDir = Vector3.zero;
                if (entity.input.IsAtBehaviour((int)EPlayerBehavior.MoveLeft))
                {
                    entity.movement.moveDir.x = -1;
                }
                else if (entity.input.IsAtBehaviour((int)EPlayerBehavior.MoveRight))
                {
                    entity.movement.moveDir.x = 1;
                }
                if (entity.input.IsAtBehaviour((int)EPlayerBehavior.MoveUp))
                {
                    entity.movement.moveDir.y = 1;
                }
                else if (entity.input.IsAtBehaviour((int)EPlayerBehavior.MoveDown))
                {
                    entity.movement.moveDir.y = -1;
                }
            }
        }

    }

}