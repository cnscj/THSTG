﻿
using System.IO;
using UnityEngine;
using XLibGame;
using XLibrary.Package;

namespace STGU3D
{
    public class SpriteManager : MonoSingleton<SpriteManager>
    {
        public GameObject GetOrNewSprite(string viewCode, bool usePool = false, int maxCount = 20)
        {
            string viewName = null;
            GameObject prefabInstance = null;
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
