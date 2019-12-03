using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace STGU3D
{
    public class WingmanFactory : BaseEntityFactory
    {
        public override GameEntity CreateEntity(string code)
        {
            var entity = CreateGameEntity(code);

            var wingmanDataCom = entity.CreateComponent<WingmanDataComponent>(GameComponentsLookup.WingmanData);
            var followCom = entity.CreateComponent<FollowComponent>(GameComponentsLookup.Follow);


            entity.AddComponent(GameComponentsLookup.Follow, followCom);
            entity.AddComponent(GameComponentsLookup.WingmanData, wingmanDataCom);

            if (entity.hasEntityData)
            {
                entity.view.viewGO = NewViewNode(false, entity.entityData.entityData["viewCode"], entity.transform.position, entity.transform.rotation);
                entity.ReplaceComponent(GameComponentsLookup.View, entity.view);
            }

            return entity;
        }

        public GameEntity CreateWingman(EWingmanType wingmanType)
        {
            string code = string.Format("{0}", 10000000 + 100000 * (int)EEntityType.Wingman + 100 * (int)wingmanType + (int)1);
            var entity = CreateEntity(code);

            var wingmanDataCom = entity.GetComponent(GameComponentsLookup.WingmanData) as WingmanDataComponent;
            if (wingmanDataCom != null)
            {
                if (wingmanType == EWingmanType.Onmyougyoku)
                {
                    var onmyougyokuCom = entity.CreateComponent<OnmyougyokuWingmanComponent>(GameComponentsLookup.OnmyougyokuWingman);
                    entity.AddComponent(GameComponentsLookup.OnmyougyokuWingman, onmyougyokuCom);
                }
            }
            return entity;
        }


    }

}
