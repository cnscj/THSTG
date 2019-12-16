
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
        //实体工厂
        public static readonly Dictionary<EEntityType, Type> s_factoryType = new Dictionary<EEntityType, Type>()
        {
            [EEntityType.Hero] = typeof(HeroFactory),
            [EEntityType.Wingman] = typeof(WingmanFactory),
            [EEntityType.Mob] = typeof(MobFactory),
            [EEntityType.Boss] = typeof(BossFactory),
            [EEntityType.Bullet] = typeof(BulletFactory),
            [EEntityType.Prop] = typeof(PropFactory),
        };
        public GameObject stageRoot;
        public GameObject mapRoot;
        public GameObject heroRoot;
        public GameObject bossRoot;
        public GameObject mobRoot;
        public GameObject bulletRoot;
        public GameObject propRoot;
        public GameObject wingmanRoot;

        public Dictionary<EEntityType, BaseEntityFactory> entityFactoryMap;

        private Contexts __contexts;
        private Systems __systems;

        private void Awake()
        {
            // 获取Entitas的上下文对象，类似一个单例管理器
            __contexts = Contexts.sharedInstance;

            // 获取所需的System组
            __systems = new Feature("Systems")
            .Add(new GameFeature(__contexts))
            .Add(new InputFeature(__contexts))
            .Add(new UIFeature(__contexts));

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

                bulletRoot = new GameObject("BulletRoot");
                bulletRoot.transform.SetParent(stageRoot.transform, true);

                propRoot = new GameObject("PropRoot");
                propRoot.transform.SetParent(stageRoot.transform, true);

                wingmanRoot = new GameObject("WingmanRoot");
                wingmanRoot.transform.SetParent(stageRoot.transform, true);
            }

            entityFactoryMap = new Dictionary<EEntityType, BaseEntityFactory>();
            
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
        ////////////////////////////

        public BaseEntityFactory GetOrNewEntityFactory(EEntityType entityType)
        {
            BaseEntityFactory factory = null;
            if (!entityFactoryMap.TryGetValue(entityType, out factory))
            {
                Type clsType = null;
                if (s_factoryType.TryGetValue(entityType,out clsType))
                {
                    factory = (BaseEntityFactory)Activator.CreateInstance(clsType);
                }

                if (factory != null)
                {
                    entityFactoryMap.Add(entityType, factory);
                }
            }

            return factory;
        }
        ////////////////////////////
        public void DestroyEntity(GameEntity entity)
        {
            if (entity != null && entity.hasEntityData)
            {
                EEntityType entityType = entity.entityData.entityType;
                BaseEntityFactory factory = GetOrNewEntityFactory(entityType);
                if (factory != null)
                {
                    factory.DestroyEntity(entity);
                }
                else
                {
                    entity.Destroy();
                }
            }
        }

        public GameEntity CreateEntity(string code)
        {
            EEntityType entityType = EntityUtil.GetEntityTypeByCode(code);
            BaseEntityFactory factory = GetOrNewEntityFactory(entityType);
            if (factory != null)
            {
                return factory.CreateEntity(code);

            }

            return null;
        }
    }
}
