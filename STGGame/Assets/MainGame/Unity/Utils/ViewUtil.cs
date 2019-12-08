using System.Collections;
using System.Collections.Generic;
using Entitas.Unity;
using UnityEngine;
using XLibGame;

namespace STGU3D
{
    public static class ViewUtil
    {
        public static GameObject NewNode(string name)
        {
            GameObject newGO = new GameObject(name);
            return newGO;
        }

        public static GameObject NewMainNode(GameEntity entity)
        {
            GameObject mainGO = NewNode("View");
            var entityLink = mainGO.AddComponent<EntityLink>();
            entityLink.Link(entity);

            return mainGO;
        }

        public static GameObject NewRendererNode(bool usePool, string viewCode)
        {
            string viewName = null;
            GameObject viewGO = null;
            GameObject prefabInstance = null;
            if (usePool)
            {
                if (!GameObjectPoolManager.GetInstance().HasGameObjectPool(viewCode))
                {
                    var prefab = AssetManager.GetInstance().LoadSprite(viewCode);
                    if (prefab)
                    {
                        GameObjectPoolManager.GetInstance().NewGameObjectPool(viewCode, prefab);
                    }
                }
                prefabInstance = GameObjectPoolManager.GetInstance().GetGameObject(viewCode);
            }
            else
            {
                var prefab = AssetManager.GetInstance().LoadSprite(viewCode);
                if (prefab)
                {
                    prefabInstance = GameObject.Instantiate(prefab);
                }
            }


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

