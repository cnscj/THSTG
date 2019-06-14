
using UnityEngine;
using THGame;
namespace STGGame
{
    public class NodeManager : SingletonBehaviour<NodeManager>
    {
        public GameObject nodeRoot = null;

        public GameObject stageRoot = null;

        public GameObject effectRoot = null;
        public GameObject playerRoot = null;
        public GameObject enemyRoot = null;

        private void Start()
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

        private NodeManager(){ }

    }
}