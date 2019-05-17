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
            }
        }
    }
}