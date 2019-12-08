using System.Collections;
using System.Collections.Generic;
using STGGame;
using UnityEngine;

namespace STGU3D
{
    public class BulletFactory : BaseEntityFactory
    {
        public override GameEntity CreateEntity(string code)
        {
            var entity = CreateGameEntity(code);
            var bulletDataCom = entity.CreateComponent<BulletDataComponent>(GameComponentsLookup.BulletData);

            var healthCom = entity.CreateComponent<HealthComponent>(GameComponentsLookup.Health);
            var recycleCom = entity.CreateComponent<RecycleComponent>(GameComponentsLookup.Recycle);

            entity.AddComponent(GameComponentsLookup.BulletData, bulletDataCom);
            entity.AddComponent(GameComponentsLookup.Health, healthCom);
            entity.AddComponent(GameComponentsLookup.Recycle, recycleCom);

            if (entity.hasEntityData)
            {
                entity.movement.moveSpeed.y = entity.entityData.entityData["speed"].ToFloat();
                entity.view.view = ComponentUtil.CreateView(entity);
                entity.view.view.CreateBody(entity.entityData.entityData["viewCode"]);
                entity.ReplaceComponent(GameComponentsLookup.View, entity.view);

                {
                    recycleCom.stayTime = 0f;
                    recycleCom.isRecycled = false;
                    //用的是左下角为起点
                    recycleCom.boundary = DirectorUtil.ScreenToWorldRect(DirectorUtil.GetScreenRect());
                }

            }
            return entity;
        }

        public GameEntity CreateBullet(ECampType campType, string code)
        {
            var entity = CreateEntity(code);

            if (campType == ECampType.Hero)
            {
                var heroBulletFlagCom = entity.CreateComponent<HeroBulletFlagComponent>(GameComponentsLookup.HeroBulletFlag);
                entity.AddComponent(GameComponentsLookup.HeroBulletFlag, heroBulletFlagCom);
            }
            else if (campType == ECampType.Entity)
            {
                var entityBulletFlagCom = entity.CreateComponent<EntityBulletFlagComponent>(GameComponentsLookup.EntityBulletFlag);
                entity.AddComponent(GameComponentsLookup.EntityBulletFlag, entityBulletFlagCom);
            }

            return entity;
        }

        public GameEntity CreateBullet(ECampType campType, EBulletType bulletType, EColorType colorType = EColorType.Unknow)
        {
            string code = string.Format("{0}", 10000000 + 100000 * (int)EEntityType.Bullet + 100 * (int)bulletType + (int)colorType);
            return CreateBullet(campType, code);
        }

    }

}
