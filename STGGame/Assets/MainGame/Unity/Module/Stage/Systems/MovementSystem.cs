using Unity.Entities;
using Unity.Jobs;
using UnityEngine;
namespace STGService
{

    public class MovementSystem : ComponentSystem
    {
        struct MovementGroup
        {
            public MovementComponent movement;
            public Transform transform;
        }

        protected override void OnUpdate()
        {
            foreach (var entity in GetEntities<MovementGroup>())
            {
                Vector3 pos = entity.transform.position + entity.movement.moveDir * entity.movement.moveSpeed * Time.deltaTime;
                entity.transform.position = pos;
            }
        }

    }

}