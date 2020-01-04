using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace STGU3D
{
    public class BossFactory : BaseEntityFactory
    {
        protected override GameEntity OnCreate(string code)
        {
            var entity = CreateGameEntity(code);

            var bossDataCom = entity.CreateComponent<BossDataComponent>(GameComponentsLookup.BossData);

            
            if (entity.hasEntityData)
            {
                EBossType bossType = EntityUtil.GetBossTypeByCode(code);



            }

            entity.AddComponent(GameComponentsLookup.BossData, bossDataCom);


            return entity;
        }
    }

}
