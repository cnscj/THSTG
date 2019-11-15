using THGame;
using Unity.Entities;
using Unity.Jobs;
using UnityEngine;
namespace STGU3D
{

    public class ShotSystem : ComponentSystem
    {
        struct ShotGroup
        {
            public ShotComponent shotCom;
        }

        protected override void OnUpdate()
        {
            foreach (var entity in GetEntities<ShotGroup>())
            {
                //开火状态
                if (entity.shotCom.isFire)
                {

                }
            }
        }

    }

}