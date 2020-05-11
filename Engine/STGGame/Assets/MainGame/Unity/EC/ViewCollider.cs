using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace STGU3D
{
    public class ViewCollider : ViewBaseClass
    {
        public List<Collider> colliders;
        public void Add(GameObject go)
        {
            if (go == null)
                return;

            colliders = colliders ?? new List<Collider>();
            var collidersInGO = go.GetComponentsInChildren<Collider>();
            colliders.AddRange(collidersInGO);

            BindColidersWithEntity(collidersInGO);
        }


        void BindColidersWithEntity(Collider []colliders)
        {
            //TODO:绑定BOX到Entity上
            if (unityView != null && unityView.entity != null)
            {
                if (colliders != null && colliders.Length > 0)
                {
                    var collider = colliders[0];
                    //Debug.Log(collider.bounds.center);
                    //Debug.Log(collider.bounds.size);
                }
            }
        }
    }
}