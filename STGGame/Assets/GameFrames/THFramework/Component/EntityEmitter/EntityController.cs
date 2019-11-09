using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace THGame
{
    public class EntityController : MonoBehaviour
    {
        public Vector3 speed = new Vector3(0,0,0);

        void Start()
        {

        }

        void Update()
        {
            transform.localPosition += speed;
        }
    }
}

