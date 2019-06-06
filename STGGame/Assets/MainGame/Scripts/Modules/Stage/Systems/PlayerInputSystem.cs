using UnityEngine;
using Unity.Entities;
namespace STGGame
{
    public class PlayerInputSystem : ComponentSystem
    {
        struct PlayerInputGroup
        {
            public PlayerInputCompnent playerInput;
            public Transform transform;
        }

        protected override void OnUpdate()
        {
            foreach (var item in GetEntities<PlayerInputGroup>())
            {
                //根据不同类型的玩家触发不同按键
                UpdateMovement(item);
                UpdateLookAt(item);
            }
        }

        private void UpdateMovement(PlayerInputGroup item)
        {
            if (Input.GetKey(KeyCode.UpArrow))
            {
                item.transform.Translate(Vector3.forward * item.playerInput.moveSpeed * Time.deltaTime);
            }
            else if (Input.GetKey(KeyCode.DownArrow))
            {
                item.transform.Translate(Vector3.forward * -item.playerInput.moveSpeed * Time.deltaTime);
            }
            if (Input.GetKey(KeyCode.LeftArrow))
            {
                item.transform.Translate(Vector3.left * item.playerInput.moveSpeed * Time.deltaTime);
            }
            else if (Input.GetKey(KeyCode.RightArrow))
            {
                item.transform.Translate(Vector3.left * -item.playerInput.moveSpeed * Time.deltaTime);
            }
        }

        private void UpdateLookAt(PlayerInputGroup item)
        {
          
        }
    }
}