using THGame;
using Unity.Entities;
using Unity.Jobs;
using UnityEngine;
namespace STGU3D
{

    public class PlayerInputDisplaySystem : ComponentSystem
    {
        struct InputDisplayGroup
        {
            public DisplayComponent display;
            public BehaviourMapper input;
        }

        protected override void OnUpdate()
        {
            foreach (var entity in GetEntities<InputDisplayGroup>())
            {
                if (!entity.display.animator)
                    break;

                entity.display.animator.SetInteger("moveSpeed", 0);
                if (entity.input.IsAtBehaviour((int)EPlayerBehavior.MoveLeft))
                {
                    entity.display.transform.localEulerAngles = new Vector3(0, 0, 0);
                    entity.display.animator.SetInteger("moveSpeed", -1);
                }
                else if (entity.input.IsAtBehaviour((int)EPlayerBehavior.MoveRight))
                {
                    entity.display.transform.localEulerAngles = new Vector3(0, 180, 0);
                    entity.display.animator.SetInteger("moveSpeed", 1);
                }
                
                
            }
        }

    }

}