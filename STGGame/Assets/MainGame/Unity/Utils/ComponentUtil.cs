using System.Collections;
using System.Collections.Generic;
using Entitas.Unity;
using STGGame;
using UnityEngine;
using XLibGame;

namespace STGU3D
{
    public static class ComponentUtil
    {
        public static IView CreateView(GameEntity entity)
        {
            var view = new UnityView();
            view.Create(entity);
            return view;
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
            if (com != null)
            {
                if (com.view != null)
                {
                    com.view.Clear();
                }
                com.view = null;
            }
        }
        public static void ClearHitbox(HitboxComponent com)
        {
            com.box = Vector2.zero;
        }

        public static void ClearDestroyed(DestroyedComponent com)
        {
            com.what = 0;
            com.isDestroyed = false;
        }
    }

}
