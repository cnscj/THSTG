using THGame;
using Unity.Entities;
using Unity.Jobs;
using UnityEngine;
namespace STGU3D
{

    public class BombSystem : ComponentSystem
    {
        struct BombGroup
        {
            public BombComponent bombCom;
        }

        protected override void OnUpdate()
        {
            foreach (var entity in GetEntities<BombGroup>())
            {
                //按下bomb
                if (entity.bombCom.isBomb)
                {
                    //是否到达冷却时间


                    entity.bombCom.isBomb = false;
                }
            }
        }

    }

}