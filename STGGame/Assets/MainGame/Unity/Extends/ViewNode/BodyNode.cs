using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace STGU3D
{
    public class BodyNode : MonoBehaviour
    {
        public GameObject body;
        public Animator animator;

        void Start()
        {
            body = new GameObject("Body");
            body.transform.SetParent(gameObject.transform);
        }

       
    }

}
