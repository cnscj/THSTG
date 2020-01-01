using System.Collections;
using System.Collections.Generic;
using STGGame;
using UnityEngine;

namespace STGU3D
{
    public class BulletFactory : BaseEntityFactory
    {
        protected override GameEntity OnCreate(string code)
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
                entity.ReplaceComponent(GameComponentsLookup.View, entity.view);

                {
                    recycleCom.maxStayTime = 1f;
                    recycleCom.stayTime = 0f;
                    recycleCom.isRecycled = false;
                    //用的是左下角为起点
                    recycleCom.boundary = DirectorUtil.ScreenRectInWorld(DirectorUtil.GetScreenRect());
                }

                {
                    healthCom.blood = healthCom.maxBlood;
                    healthCom.trueDeathTime = -1f;
                    healthCom.isTrueDied = false;
                }

                {
                    entity.collider.obj.data = entity;
                    entity.collider.obj.AddShape(new CircleCollider()
                    {
                        radius = 0.1f
                    });
                }

                //TODO:先简单通过种类区分阵营
                var campType = (ECampType)entity.entityData.entityData["category"].ToInt();
                if (campType == ECampType.Hero)
                {
                    var heroBulletFlagCom = entity.CreateComponent<HeroBulletFlagComponent>(GameComponentsLookup.HeroBulletFlag);

                    {
                        entity.collider.tag = ColliderType.HeroBullet;
                        entity.collider.mask = ColliderType.Mob | ColliderType.Boss;
                    }


                    entity.AddComponent(GameComponentsLookup.HeroBulletFlag, heroBulletFlagCom);
                }
                else if (campType == ECampType.Entity)
                {
                    var entityBulletFlagCom = entity.CreateComponent<EntityBulletFlagComponent>(GameComponentsLookup.EntityBulletFlag);

                    {
                        entity.collider.tag = ColliderType.EntityBullet;
                        entity.collider.mask = 0;
                    }

                    entity.AddComponent(GameComponentsLookup.EntityBulletFlag, entityBulletFlagCom);
                }
                entity.bulletData.campType = campType;

            }
            return entity;
        }

        public GameEntity CreateBullet(ECampType campType, string code)
        {
            var entity = CreateEntity(code);
            entity.bulletData.campType = campType;

            return entity;
        }

        public GameEntity CreateBullet(ECampType campType, int bulletType, EColorType colorType = EColorType.Unknow)
        {
            int bulletCode = 100 * bulletType + (int)colorType;
            string code = EntityUtil.GetBulletCode(bulletCode);
            return CreateBullet(campType, code);
        }

    }

}
