
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using THGame;
namespace STGGame
{
    public class EntityManager : SingletonBehaviour<StageEntityManager>
    {
        public World world { get; private set; } = null;

        public GameObject PlayerTmpl;
        public GameObject MobTmpl;

        [SerializeField] public List<GameObject> players = new List<GameObject>();
        [SerializeField] public List<GameObject> mobs = new List<GameObject>();


        public GameObject CreatePlayer(EPlayerType type)
        {
            GameObject player = Instantiate(PlayerTmpl, NodeManager.GetInstance().playerRoot.transform);
            player.GetComponent<PlayerDataComponent>().playerType = type;

            return player;
        }

        public GameObject CreateMob()
        {
            return null;
        }

        private GameObject CreateEntity(int code)
        {
            return null;
        }

        private void Start()
        {
            //初始化ECS-World
            world = new World("Game");
            World.Active = world;
        }

        private EntityManager()
        {

        }

    }
}