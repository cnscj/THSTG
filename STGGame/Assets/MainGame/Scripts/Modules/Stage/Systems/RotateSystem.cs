using UnityEngine;
using Unity.Entities;

namespace STGGame
{
    public class RotateSystem : ComponentSystem
    {
        protected override void OnUpdate()
        {
            foreach (var item in GetEntities<RotateGroup>())
            {
                item.transform.Rotate(0f, item.rotator.speed * Time.deltaTime, 0f);
            }
        }

    }
}