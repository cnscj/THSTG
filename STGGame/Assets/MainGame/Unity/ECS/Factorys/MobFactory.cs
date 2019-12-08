using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace STGU3D
{
    public class MobFactory : BaseEntityFactory
    {
        public override GameEntity CreateEntity(string code)
        {
            var entity = CreateGameEntity(code);

            var mobDataCom = entity.CreateComponent<MobDataComponent>(GameComponentsLookup.MobData);
           

            entity.AddComponent(GameComponentsLookup.MobData, mobDataCom);

            if (entity.hasEntityData)
            {
               
            }
            return entity;
        }
        


    }

}
