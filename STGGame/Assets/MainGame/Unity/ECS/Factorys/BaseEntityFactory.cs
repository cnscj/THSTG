using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XLibGame;
using XLibrary;

namespace STGU3D
{
    public abstract class BaseEntityFactory
    {
        public abstract GameEntity CreateEntity(string code);

        public virtual void DestroyEntity(GameEntity entity)
        {
            if (entity.hasDestroyed)
            {
                entity.destroyed.isDestroyed = true;
            }
            else
            {
                entity.Destroy();
            }
        }
        public GameEntity CreateEmptyEntity()
        {
            var entity = Contexts.sharedInstance.game.CreateEntity();

            return entity;
        }

        public GameEntity CreateGameEntity(string code)
        {
            var entity = CreateEmptyEntity();
            AddCommonComponent(entity);
            AddEntityDataComponent(entity, code);
            return entity;
        }

        //转换方法
        public HeroFactory AsHero(){ return (HeroFactory)this; }
        public BulletFactory AsBullet() { return (BulletFactory)this; }
        public WingmanFactory AsWingman() { return (WingmanFactory)this; }
        public MobFactory AsMob() { return (MobFactory)this; }
        //通用方法


        protected void AddEntityDataComponent(GameEntity entity, string code)
        {
            EEntityType type = EntityUtil.GetEntityTypeByCode(code);
            CSVObject infos = EntityConfiger.GetEntityInfo(code);

            if (infos != null)
            {
                var entityDataCom = entity.CreateComponent<EntityDataComponent>(GameComponentsLookup.EntityData);
                entityDataCom.entityCode = code;
                entityDataCom.entityType = type;
                entityDataCom.entityData = infos;
               
                entity.AddComponent(GameComponentsLookup.EntityData, entityDataCom);
            }

        }

        protected void AddCommonComponent(GameEntity entity)
        {
            //用这个法子创建的组件是复用原有的,因此必须手动初始化下
            var transCom = entity.CreateComponent<TransformComponent>(GameComponentsLookup.Transform);
            var movementCom = entity.CreateComponent<MovementComponent>(GameComponentsLookup.Movement);
            var viewCom = entity.CreateComponent<ViewComponent>(GameComponentsLookup.View);
            var destroyCom = entity.CreateComponent<DestroyedComponent>(GameComponentsLookup.Destroyed);

            ////
            ComponentUtil.ClearTransform(transCom);
            ComponentUtil.ClearMovement(movementCom);
            ComponentUtil.ClearView(viewCom);
            ComponentUtil.ClearDestroyed(destroyCom);

            ////
            entity.AddComponent(GameComponentsLookup.Transform, transCom);
            entity.AddComponent(GameComponentsLookup.Movement, movementCom);
            entity.AddComponent(GameComponentsLookup.View, viewCom);
            entity.AddComponent(GameComponentsLookup.Destroyed, destroyCom);
        }
    }
}
