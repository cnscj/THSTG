
using System.Collections.Generic;
using UnityEngine;
using System;
using XLibrary.Package;
using THGame;
using Entitas;
using XLibrary;
using XLibGame;

namespace STGU3D
{
    public class EntityManager : MonoSingleton<EntityManager>
    {
        public GameObject stageRoot;
        public GameObject mapRoot;
        public GameObject heroRoot;
        public GameObject bossRoot;
        public GameObject mobRoot;

        private Contexts __contexts;
        private Systems __systems;

        private void Awake()
        {
            // 获取Entitas的上下文对象，类似一个单例管理器
            __contexts = Contexts.sharedInstance;

            // 获取所需的System组
            __systems = new Feature("Systems")
            .Add(new GameFeature(__contexts))
            .Add(new InputFeature(__contexts));

            stageRoot = new GameObject("StageRoot");
            stageRoot.transform.SetParent(gameObject.transform, true);
            {

                mapRoot = new GameObject("MapRoot");
                mapRoot.transform.SetParent(stageRoot.transform, true);

                heroRoot = new GameObject("HeroRoot");
                heroRoot.layer = 10;
                heroRoot.transform.SetParent(stageRoot.transform, true);

                bossRoot = new GameObject("BossRoot");
                bossRoot.transform.SetParent(stageRoot.transform, true);

                mobRoot = new GameObject("MobRoot");
                mobRoot.transform.SetParent(stageRoot.transform, true);
            }
        }

        private void Start()
        {
            // 初始化System
            __systems.Initialize();
        }

        private void Update()
        {
            __systems.Execute();
            __systems.Cleanup();
        }

        private void OnDestroy()
        {
            __systems.TearDown();
        }
        ///////////////////////
        public GameObject NewViewNode(bool usePool, string viewCode, Vector3 position, Vector3 rotation)
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

            viewGO.transform.localPosition = position;
            viewGO.transform.localEulerAngles = rotation;

            return viewGO;
        }

        ///////////////////////
        public void DestroyEntity(GameEntity entity)
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
            var entity = __contexts.game.CreateEntity();

