using Unity.Entities;
using Unity.Jobs;
using UnityEngine;
namespace STGU3D
{

    public class HealthSystem : ComponentSystem
    {
        struct HealthGroup
        {
            public HealthComponent healthCom;
        }

        struct HeroHealthGroup
        {
            public HealthComponent healthCom;
            public PlayerDataComponent playerData;
        }

        protected override void OnUpdate()
        {
            foreach (var entity in GetEntities<HealthGroup>())
            {
              
            }

            foreach (var entity in GetEntities<HeroHealthGroup>())
            {
                //发送消息
            }
        }

    }

}