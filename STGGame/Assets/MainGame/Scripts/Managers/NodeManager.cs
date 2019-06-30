
using UnityEngine;
using THGame;
using System.Collections.Generic;

namespace STGGame
{
    public class NodeManager : SingletonBehaviour<NodeManager>
    {
        public GameObject nodeRoot = null;

        public GameObject stageRoot = null;

        public GameObject heroRoot = null;
        public GameObject bossRoot = null;
        public GameObject mobRoot = null;

        private Dictionary<EEntityType, GameObject> entitiesNode = new Dictionary<EEntityType, GameObject>();

        private void Awake()
        {
            nodeRoot = new GameObject("NodeRoot");
            nodeRoot.transform.SetParent(gameObject.transform, true);
            {
                stageRoot = new GameObject("StageRoot");
                stageRoot.transform.SetParent(nodeRoot.transform, true);
                {
                    heroRoot = new GameObject("PlayerRoot");
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
            if(entitiesNode.ContainsKey(type))
            {
                return entitiesNode[type];
            }
            return null;
        }

        private NodeManager(){ }

    }
}