            return entity;
        }

        public GameEntity CreateGameEntity(string code)
        {
            var entity = CreateEmptyEntity();
            AddCommonComponent(entity);
            AddEntityDataComponent(entity, code);
            return entity;
        }

        //单独处理独特组件
        public GameEntity CreateEntity(string code)
        {
            var entity = CreateGameEntity(code);
            EEntityType entityType = EntityUtil.GetEntityTypeByCode(code);

            if (entityType == EEntityType.Hero)
            {
                var shotCom = entity.CreateComponent<ShotComponent>(GameComponentsLookup.Shot);
                var bombCom = entity.CreateComponent<BombComponent>(GameComponentsLookup.Bomb);
                var decelerateCom = entity.CreateComponent<DecelerateComponent>(GameComponentsLookup.Decelerate);
                var eliminateCom = entity.CreateComponent<EliminateComponent>(GameComponentsLookup.Eliminate);
                var healthCom = entity.CreateComponent<HealthComponent>(GameComponentsLookup.Health);
                var cageCom = entity.CreateComponent<CageComponent>(GameComponentsLookup.Cage);

                entity.AddComponent(GameComponentsLookup.Shot, shotCom);
                entity.AddComponent(GameComponentsLookup.Bomb, bombCom);
                entity.AddComponent(GameComponentsLookup.Decelerate, decelerateCom);
                entity.AddComponent(GameComponentsLookup.Eliminate, eliminateCom);
                entity.AddComponent(GameComponentsLookup.Health, healthCom);
                entity.AddComponent(GameComponentsLookup.Cage, cageCom);

                if (entity.hasEntityData)
                {
                    //根据
                    shotCom.action = (shotEntity) =>
                    {
                        var bulletEntity = EntityManager.GetInstance().CreateBullet(ECampType.Hero, shotEntity.entityData.entityData["bulletCode"]);
                        bulletEntity.transform.position = shotEntity.transform.position;            //在自机处生成
                        bulletEntity.view.viewGO.transform.position = shotEntity.transform.position;//覆盖第一帧刷新


                        return bulletEntity;
                    };
                   
                    entity.view.viewGO = NewViewNode(false, entity.entityData.entityData["viewCode"],entity.transform.position, entity.transform.rotation);
                    entity.ReplaceComponent(GameComponentsLookup.View, entity.view);


                    entity.playerData.moveSpeed = entity.entityData.entityData["speed"].ToFloat();
                    healthCom.maxBlood = entity.entityData.entityData["blood"].ToInt();
                }
            }
            else if(entityType == EEntityType.Mob)
            {
                var healthCom = entity.CreateComponent<HealthComponent>(GameComponentsLookup.Health);

                entity.AddComponent(GameComponentsLookup.Health, healthCom);

                var mobDataCom = entity.GetComponent(GameComponentsLookup.MobData) as MobDataComponent;
                if (mobDataCom != null)
                {

                }

            }
            else if (entityType == EEntityType.Bullet)
            {
                var healthCom = entity.CreateComponent<HealthComponent>(GameComponentsLookup.Health);
                var recycleCom = entity.CreateComponent<RecycleComponent>(GameComponentsLookup.Recycle);

                {
                    Vector3 v3 = Camera.main.ScreenToWorldPoint(Vector3.zero);
                    var winSize = new Vector2(Mathf.Abs(v3.x) * 2, Mathf.Abs(v3.y) * 2);
                    var pixelPerPot = winSize.x / Screen.width;

                    recycleCom.stayTime = 0f;
                    recycleCom.isRecycled = false;
                    //用的是左下角为起点
                    recycleCom.boundary = new Rect(-pixelPerPot * Screen.width * 0.5f,- pixelPerPot * Screen.height * 0.5f, pixelPerPot * Screen.width, pixelPerPot * Screen.height);
                }

                entity.AddComponent(GameComponentsLookup.Health, healthCom);
                entity.AddComponent(GameComponentsLookup.Recycle, recycleCom);

                if (entity.hasEntityData)
                {
                    //TODO:
                    entity.transform.rotation.z = 90;
                    entity.movement.moveSpeed.y = 8f;
                    entity.view.viewGO = NewViewNode(true, entity.entityData.entityData["viewCode"], entity.transform.position, entity.transform.rotation);
                    entity.ReplaceComponent(GameComponentsLookup.View, entity.view);

                }

            }

            return entity;
        }

        public GameEntity CreateHero(EHeroType type, EPlayerType playerType = EPlayerType.Player01)
        {
            string code = string.Format("{0}", 10000000 + 100000 * (int)EEntityType.Hero + 1000 * (int)type + 1);
            var entity = CreateEntity(code);

            var playerDataCom = entity.GetComponent(GameComponentsLookup.PlayerData) as PlayerDataComponent;
            if (playerDataCom != null)
            {
                playerDataCom.playerType = playerType;
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

        //public GameEntity CreateWingman()
        //{
        //    string code = string.Format("{0}", 10000000 + 100000 * (int)EEntityType.Hero + 1000 * (int)type + 1);
        //    var entity = CreateEntity(code);

        //    var wingmanDataCom = entity.GetComponent(GameComponentsLookup.WingmanData) as WingmanDataComponent;
        //    if (wingmanDataCom != null)
        //    {
                
        //    }
        //    return entity;
        //}


        //Data处理
        private void AddEntityDataComponent(GameEntity entity, string code)
        {
            EEntityType type = EntityUtil.GetEntityTypeByCode(code);
            CSVObject infos = EntityConfiger.GetEntityInfo(code);
            
            if (infos != null)
            {
                var entityDataCom = entity.CreateComponent<EntityDataComponent>(GameComponentsLookup.EntityData);
                entityDataCom.entityCode = code;
                entityDataCom.entityType = type;
                entityDataCom.entityData = infos;
                switch (type)
                {
                    case EEntityType.Hero:
                        var playerDataCom = entity.CreateComponent<PlayerDataComponent>(GameComponentsLookup.PlayerData);
                        var heroType = EntityUtil.GetHeroTypeByCode(code);
                        playerDataCom.heroType = heroType;

                        entity.AddComponent(GameComponentsLookup.PlayerData, playerDataCom);
                        break;
                    case EEntityType.Wingman:
                        var wingmanDataCom = entity.CreateComponent<WingmanDataComponent>(GameComponentsLookup.WingmanData);

                        entity.AddComponent(GameComponentsLookup.WingmanData, wingmanDataCom);
                        break;
                    case EEntityType.Mob:
                        var mobDataCom = entity.CreateComponent<MobDataComponent>(GameComponentsLookup.MobData);

                        entity.AddComponent(GameComponentsLookup.MobData, mobDataCom);
                        break;
                    case EEntityType.Boss:
                        var bossDataCom = entity.CreateComponent<MobDataComponent>(GameComponentsLookup.BossData);

                        entity.AddComponent(GameComponentsLookup.BossData, bossDataCom);
                        break;
                    case EEntityType.Bullet:
                        var bulletDataCom = entity.CreateComponent<BulletDataComponent>(GameComponentsLookup.BulletData);

                        entity.AddComponent(GameComponentsLookup.BulletData, bulletDataCom);
                        break;
                }


                entity.AddComponent(GameComponentsLookup.EntityData, entityDataCom);
            }

        }

        private void AddCommonComponent(GameEntity entity)
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

            ////
            entity.AddComponent(GameComponentsLookup.Transform, transCom);
            entity.AddComponent(GameComponentsLookup.Movement, movementCom);
            entity.AddComponent(GameComponentsLookup.View, viewCom);
            entity.AddComponent(GameComponentsLookup.Destroyed, destroyCom);
        }


    }
}
