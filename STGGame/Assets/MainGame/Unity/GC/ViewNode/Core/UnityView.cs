using System.Collections;
using System.Collections.Generic;
using Entitas.Unity;
using UnityEngine;

namespace STGU3D
{
    public class UnityView : IView
    {
        public GameEntity entity;           //GE
        public GameObject node;             //与Unity关联的节点
        public BodyBehaviour bodyCom;       //身体节点
        private void Awake()
        {

        }
        public void Clear()
        {
            if (node != null)
            {
                var entityLink = node.GetComponent<EntityLink>();
                if (entityLink != null)
                {
                    if (entityLink.entity != null)
                    {
                        entityLink.Unlink();
                    }
                }
                if(bodyCom != null)
                {
                    bodyCom.Destroy();
                }
                //TODO:应该送入缓存区
                GameObject.Destroy(node);
                node = null;
                entity = null;
            }
        }

        public void Create(GameEntity entity)
        {
            if (node == null)
            {
                node = new GameObject("View");
            }
            else
            {
                node.name = "View";
            }

            var entityLink = node.GetComponent<EntityLink>();
            if (entityLink != null)
            {
                entityLink.Unlink();
            }
            else
            {
                entityLink = node.AddComponent<EntityLink>();
            }
            entityLink.Link(entity);

            if (EntityManager.GetInstance())
            {
                if (entity.hasEntityData)
                {
                    switch (entity.entityData.entityType)
                    {
                        case EEntityType.Hero:
                            node.transform.SetParent(EntityManager.GetInstance().heroRoot.transform);
                            break;
                        case EEntityType.Mob:
                            node.transform.SetParent(EntityManager.GetInstance().mobRoot.transform);
                            break;
                        case EEntityType.Bullet:
                            node.transform.SetParent(EntityManager.GetInstance().bulletRoot.transform);
                            break;
                        case EEntityType.Prop:
                            node.transform.SetParent(EntityManager.GetInstance().propRoot.transform);
                            break;
                        case EEntityType.Wingman:
                            node.transform.SetParent(EntityManager.GetInstance().wingmanRoot.transform);
                            break;
                    }
                }
            }
            this.entity = entity;
        }

        public void SetRotation(in float x, in float y, in float z)
        {
            if (node == null) return;
            var euler = node.transform.localEulerAngles;
            euler.x = x;
            euler.y = y;
            euler.z = z;
            node.transform.localEulerAngles = euler;
        }
        public void GetRotation(out float x, out float y, out float z)
        {
            var euler = node.transform.localEulerAngles;
            x = euler.x;
            y = euler.y;
            z = euler.z;
        }

        public void SetPosition(in float x, in float y, in float z)
        {
            if (node == null) return;
            var position = node.transform.localPosition;
            position.x = x;
            position.y = y;
            position.z = z;
            node.transform.localPosition = position;
        }

        public void GetPosition(out float x, out float y, out float z)
        {
            var position = node.transform.localPosition;
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
            if (bodyCom != null)
            {
                GameObject.Destroy(bodyCom);
            }

            if (node != null)
            {
                bodyCom = node.AddComponent<BodyBehaviour>();
                bodyCom.Create(code);
            }
        }

        public object GetObject()
        {
            return node;
        }

        public void SetObject(object obj)
        {
            throw new System.NotImplementedException();
        }
    }
}
