using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

namespace STGU3D
{
    public class InputSystem : ComponentSystem
    {
        struct InputGroup
        {
            public InputComponent input;
        }

        protected override void OnUpdate()
        {
            foreach (var entity in GetEntities<InputGroup>())
            {
                
            }
        }
    }

}
