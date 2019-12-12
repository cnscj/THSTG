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
        public EWingmanType wingmanType;

        public Vector3 initSpeed;

        void Start()
        {
            //用其他转换
            if (string.IsNullOrEmpty(entityCode))
            {
                RefreshCode();
            }
            if(!string.IsNullOrEmpty(entityCode))
            {
                var entity = EntityManager.GetInstance().CreateEntity(entityCode);
                if (entity != null)
                {
                    if (entity.hasTransform)
                    {
                        entity.transform.localPosition = gameObject.transform.position;
                        entity.transform.localRotation = gameObject.transform.eulerAngles;

                    }
                    if (entity.hasMovement)
                    {
                        var entityController = GetComponent<THGame.EntityController>();
                        if (entityController != null)
                        {
                            initSpeed = entityController.speed;
                        }
                        entity.movement.moveSpeed = initSpeed;
                    }
                }
            }
            GameObject.Destroy(gameObject);
        }

        public void RefreshCode()
        {
            switch (entityType)
            {
                case EEntityType.Hero:
                    entityCode = EntityUtil.GetHeroCode(heroType);
                    break;
                case EEntityType.Boss:
                    entityCode = EntityUtil.GetBossCode(bossType);
                    break;
                case EEntityType.Wingman:
                    entityCode = EntityUtil.GetWingmanCode(wingmanType);
                    break;
                case EEntityType.Mob:
                    entityCode = EntityUtil.GetMobCode(type);
                    break;
                case EEntityType.Bullet:
                    entityCode = EntityUtil.GetBulletCode(type);
                    break;
                case EEntityType.Prop:
                    entityCode = EntityUtil.GetPropCode(type);
                    break;

            }
            
        }

        private void OnDrawGizmosSelected()
        {

        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;                       //为随后绘制的gizmos设置颜色。
            Gizmos.DrawWireSphere(transform.position, .20f);//使用center和radius参数，绘制一个线框球体
        }
    }
}
