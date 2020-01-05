using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace STGU3D
{
    public class ViewCollider : MonoBehaviour
    {
        public List<Collider> colliders;
        public void Add(GameObject go)
        {
            if (go == null)
                return;

            colliders = colliders ?? new List<Collider>();
            colliders.AddRange(go.GetComponentsInChildren<Collider>());
        }
    }
}