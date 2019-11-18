
using System.Collections.Generic;
using UnityEngine;
using System;
using XLibrary.Package;
using THGame;
using Entitas;

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

            return entity;
        }

        public GameEntity CreateHero()
        {
            var entity = __contexts.game.CreateEntity();
            var transCom = entity.CreateComponent<TransformComponent>(GameComponentsLookup.Transform);
            var entityDataCom = entity.CreateComponent<EntityDataComponent>(GameComponentsLookup.EntityData);
            var playerDataCom = entity.CreateComponent<PlayerDataComponent>(GameComponentsLookup.PlayerData);
            var movementCom = entity.CreateComponent<MovementComponent>(GameComponentsLookup.Movement);
            var viewCom = entity.CreateComponent<ViewComponent>(GameComponentsLookup.View);

            entity.AddComponent(GameComponentsLookup.Transform, transCom);
            entity.AddComponent(GameComponentsLookup.EntityData, entityDataCom);
            entity.AddComponent(GameComponentsLookup.PlayerData, playerDataCom);
            entity.AddComponent(GameComponentsLookup.Movement, movementCom);
            entity.AddComponent(GameComponentsLookup.View, viewCom);

            return entity;
        }

        private void AddEntityDataComponent(GameEntity entity,string code)
        {

        }

        private void AddCommonComponent(GameEntity entity)
        {

        }


    }
}
