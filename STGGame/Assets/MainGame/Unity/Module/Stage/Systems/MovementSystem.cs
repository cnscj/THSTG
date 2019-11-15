using Unity.Entities;
using Unity.Jobs;
using UnityEngine;
namespace STGU3D
{

    public class MovementSystem : ComponentSystem
    {
        struct MovementGroup
        {
            public MovementComponent movementCom;
            public Transform transform;
        }

        protected override void OnUpdate()
        {
            foreach (var entity in GetEntities<MovementGroup>())
            {
                Vector3 pos = entity.transform.position + entity.movementCom.moveSpeed * Time.deltaTime;
                entity.transform.position = pos;

                //Vector3 rotation = entity.transform.localEulerAngles + entity.movementCom.rotationSpeed * Time.deltaTime;
                //entity.transform.localEulerAngles = rotation;
            }
        }

    }

}