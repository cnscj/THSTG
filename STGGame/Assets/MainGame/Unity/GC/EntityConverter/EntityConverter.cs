using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace STGU3D
{
    public class EntityConverter : MonoBehaviour
    {
        public string entityCode;
        void Start()
        {
            if(!string.IsNullOrEmpty(entityCode))
            {
                EntityManager.GetInstance().CreateEntity(entityCode);
            }
            GameObject.Destroy(gameObject);
        }  
    }
}
