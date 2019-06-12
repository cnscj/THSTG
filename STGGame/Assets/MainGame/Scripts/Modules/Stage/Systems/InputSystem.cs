using UnityEngine;
using Unity.Entities;
namespace STGGame
{
    public class InputSystem : ComponentSystem
    {
        struct ComponentGroup
        {
            public PlayerMovementComponent playerMovement;
        }

        protected override void OnUpdate()
        {
            foreach (var item in GetEntities<ComponentGroup>())
            {
                //根据不同类型的玩家触发不同按键
                UpdateMovement(item);
                UpdateLookAt(item);
            }
        }

        private void UpdateMovement(ComponentGroup entity)
        {
            entity.playerMovement.moveDir = Vector3.zero;
            if (Input.GetKey(KeyCode.UpArrow))
            {
                entity.playerMovement.moveDir = Vector3.forward;
            }
            else if (Input.GetKey(KeyCode.DownArrow))
            {
                entity.playerMovement.moveDir = Vector3.back;
            }
            if (Input.GetKey(KeyCode.LeftArrow))
            {
                entity.playerMovement.moveDir = Vector3.left;
            }
            else if (Input.GetKey(KeyCode.RightArrow))
            {
                entity.playerMovement.moveDir = Vector3.right;
            }
        }

        private void UpdateLookAt(ComponentGroup entity)
        {
          
        }
    }
}