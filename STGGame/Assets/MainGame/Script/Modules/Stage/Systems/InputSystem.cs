using Unity.Entities;
using Unity.Jobs;
using UnityEngine;
namespace STGGame
{

    public class InputSystem : ComponentSystem
    {
        struct InputGroup
        {
            public InputCompnent input;
            public MovementComponent movement;
        }

        protected override void OnUpdate()
        {
            foreach (var entity in GetEntities<InputGroup>())
            {
               
            }
        }

    }

}