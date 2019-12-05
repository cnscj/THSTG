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

        //通用方法
        protected GameObject NewViewNode(bool usePool, string viewCode, Vector3 position, Vector3 rotation)
        {
            string viewName = null;
            GameObject viewGO = null;
            GameObject prefabInstance = null;
            if (usePool)
            {
                if (!GameObjectPoolManager.GetInstance().HasGameObjectPool(viewCode))
                {
                    var prefab = AssetManager.GetInstance().LoadSprite(viewCode);
                    if (prefab)
                    {
                        GameObjectPoolManager.GetInstance().NewGameObjectPool(viewCode, prefab);
                    }
                }
                prefabInstance = GameObjectPoolManager.GetInstance().GetGameObject(viewCode);
            }
            else
            {
                var prefab = AssetManager.GetInstance().LoadSprite(viewCode);
                if (prefab)
                {
                    prefabInstance = GameObject.Instantiate(prefab);
                }
            }


            if (!string.IsNullOrEmpty(viewName))
            {
                viewGO = new GameObject(viewName);
                prefabInstance.transform.SetParent(viewGO.transform);
            }
            else
            {
                viewGO = prefabInstance;
            }

            //初始化
            viewGO.transform.localPosition = position;
            viewGO.transform.localEulerAngles = rotation;

            //EEntityType entityType = EntityUtil.GetEntityTypeByCode(viewCode);
            

            return viewGO;
        }

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
            //XXX:用这个法子创建的组件是复用原有的,因此必须手动初始化下
            var transCom = entity.CreateComponent<TransformComponent>(GameComponentsLookup.Transform);
            var movementCom = entity.CreateComponent<MovementComponent>(GameComponentsLookup.Movement);
            var viewCom = entity.CreateComponent<ViewComponent>(GameComponentsLookup.View);
            var destroyCom = entity.CreateComponent<DestroyedComponent>(GameComponentsLookup.Destroyed);

            ////
            transCom.position = Vector3.zero;
            transCom.rotation = Vector3.zero;

            movementCom.moveSpeed = Vector3.zero;
            movementCom.rotationSpeed = Vector3.zero;

            viewCom.collider = null;
            viewCom.animator = null;
            viewCom.renderer = null;
            viewCom.viewGO = null;

            destroyCom.isDestroyed = false;

            ////
            entity.AddComponent(GameComponentsLookup.Transform, transCom);
            entity.AddComponent(GameComponentsLookup.Movement, movementCom);
            entity.AddComponent(GameComponentsLookup.View, viewCom);
            entity.AddComponent(GameComponentsLookup.Destroyed, destroyCom);
        }
    }
}
