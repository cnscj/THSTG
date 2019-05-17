using UnityEngine;
using Unity.Entities;

namespace STGGame
{
    public class RotateSystem : ComponentSystem
    {
        struct RotateGroup
        {
            public RotateComponent rotator;
            public Transform transform;
        }

        protected override void OnUpdate()
        {
            foreach (var item in GetEntities<RotateGroup>())
            {
                item.transform.Rotate(0f, item.rotator.speed * Time.deltaTime, 0f);
            }
        }

    }
}