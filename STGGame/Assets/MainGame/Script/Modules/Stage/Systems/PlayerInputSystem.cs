using Unity.Entities;
using Unity.Jobs;
using UnityEngine;
namespace STGGame
{

    public class PlayerInputSystem : ComponentSystem
    {
        struct PlayerInputGroup
        {
            public PlayerInputComponent input;
        }

        protected override void OnUpdate()
        {
            foreach (var entity in GetEntities<PlayerInputGroup>())
            {
                //把所有的按键记录到输入组件中
                foreach(var keyPair in entity.input.keyList)
                {
                    bool ret = false;
                    foreach(var keyCode in keyPair.keycodes)
                    {
                        ret = ret | Input.GetKey(keyCode);
                    }
                    entity.input.keyStatus[keyPair.behaviour] = ret;
                }
            }
        }

    }

}