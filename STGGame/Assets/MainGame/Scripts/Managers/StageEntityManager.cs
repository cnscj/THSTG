using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Transforms;

namespace STGGame
{
    public class StageEntityManager : MonoBehaviour
    {
        public static StageEntityManager instance { get; private set; } = null;

        [SerializeField] public List<GameObject> players = new List<GameObject>();
        [SerializeField] public List<GameObject> emenies = new List<GameObject>();

        private EntityManager m_manager;

        void Awake()
        {
            instance = this;
            //m_manager = GameManager.instance.world.CreateManager<EntityManager>();
        }

        public GameObject CreatePlayer(int type)
        {
            GameObject player = CreateMoveable();
            player.name = string.Format("Player_{0}", type);
            //根节点组件
            var inputComp = player.AddComponent<PlayerInputCompnent>();
            inputComp.type = type;

            //子节点


            player.transform.SetParent(GameManager.instance.playerRoot.transform);
            players.Add(player);

            return player;
        }

        public GameObject CreateEnemy()
        {
            GameObject enemy = CreateMoveable("Enemy");
            //根节点组件
            var rotateComp = enemy.AddComponent<RotateComponent>();
            rotateComp.speed = 100;

            //子节点
            Transform modelNode = enemy.transform.Find("Local/Model");
            GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
            cube.transform.SetParent(modelNode);

            enemy.transform.SetParent(GameManager.instance.enemyRoot.transform);
            emenies.Add(enemy);

            return enemy;
        }

        private GameObject CreateMoveable(string name = "Entity")
        {

            //EntityArchetype entityArchetype = m_manager.CreateArchetype(typeof(Position), typeof(Rotation));
            //Entity entity1 = m_manager.CreateEntity(entityArchetype);

            GameObject entity = new GameObject(name);
            entity.AddComponent<ModelComponent>();

            //子节点
            GameObject localNode = new GameObject("Local");
            localNode.transform.SetParent(entity.transform);

            GameObject modelNode = new GameObject("Model");
            modelNode.transform.SetParent(localNode.transform);

            return entity;
        }


    }
}