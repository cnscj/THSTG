using THGame;
using Unity.Entities;
using UnityEngine;

namespace STGU3D
{
    public class AnimatorSystem : ComponentSystem
    {
        struct AnimatorGroup
        {
            public AnimatorComponent animatorCom;
            public RendererComponent rendererCom;
        }

        struct InputAnimatorGroup
        {
            public AnimatorComponent animatorCom;
            public BehaviourMapper keysmapCom;
        }

        protected override void OnUpdate()
        {
            foreach (var entity in GetEntities<AnimatorGroup>())
            {
                if (!entity.animatorCom.animator)
                {
                    if (entity.rendererCom.renderer != null)
                    {
                        entity.animatorCom.animator = entity.rendererCom.renderer.gameObject.GetComponent<Animator>();
                    }
                }
            }

            foreach (var entity in GetEntities<InputAnimatorGroup>())
            {
                if (entity.animatorCom.animator)
                {
                   
                    entity.animatorCom.animator.SetInteger("moveSpeed", 0);
                    if (entity.keysmapCom.IsAtBehaviour((int)EPlayerBehavior.MoveLeft))
                    {
                        entity.animatorCom.transform.localEulerAngles = new Vector3(0, 0, 0);
                        entity.animatorCom.animator.SetInteger("moveSpeed", -1);
                    }
                    else if (entity.keysmapCom.IsAtBehaviour((int)EPlayerBehavior.MoveRight))
                    {
                        entity.animatorCom.transform.localEulerAngles = new Vector3(0, 180, 0);
                        entity.animatorCom.animator.SetInteger("moveSpeed", 1);
                    }


                }
            }
        }
    }

}
