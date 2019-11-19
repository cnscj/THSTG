
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
        }

        ///////////////////////

        public GameEntity CreateEntity(string code)
        {
            var entity = __contexts.game.CreateEntity();
            AddCommonComponent(entity);
            AddEntityDataComponent(entity, code);
            return entity;
        }

        public GameEntity CreateHero(EHeroType type, EPlayerType playerType = EPlayerType.Player01)
        {
            string code = string.Format("{0}", 10100001 + (int)type * 1000) ;
            var entity = CreateEntity(code);

            var playerDataCom = entity.GetComponent(GameComponentsLookup.PlayerData) as PlayerDataComponent;
            if (playerDataCom != null) playerDataCom.playerType = playerType;

            return entity;
        }

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
                        playerDataCom.armor = infos["armor"].ToInt();
                        playerDataCom.bomb = infos["bomb"].ToInt();
                        playerDataCom.speed = infos["speed"].ToFloat();

                        playerDataCom.modelCode = infos["modelCode"];
                        playerDataCom.bulletCode = infos["bulletCode"];
                        playerDataCom.wingmanCode = infos["wingmanCode"];

                        entity.AddComponent(GameComponentsLookup.PlayerData, playerDataCom);
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
