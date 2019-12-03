using System.Collections;
using System.Collections.Generic;
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
                //TODO:
                entity.transform.rotation.z = 90;
                entity.movement.moveSpeed.y = 8f;
                entity.view.viewGO = NewViewNode(true, entity.entityData.entityData["viewCode"], entity.transform.position, entity.transform.rotation);
                entity.ReplaceComponent(GameComponentsLookup.View, entity.view);

                {
                    Vector3 v3 = Camera.main.ScreenToWorldPoint(Vector3.zero);
                    var winSize = new Vector2(Mathf.Abs(v3.x) * 2, Mathf.Abs(v3.y) * 2);
                    var pixelPerPot = winSize.x / Screen.width;

                    recycleCom.stayTime = 0f;
                    recycleCom.isRecycled = false;
                    //用的是左下角为起点
                    recycleCom.boundary = new Rect(-pixelPerPot * Screen.width * 0.5f, -pixelPerPot * Screen.height * 0.5f, pixelPerPot * Screen.width * 0.5f, pixelPerPot * Screen.height * 0.5f);
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
