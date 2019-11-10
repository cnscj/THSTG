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
            public AnimatorComponent animatorCom;
            public BehaviourMapper inputCom;
        }

        protected override void OnUpdate()
        {
            foreach (var entity in GetEntities<InputDisplayGroup>())
            {
                if (!entity.animatorCom.animator)
                    continue;

                entity.animatorCom.animator.SetInteger("moveSpeed", 0);
                if (entity.inputCom.IsAtBehaviour((int)EPlayerBehavior.MoveLeft))
                {
                    entity.animatorCom.transform.localEulerAngles = new Vector3(0, 0, 0);
                    entity.animatorCom.animator.SetInteger("moveSpeed", -1);
                }
                else if (entity.inputCom.IsAtBehaviour((int)EPlayerBehavior.MoveRight))
                {
                    entity.animatorCom.transform.localEulerAngles = new Vector3(0, 180, 0);
                    entity.animatorCom.animator.SetInteger("moveSpeed", 1);
                }
                
                
            }
        }

    }

}