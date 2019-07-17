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
            public PlayerInputComponent input;
        }

        bool IsHaveBahaviour(PlayerInputComponent input, EPlayerBehavior behaviour)
        {
            if (input.keyStatus.ContainsKey(behaviour))
            {
                return input.keyStatus[behaviour];
            }
            return false;
        }
        

        protected override void OnUpdate()
        {
            foreach (var entity in GetEntities<InputMovementGroup>())
            {
                entity.movement.moveDir = Vector3.zero;
                if (IsHaveBahaviour(entity.input,EPlayerBehavior.MoveLeft))
                {
                    entity.movement.moveDir.x = -1;
                }

                else if (IsHaveBahaviour(entity.input, EPlayerBehavior.MoveRight))
                {
                    entity.movement.moveDir.x = 1;
                }

                if (IsHaveBahaviour(entity.input, EPlayerBehavior.MoveUp))
                {
                    entity.movement.moveDir.y = 1;
                }

                else if (IsHaveBahaviour(entity.input, EPlayerBehavior.MoveDown))
                {
                    entity.movement.moveDir.y = -1;
                }


            }
        }

    }

}