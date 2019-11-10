using THGame;
using Unity.Entities;
using Unity.Jobs;
using UnityEngine;
namespace STGU3D
{

    public class PlayerShotSystem : ComponentSystem
    {
        struct PlayerShotGroup
        {
            public PlayerDataComponent playerData;
            public ShotComponent shotCom;
            public BehaviourMapper input;
        }

        protected override void OnUpdate()
        {
            foreach (var entity in GetEntities<PlayerShotGroup>())
            {
                
            }
        }

    }

}