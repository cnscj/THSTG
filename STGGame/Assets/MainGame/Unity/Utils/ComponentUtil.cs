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

        public static void ClearEntityData(EntityDataComponent com)
        {
            com.entityCode = null;
            com.entityData = null;
            com.entityType = 0;
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
            if (com.view != null)
            {
                //复用bug,会把正在用的清掉
                //com.view.Clear();
            }
            com.isEditor = false;
            com.view = null;
            
        }
        public static void ClearHitbox(HitboxComponent com)
        {
            com.radius = 0f;
        }

        public static void ClearCollider(ColliderComponent com)
        {
            com.tag = 0;
            com.mask = 0;
            com.obj?.Clear();
        }

        public static void ClearDestroyed(DestroyedComponent com)
        {
            com.what = 0;
            com.isDestroyed = false;
        }
    }

}
