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

        protected override void OnUpdate()
        {
            foreach (var entity in GetEntities<AnimatorGroup>())
            {
                if (entity.animatorCom.animator)
                {
                    //TODO:根据一系列行为展示动画
                }
                else
                {
                    if (entity.rendererCom.renderer != null)
                    {
                        entity.animatorCom.animator = entity.rendererCom.renderer.gameObject.GetComponent<Animator>();
                    }
                }
            }
        }
    }

}
