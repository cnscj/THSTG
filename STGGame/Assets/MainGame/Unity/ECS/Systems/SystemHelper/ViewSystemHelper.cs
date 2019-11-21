using UnityEngine;

namespace STGU3D
{
    public static class ViewSystemHelper
    {
        public static readonly string viewName = "View";
        public static void TryCreateView(GameEntity entity)
        {
            if (string.IsNullOrEmpty(entity.view.viewCode))
                return;


            var prefab = AssetManager.GetInstance().LoadSprite(entity.view.viewCode);
            if (prefab)
            {
                var prefabInstance = GameObject.Instantiate(prefab);
                if (!string.IsNullOrEmpty(viewName))
                {
                    GameObject.Destroy(entity.view.viewGO);
                    entity.view.renderer = null;
                    entity.view.animator = null;

                    entity.view.viewGO = new GameObject(viewName);
                    prefabInstance.transform.SetParent(entity.view.viewGO.transform);

                }
                else
                {
                    entity.view.viewGO = prefabInstance;
                }
                entity.view.animator = prefabInstance.GetComponent<Animator>();
                entity.view.renderer = prefabInstance.GetComponent<Renderer>();
            }
        }
    }
}
