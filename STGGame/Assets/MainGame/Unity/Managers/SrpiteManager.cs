
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using XLibGame;
using XLibrary.Package;

namespace STGU3D
{
    public class SpriteManager : MonoSingleton<SpriteManager>
    {
        public class PoolInfo
        {
            public int maxCount = 20;
            public int defaultCount = 0;
        }
        public Dictionary<EEntityType, PoolInfo> poolInfos = new Dictionary<EEntityType, PoolInfo>()
        {
            [EEntityType.Hero] = new PoolInfo(),
            [EEntityType.Boss] = new PoolInfo(),
        };

        public GameObject GetOrNewSprite(string viewCode, bool usePool = false, int maxCount = 20)
        {
            string viewName = null;
            GameObject prefabInstance = null;
            if (!usePool)
            {
                EEntityType entityType = EntityUtil.GetEntityTypeByCode(viewCode);
                if (poolInfos.TryGetValue(entityType, out var poolInfo))
                {
                    usePool = true;
                    maxCount = poolInfo.maxCount;
                }
            }

            if (usePool)
            {
                if (!GameObjectPoolManager.GetInstance().HasGameObjectPool(viewCode))
                {
                    var prefab = AssetManager.GetInstance().LoadSprite(viewCode);
                    if (prefab)
                    {
                        GameObjectPoolManager.GetInstance().NewGameObjectPool(viewCode, prefab, maxCount);
                    }
                }
                prefabInstance = GameObjectPoolManager.GetInstance().GetGameObject(viewCode);
            }
            else
            {
                var prefab = AssetManager.GetInstance().LoadSprite(viewCode);
                if (prefab)
                {
                    prefabInstance = Object.Instantiate(prefab);
                }
            }


            GameObject viewGO;
            if (!string.IsNullOrEmpty(viewName))
            {
                viewGO = new GameObject(viewName);
                prefabInstance.transform.SetParent(viewGO.transform);
            }
            else
            {
                viewGO = prefabInstance;
            }

            return viewGO;
        }
    }
}
