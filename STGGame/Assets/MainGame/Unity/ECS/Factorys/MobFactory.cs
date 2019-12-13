using System.Collections;
using System.Collections.Generic;
using STGGame;
using UnityEngine;

namespace STGU3D
{
    public class MobFactory : BaseEntityFactory
    {
        public override GameEntity CreateEntity(string code)
        {
            var entity = CreateGameEntity(code);

            var mobDataCom = entity.CreateComponent<MobDataComponent>(GameComponentsLookup.MobData);
            var recycleCom = entity.CreateComponent<RecycleComponent>(GameComponentsLookup.Recycle);

            entity.AddComponent(GameComponentsLookup.MobData, mobDataCom);
            entity.AddComponent(GameComponentsLookup.Recycle, recycleCom);

            if (entity.hasEntityData)
            {
                entity.view.view = ComponentUtil.CreateView(entity);
                ((UnityView)entity.view.view).AddBody(entity.entityData.entityData["viewCode"]);
                entity.ReplaceComponent(GameComponentsLookup.View, entity.view);

                {
                    recycleCom.maxStayTime = 3f;
                    recycleCom.stayTime = 0f;
                    recycleCom.isRecycled = false;
                    recycleCom.boundary = DirectorUtil.ScreenRectInWorld(DirectorUtil.GetScreenRect());
                }

            }
            return entity;
        }
        


    }

}
