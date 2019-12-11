using System.Collections;
using System.Collections.Generic;
using STGU3D;
using UnityEngine;

namespace STGGame
{
    public class EntityConverter : MonoBehaviour
    {
        public string entityCode;

        public EEntityType entityType;
        public int type;
        public EHeroType heroType;
        public EBossType bossType;

        public Vector3 initSpeed;

        void Start()
        {
            if(!string.IsNullOrEmpty(entityCode))
            {
                var entity = EntityManager.GetInstance().CreateEntity(entityCode);
                if (entity != null)
                {

                }
            }
            GameObject.Destroy(gameObject);
        }  
    }
}
