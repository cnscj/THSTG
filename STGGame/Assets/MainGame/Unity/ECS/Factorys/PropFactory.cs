using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace STGU3D
{
    public class PropFactory : BaseEntityFactory
    {
        public override GameEntity CreateEntity(string code)
        {
            var entity = CreateGameEntity(code);

            var propDataCom = entity.CreateComponent<PropDataComponent>(GameComponentsLookup.PropData);

            entity.AddComponent(GameComponentsLookup.PropData, propDataCom);

            if (entity.hasEntityData)
            {
               
            }
            return entity;
        }
        


    }

}
