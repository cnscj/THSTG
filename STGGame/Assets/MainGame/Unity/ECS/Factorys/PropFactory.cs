using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace STGU3D
{
    public class PropFactory : BaseEntityFactory
    {
        protected override GameEntity OnCreate(string code)
        {
            var entity = CreateGameEntity(code);

            var propDataCom = entity.CreateComponent<PropDataComponent>(GameComponentsLookup.PropData);

            

            if (entity.hasEntityData)
            {
               
            }

            entity.AddComponent(GameComponentsLookup.PropData, propDataCom);
            return entity;
        }
    }

}
