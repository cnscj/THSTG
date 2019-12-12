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
            var wingmanType = EntityUtil.GetWingmanTypeByCode(code);

            var wingmanDataCom = entity.CreateComponent<WingmanDataComponent>(GameComponentsLookup.WingmanData);
            var followCom = entity.CreateComponent<FollowComponent>(GameComponentsLookup.Follow);


            entity.AddComponent(GameComponentsLookup.Follow, followCom);
            entity.AddComponent(GameComponentsLookup.WingmanData, wingmanDataCom);

            if (entity.hasEntityData)
            {
                entity.view.view = ComponentUtil.CreateView(entity);
                ((UnityView)entity.view.view).AddBody(entity.entityData.entityData["viewCode"]);
                entity.ReplaceComponent(GameComponentsLookup.View, entity.view);

                wingmanDataCom.wingmanType = wingmanType;
                if (wingmanType == EWingmanType.Onmyougyoku)
                {
                    var onmyougyokuCom = entity.CreateComponent<OnmyougyokuWingmanComponent>(GameComponentsLookup.OnmyougyokuWingman);
                    entity.AddComponent(GameComponentsLookup.OnmyougyokuWingman, onmyougyokuCom);
                }
            }

            return entity;
        }

        public GameEntity CreateWingman(EWingmanType wingmanType)
        {
            string code = EntityUtil.GetWingmanCode(wingmanType);
            var entity = CreateEntity(code);
   
            return entity;
        }


    }

}
