using System.Collections;
using System.Collections.Generic;
using Entitas.Unity;
using UnityEngine;
using XLibGame;

namespace STGU3D
{
    public static class ComponentUtil
    {
        public static void InitView(ViewComponent com)
        {

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
