
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

        public Callback<GameObject, int> GetOrNewSprite(string viewCode, bool usePool = false, int maxCount = 20)
        {
            string viewName = null;
            GameObject prefabInstance = null;
            var callback = Callback<GameObject, int>.GetOrNew();

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
                    AssetManager.GetInstance().LoadSprite(viewCode).onSuccess.Set((prefab)=>
                    {
                        GameObjectPoolManager.GetInstance().NewGameObjectPool(viewCode, prefab, maxCount);
                        prefabInstance = GameObjectPoolManager.GetInstance().GetOrCreateGameObject(viewCode);
                        GameObject viewGO = new GameObject(viewName);
                        prefabInstance.transform.SetParent(viewGO.transform, false);
                        callback.onSuccess?.Invoke(prefabInstance);
                    });
                }
                else
                {
                    prefabInstance = GameObjectPoolManager.GetInstance().GetOrCreateGameObject(viewCode);
                    GameObject viewGO = new GameObject(viewName);
                    prefabInstance.transform.SetParent(viewGO.transform, false);
                    callback.onSuccess?.Invoke(prefabInstance);
                }

            }
            else
            {
                AssetManager.GetInstance().LoadSprite(viewCode).onSuccess.Set((prefab)=>
                {
                    if (prefab)
                    {
                        prefabInstance = Object.Instantiate(prefab);
                        callback.onSuccess?.Invoke(prefabInstance);
                    }
                });
            }

            return callback;
        }
    }
}
