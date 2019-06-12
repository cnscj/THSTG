using UnityEngine;
using Unity.Entities;

namespace STGGame
{

    public class PlayerMovementSystem : ComponentSystem
    {

        struct PlayerMovementGroup
        {
            public PlayerMovementComponent playerMovement;
            public Transform transform;
        }

        protected override void OnUpdate()
        {
            foreach (var entity in GetEntities<PlayerMovementGroup>())
            {
                Vector3 pos = entity.transform.position + entity.playerMovement.moveDir * entity.playerMovement.moveSpeed * Time.deltaTime;
                entity.transform.position = pos;
            }
        }
    }

}