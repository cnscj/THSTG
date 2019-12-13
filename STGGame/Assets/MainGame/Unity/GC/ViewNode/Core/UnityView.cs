using System.Collections;
using System.Collections.Generic;
using Entitas.Unity;
using UnityEngine;

namespace STGU3D
{
    public class UnityView : IView
    {
        public GameObject viewGO;           //与Unity关联的节点
        public BodyNode body;               //身体节点
        private void Awake()
        {

        }
        public void Clear()
        {
            if (viewGO != null)
            {
                var entityLink = viewGO.GetComponent<EntityLink>();
                if (entityLink != null)
                {
                    if (entityLink.entity != null)
                    {
                        entityLink.Unlink();
                    }
                }
                if(body != null)
                {
                    body.Destroy();
                }
                //TODO:应该送入缓存区
                GameObject.Destroy(viewGO);
                viewGO = null;
            }
        }

        public void Create(GameEntity entity)
        {
            Clear();
            viewGO = new GameObject("View");

            var entityLink = viewGO.AddComponent<EntityLink>();
            entityLink.Link(entity);

            if (EntityManager.GetInstance())
            {
                if (entity.hasEntityData)
                {
                    switch (entity.entityData.entityType)
                    {
                        case EEntityType.Hero:
                            viewGO.transform.SetParent(EntityManager.GetInstance().heroRoot.transform);
                            break;
                        case EEntityType.Mob:
                            viewGO.transform.SetParent(EntityManager.GetInstance().mobRoot.transform);
                            break;
                        case EEntityType.Bullet:
                            viewGO.transform.SetParent(EntityManager.GetInstance().bulletRoot.transform);
                            break;
                        case EEntityType.Prop:
                            viewGO.transform.SetParent(EntityManager.GetInstance().propRoot.transform);
                            break;
                        case EEntityType.Wingman:
                            viewGO.transform.SetParent(EntityManager.GetInstance().wingmanRoot.transform);
                            break;
                    }
                }
            }
            
        }

        public void SetRotation(in float x, in float y, in float z)
        {
            if (viewGO == null) return;
            var euler = viewGO.transform.localEulerAngles;
            euler.x = x;
            euler.y = y;
            euler.z = z;
            viewGO.transform.localEulerAngles = euler;
        }
        public void GetRotation(out float x, out float y, out float z)
        {
            var euler = viewGO.transform.localEulerAngles;
            x = euler.x;
            y = euler.y;
            z = euler.z;
        }

        public void SetPosition(in float x, in float y, in float z)
        {
            if (viewGO == null) return;
            var position = viewGO.transform.localPosition;
            position.x = x;
            position.y = y;
            position.z = z;
            viewGO.transform.localPosition = position;
        }

        public void GetPosition(out float x, out float y, out float z)
        {
            var position = viewGO.transform.localPosition;
            x = position.x;
            y = position.y;
            z = position.z;
        }

        ///

        public object Execute(int operate, object data = null)
        {

            return null;
        }

        public void AddBody(string code)
        {
            if (body != null)
            {
                GameObject.Destroy(body);
            }

            if (viewGO != null)
            {
                body = viewGO.AddComponent<BodyNode>();
                body.Create(code);
            }
        }

        public object GetObject()
        {
            return viewGO;
        }
    }
}
