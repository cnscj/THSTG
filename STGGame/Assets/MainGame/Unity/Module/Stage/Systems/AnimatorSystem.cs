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
        //按键不是移动动画的起因
        struct InputAnimatorGroup
        {
            public AnimatorComponent animatorCom;
            public MovementComponent movementCom;
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
                   
                    if (entity.movementCom.moveSpeed.x > 0f) //右
                    {
                        entity.animatorCom.transform.localEulerAngles = new Vector3(0, 180, 0);
                        entity.animatorCom.animator.SetInteger("moveSpeed", 1);
                    }
                    else if(entity.movementCom.moveSpeed.x < 0f) //左
                    {
                        entity.animatorCom.transform.localEulerAngles = new Vector3(0, 0, 0);
                        entity.animatorCom.animator.SetInteger("moveSpeed", -1);
                    }

                    if (System.Math.Abs(entity.movementCom.moveSpeed.x) < 0.000001f && System.Math.Abs(entity.movementCom.moveSpeed.y) < 0.000001f)
                    {
                        entity.animatorCom.animator.SetInteger("moveSpeed", 0);
                    }

 
                }
            }
        }
    }

}
