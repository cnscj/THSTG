using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XLibGame;

namespace STGU3D
{
    public static class ComponentUtil
    {
        public static GameObject NewViewNode(bool usePool, string viewCode, Vector3 position, Vector3 rotation)
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

            //初始化
            viewGO.transform.localPosition = position;
            viewGO.transform.localEulerAngles = rotation;

            //EEntityType entityType = EntityUtil.GetEntityTypeByCode(viewCode);


            return viewGO;
        }

        public static void ClearTransform(TransformComponent com)
        {
            com.parent = null;
            com.position = Vector3.zero;
            com.rotation = Vector3.zero;
            com.localPosition = Vector3.zero;
            com.localRotation = Vector3.zero;
        }

        public static void ClearMovement(MovementComponent com)
        {
            com.moveSpeed = Vector3.zero;
            com.rotationSpeed = Vector3.zero;
        }


        public static void ClearView(ViewComponent com)
        {
            com.collider = null;
            com.animator = null;
            com.renderer = null;
            com.viewGO = null;
        }

        public static void ClearDestroyed(DestroyedComponent com)
        {
            com.what = 0;
            com.isDestroyed = false;
        }
    }

}
