
using System.Collections.Generic;
using UnityEngine;
using System;
using XLibrary.Package;
using THGame;
using Entitas;
using XLibrary;

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
            // 获取所需的System组
            __systems = new Feature("Systems")
            .Add(new GameFeature(__contexts))
            .Add(new InputFeature(__contexts));

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
                var eliminateCom = entity.CreateComponent<EliminateComponent>(GameComponentsLookup.Eliminate);
                var healthCom = entity.CreateComponent<HealthComponent>(GameComponentsLookup.Health);
                var boundaryLimitationCom = entity.CreateComponent<BoundaryLimitationComponent>(GameComponentsLookup.BoundaryLimitation);

                entity.AddComponent(GameComponentsLookup.Shot, shotCom);
                entity.AddComponent(GameComponentsLookup.Bomb, bombCom);
                entity.AddComponent(GameComponentsLookup.Eliminate, eliminateCom);
                entity.AddComponent(GameComponentsLookup.Health, healthCom);
                entity.AddComponent(GameComponentsLookup.BoundaryLimitation, boundaryLimitationCom);

                var playerDataCom = entity.GetComponent(GameComponentsLookup.PlayerData) as PlayerDataComponent;
                if (playerDataCom != null)
                {
                    entity.view.viewCode = playerDataCom.modelCode;

                    healthCom.maxBlood = playerDataCom.life;
                    //healthCom.maxArmor = playerDataCom.armor;
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
                entity.AddComponent(GameComponentsLookup.Health, healthCom);

                if(entity.hasEntityData)
                { 
                    //TODO:
                    entity.view.viewCode = entity.entityData.entityData["viewCode"].ToString();
                    entity.movement.moveSpeed.y = 10f;
                    ViewSystemHelper.TryCreateView(entity);
                }

            }
            return entity;
        }

        private void Entity_OnComponentAdded(IEntity entity, int index, IComponent component)
        {
            throw new NotImplementedException();
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

        public GameEntity CreateBullet(ECampType campType, EBulletType bulletType, EColorType colorType = EColorType.Unknow)
        {
            string code = string.Format("{0}", 10000000 + 100000 * (int)EEntityType.Bullet + 100 * (int)bulletType + (int)colorType);
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
                        playerDataCom.life = infos["life"].ToInt();
                        playerDataCom.blood = infos["blood"].ToInt();
                        playerDataCom.armor = infos["armor"].ToInt();
                        playerDataCom.bomb = infos["bomb"].ToInt();
                        playerDataCom.speed = infos["speed"].ToFloat();

                        playerDataCom.modelCode = infos["viewCode"];
                        playerDataCom.bulletCode = infos["bulletCode"];
                        playerDataCom.wingmanCode = infos["wingmanCode"];

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
            var movementCom = entity.CreateComponent<MovementComponent>(GameComponentsLookup.Movement);
            var viewCom = entity.CreateComponent<ViewComponent>(GameComponentsLookup.View);
            var transCom = entity.CreateComponent<TransformComponent>(GameComponentsLookup.Transform);

            entity.AddComponent(GameComponentsLookup.Transform, transCom);
            entity.AddComponent(GameComponentsLookup.Movement, movementCom);
            entity.AddComponent(GameComponentsLookup.View, viewCom);
        }


    }
}
