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

            //绑定GameObject
            if (entity.view.viewGO != null)
            {
                GameObject.Destroy(entity.view.viewGO);
                entity.view.renderer = null;
                entity.view.animator = null;
            }

            GameObject view = new GameObject(viewName);
            entity.view.viewGO = view;

            //加载模型
            var prefab = AssetManager.GetInstance().LoadSprite(entity.view.viewCode);
            if (prefab)
            {
                var viewNode = GameObject.Instantiate(prefab, view.transform, false);
                entity.view.animator = viewNode.GetComponent<Animator>();
                entity.view.renderer = viewNode.GetComponent<Renderer>();
            }
        }
    }
}