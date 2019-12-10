using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace STGU3D
{
    public class BossFactory : BaseEntityFactory
    {
        public override GameEntity CreateEntity(string code)
        {
            var entity = CreateGameEntity(code);

            var bossDataCom = entity.CreateComponent<BossDataComponent>(GameComponentsLookup.BossData);
           

            entity.AddComponent(GameComponentsLookup.BossData, bossDataCom);

            if (entity.hasEntityData)
            {
                EBossType bossType = EntityUtil.GetBossTypeByCode(code);



            }

            return entity;
        }
        


    }

}
