using THGame;
using Unity.Entities;
using Unity.Jobs;
using UnityEngine;
namespace STGU3D
{

    public class PlayerInputBehaviorSystem : ComponentSystem
    {
        struct InputBehaviorGroup
        {
            public PlayerBehaviorComponent behavior;
            public BehaviourMapper input;
        }

        protected override void OnUpdate()
        {
            foreach (var entity in GetEntities<InputBehaviorGroup>())
            {
                if (entity.behavior.myselfBehavior != null)
                {
                    entity.behavior.myselfBehavior = entity.behavior.myselfBehavior.HandleInput(null, entity.input);
                }
                
            }
        }

    }

}