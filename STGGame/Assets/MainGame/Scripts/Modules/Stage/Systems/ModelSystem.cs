using UnityEngine;
using Unity.Entities;
namespace STGGame
{
    public class ModelSystem : ComponentSystem
    {
        struct ComponentGroup
        {
            public ModelComponent modelComp;
        }

        protected override void OnUpdate()
        {

        }
    }
}