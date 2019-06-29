
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using THGame;
using Unity.Collections;
using System;

namespace STGGame
{
    public class EntityManager : SingletonBehaviour<EntityManager>
    {
        public World world { get; private set; } = null;
        public Unity.Entities.EntityManager manager { get; private set; } = null;

        public GameObject PlayerPrefab;
        public GameObject MobPrefab;
        public GameObject BossPrefab;

        [SerializeField] public List<GameObject> players = new List<GameObject>();
        [SerializeField] public List<GameObject> mobs = new List<GameObject>();
        [SerializeField] public List<GameObject> bosses = new List<GameObject>();

        public Entity CreatePlayer(EPlayerType type = EPlayerType.Player01)
        {
            var entity = CreateEntity(PlayerPrefab);
            //修改玩家类型
            var playerData = manager.GetComponentData<PlayerData>(entity);
            playerData.playerType = type;
            return entity;
        }

        //小怪,敌机子弹,全部当成Mob处理,用EntityType区别(因为是大量出现的东西)
        public Entity CreateMob()
        {
            var entity = CreateEntity(MobPrefab);
            return entity;
        }

        public Entity CreateBoss()
        {
            var entity = CreateEntity(BossPrefab);
            return entity;
        }

        public Entity CreateEntity(GameObject entityPrefab,Action<Entity> initFunc = null)
        {
            if (entityPrefab == null)
            {
                entityPrefab = new GameObject();
                var entities = CreateEntities(entityPrefab, 1);
                var entity = entities[0];
                manager.AddComponentData(entity, new Position { position = new Vector3(0, 0, 0) });
                manager.AddComponentData(entity, new Rotation { rotation = new Vector3(0, 0, 0) });
                manager.AddComponentData(entity, new Movement { jumpSpeed = 0, moveSpeed = 0, rotateSpeed = 0});
                initFunc?.Invoke(entity);
                return entity;
            }
            else
            {
                var entities = CreateEntities(entityPrefab, 1);
                var entity = entities[0];
                initFunc?.Invoke(entity);
                return entity;
            }
        }

        public Entity[] CreateEntities(GameObject entityPrefab, int amount, Action<Entity,int> initFunc = null)
        {
            NativeArray<Entity> entities = new NativeArray<Entity>(amount, Allocator.Temp);
            manager.Instantiate(entityPrefab, entities);
            if (initFunc != null)
            {
                for (int i = 0; i < amount; i++)
                {
                    initFunc(entities[i], i);
                }
            }
            var array = entities.ToArray();
            entities.Dispose();
            return array;
        }


        ///

        private void Awake()
        {
            //初始化ECS-World
            world = World.Active;   //直接使用默认的
            manager = world.GetOrCreateManager<Unity.Entities.EntityManager>();
        }

        private EntityManager()
        {
            
        }

    }
}
