
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using THGame;
namespace STGGame
{
    public class StageEntityManager : SingletonBehaviour<StageEntityManager>
    {

        //[SerializeField] public List<GameObject> players = new List<GameObject>();
        //[SerializeField] public List<GameObject> emenies = new List<GameObject>();

        //public EntityManager manager;

        //private StageEntityManager(){ }

        //void Awake()
        //{

        //}

        //GameObject CreateEmptyEntity(string name)
        //{
        //    GameObject GO = new GameObject(name);


        //    return GO;
        //}

        ////GameObjectEntity必须最后添加才起效
        //GameObject ReturnEntityGO(GameObject GO)
        //{
        //    if (GO)
        //    {
        //        //依据Code创建实体
        //        if (!GO.GetComponent<GameObjectEntity>())
        //        {
        //            GO.AddComponent<GameObjectEntity>();
        //        }
        //    }
        //    return GO;
        //}

        //public GameObject CreatePlayer(EPlayerType type)
        //{
        //    GameObject player = CreateMoveable();
        //    player.name = string.Format("Player_{0}", type);
        //    //根节点组件
        //    var playerDataComp = player.GetComponent<PlayerDataComponent>();
        //    playerDataComp.playerType = type;

        //    //子节点


        //    player.transform.SetParent(GameManager.GetInstance().playerRoot.transform);
        //    players.Add(player);

        //    return ReturnEntityGO(player);
        //}

        //public GameObject CreateEnemy()
        //{
        //    GameObject enemy = CreateMoveable("Enemy");
        //    //根节点组件
        //    var rotateComp = enemy.AddComponent<RotateComponent>();
        //    rotateComp.speed = 100;

        //    //子节点
        //    Transform modelNode = enemy.transform.Find("Local/Model");
        //    GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
        //    cube.transform.SetParent(modelNode);

        //    enemy.transform.SetParent(GameManager.GetInstance().enemyRoot.transform);
        //    emenies.Add(enemy);

        //    return ReturnEntityGO(enemy);
        //}

        //private GameObject CreateMoveable(string name = "Entity")
        //{
        //    GameObject GO = CreateEmptyEntity(name);
        //    GO.AddComponent<ModelComponent>();

        //    //子节点
        //    GameObject localNode = new GameObject("Local");
        //    localNode.transform.SetParent(GO.transform);

        //    GameObject modelNode = new GameObject("Model");
        //    modelNode.transform.SetParent(localNode.transform);

        //    return GO;
        //}

        //////
        //GameObject CreateEntity(int code, string name)
        //{
        //    GameObject GO = CreateEmptyEntity(name);

        //    return GO;
        //}





    }
}