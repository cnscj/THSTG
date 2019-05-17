
using UnityEngine;
using Unity.Entities;
namespace STGGame
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager instance { get; private set; } = null;
        public World world { get; private set; } = null;

        public GameObject nodeRoot { get; private set; } = null;

        public GameObject stageRoot { get; private set; } = null;

        public GameObject effectRoot { get; private set; } = null;
        public GameObject playerRoot { get; private set; } = null;
        public GameObject enemyRoot { get; private set; } = null;

        //
        GameManager()
        {

        }

        void Awake()
        {
            instance = this;
        }

        void Start()
        {
            InitGame();
            InitNode();
            InitGameObject();
        }

        //游戏初始化
        void InitGame()
        {
            //初始化ECS-World
            //world = new World("Game");
            //World.Active = world;

            ////场景实体管理器
            //StageEntityManager.instance.manager = world.GetOrCreateManager<EntityManager>();
        }

        //初始化节点
        void InitNode()
        {
           
            nodeRoot = new GameObject("NodeRoot");
            nodeRoot.transform.SetParent(gameObject.transform, true);
            {
                stageRoot = new GameObject("StageRoot");
                stageRoot.transform.SetParent(nodeRoot.transform, true);
                {
                    playerRoot = new GameObject("PlayerRoot");
                    playerRoot.transform.SetParent(stageRoot.transform, true);

                    enemyRoot = new GameObject("EnemyRoot");
                    enemyRoot.transform.SetParent(stageRoot.transform, true);

                    effectRoot = new GameObject("EffectRoot");
                    effectRoot.transform.SetParent(stageRoot.transform, true);
                }
            }
        }

        //初始化实体
        void InitGameObject()
        {
            //StageEntityManager.instance.CreatePlayer(1);
            StageEntityManager.instance.CreateEnemy();
        }

    }
}