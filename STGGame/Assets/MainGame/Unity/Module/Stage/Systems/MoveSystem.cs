using Unity.Entities;
using Unity.Jobs;
using UnityEngine;
namespace STGU3D
{

    public class MoveSystem : ComponentSystem
    {
        struct MoveGroup
        {
            public MoveComponent moveCom;
            public Transform transform;
        }

        protected override void OnUpdate()
        {
            foreach (var entity in GetEntities<MoveGroup>())
            {
                Vector3 pos = entity.transform.position + entity.moveCom.speed * Time.deltaTime;
                entity.transform.position = pos;
            }
        }

    }

}