
using System.Collections.Generic;
using UnityEngine;
using System;
using XLibrary.Package;

namespace STGU3D
{
    public class EntityManager : MonoSingleton<EntityManager>
    {
        [HideInInspector] public GameObject stageRoot;
        [HideInInspector] public GameObject mapRoot;
        [HideInInspector] public GameObject heroRoot;
        [HideInInspector] public GameObject bossRoot;
        [HideInInspector] public GameObject mobRoot;

        [HideInInspector] public GameObject PlayerPrefab;
        [HideInInspector] public GameObject MobPrefab;
        [HideInInspector] public GameObject BossPrefab;
        [HideInInspector] private Dictionary<EEntityType, GameObject> entitiesNode = new Dictionary<EEntityType, GameObject>();
        [HideInInspector] [SerializeField] public List<GameObject> heros = new List<GameObject>();
        [HideInInspector] [SerializeField] public List<GameObject> mobs = new List<GameObject>();
        [HideInInspector] [SerializeField] public List<GameObject> bosses = new List<GameObject>();

        public GameObject CreatePlayer(EPlayerType type = EPlayerType.Player01)
        {
            var entity = CreateEntity(PlayerPrefab);
            //修改玩家类型
            var playerData = entity.GetComponent<PlayerDataComponent>();
            playerData.playerType = type;
            playerData.heroType = EHeroType.Reimu;

            //赋予可视化身体
            var displayComp = entity.GetComponent<DisplayComponent>();
            displayComp.spriteCode = "400001";  //TODO:应该读取配置

            //修改GameObject名称
            entity.name = string.Format("{0}_{1:D2}", "Player", (int)type);

            //层级设置
            int layerIndex = LayerMask.NameToLayer("Hero");
            entity.layer = layerIndex;

            return entity;
        }

        //小怪,敌机子弹,全部当成Mob处理,用EntityType区别(因为是大量出现的东西)
        public GameObject CreateMob()
        {
            var entity = CreateEntity(MobPrefab);
            return entity;
        }

        public GameObject CreateBoss()
        {
            var entity = CreateEntity(BossPrefab);
            return entity;
        }

        public GameObject CreateEntity(GameObject entityPrefab,Action<GameObject> initFunc = null)
        {
            if (entityPrefab == null)
            {
                return null;
            }
            else
            {
                var entities = CreateEntities(entityPrefab, 1);
                var entity = entities[0];
                initFunc?.Invoke(entity);
                return entity;
            }
        }

        public GameObject[] CreateEntities(GameObject entityPrefab, int amount, Action<GameObject,int> initFunc = null)
        {
            if (entityPrefab == null)
                return null;

            GameObject fatherNode = GetNodeByEntity(entityPrefab);
            List<GameObject> list = GetNodeListByEntity(entityPrefab);

            GameObject[] entities = new GameObject[amount];
            for(int i = 0; i < amount; i++)
            {
                entities[i] = Instantiate(entityPrefab, fatherNode.transform);
                initFunc?.Invoke(entities[i], i);
                list.Add(entities[i]);
            }
            return entities;
        }


        //通过唯一索引编号
        public GameObject CreateEntity(int code, Action<GameObject> initFunc = null)
        {
            return null;
        }

        public GameObject[] CreateEntities(int code, int amount, Action<GameObject, int> initFunc = null)
        {
         
            return null;
        }

        ///
        private List<GameObject> GetNodeListByEntity(GameObject entity)
        {
            if (entity)
            {
                var entityData = entity.GetComponent<EntityDataComponent>();
                if (entityData)
                {
                    var entityType = entityData.entityType;
                    switch (entityType)
                    {
                        case EEntityType.Hero:
                            return heros;
                        case EEntityType.Mob:
                            return mobs;
                        case EEntityType.Boss:
                            return bosses;
                    }
                }
            }
            return null;
            
        }

        private void Awake()
        {
            stageRoot = new GameObject("StageRoot");
            stageRoot.transform.SetParent(gameObject.transform, true);
            {

                mapRoot = new GameObject("MapRoot");
                mapRoot.transform.SetParent(stageRoot.transform, true);

                heroRoot = new GameObject("HeroRoot");
                heroRoot.layer = 10;
                heroRoot.transform.SetParent(stageRoot.transform, true);
                entitiesNode.Add(EEntityType.Hero, heroRoot);

                bossRoot = new GameObject("BossRoot");
                bossRoot.transform.SetParent(stageRoot.transform, true);
                entitiesNode.Add(EEntityType.Boss, bossRoot);

                mobRoot = new GameObject("MobRoot");
                mobRoot.transform.SetParent(stageRoot.transform, true);
                entitiesNode.Add(EEntityType.Mob, mobRoot);
            }
        }

        public GameObject GetNodeByEntity(GameObject entity)
        {
            if (entity)
            {
                var entityData = entity.GetComponent<EntityDataComponent>();
                if (entityData)
                {
                    var entityType = entityData.entityType;
                    return GetNodeEntityByType(entityType);
                }
            }
            return null;
        }

        public GameObject GetNodeEntityByType(EEntityType type)
        {
            if (entitiesNode.ContainsKey(type))
            {
                return entitiesNode[type];
            }
            return null;
        }

        private EntityManager()
        {
            
        }

    }
}
