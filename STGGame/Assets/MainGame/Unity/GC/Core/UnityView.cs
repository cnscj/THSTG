﻿using System.Collections;
using System.Collections.Generic;
using Entitas.Unity;
using UnityEngine;
using XLibGame;

namespace STGU3D
{
    public class UnityView : IView
    {
        public GameEntity entity;           //GE
        public GameObject node;             //与Unity关联的节点
        public BodyBehaviour bodyCom;       //身体节点

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
                if (bodyCom != null)
                {
                    bodyCom.Destroy();
                }
                //TODO:应该送入缓存区
                GameObject.Destroy(node);
            }
            node = null;
            entity = null;
        }

        //这里需要用协程延迟一帧
        public void Create(GameEntity ent)
        {
            Clear();

            entity = ent;

            //延迟一帧后,初始位置有问题
            SchedulerManager.GetInstance().ScheduleNextFrame(InitView);
        }

        public void SetRotation(float x, float y, float z)
        {
            if (node == null) return;
            var euler = node.transform.localEulerAngles;
            euler.x = x;
            euler.y = y;
            euler.z = z;
            node.transform.localEulerAngles = euler;
        }
        public void GetRotation(ref float x, ref float y, ref float z)
        {
            if (node == null) return;
            var euler = node.transform.localEulerAngles;
            x = euler.x;
            y = euler.y;
            z = euler.z;
        }

        public void SetPosition(float x, float y, float z)
        {
            if (node == null) return;
            var position = node.transform.localPosition;
            position.x = x;
            position.y = y;
            position.z = z;
            node.transform.localPosition = position;
        }

        public void GetPosition(ref float x, ref float y, ref float z)
        {
            if (node == null) return;
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

        bool LinkGOAndEntity(GameObject go, GameEntity ent)
        {
            if (go != null && ent != null)
            {
                var entityLink = go.GetComponent<EntityLink>();
                if (entityLink != null)
                {
                    entityLink.Unlink();
                }
                else
                {
                    entityLink = go.AddComponent<EntityLink>();
                }
                entityLink.Link(ent);
                return true;
            }
            return false;

        }

        void InitView()
        {
            if (node == null)
            {
                node = new GameObject("View");
            }
           
            if (LinkGOAndEntity(node, entity))
            {
                if (EntityManager.GetInstance())
                {
                    if (entity.hasEntityData)
                    {
                        MoveNode(entity);
                        AddBody(entity);
                        InitNode(entity);
                    }
                }
            }
        }

        void AddBody(GameEntity ent)
        {
            if (bodyCom != null)
            {
                GameObject.Destroy(bodyCom);
            }

            if (node != null)
            {
                string viewCode = ent.entityData.entityData["viewCode"];
                bodyCom = node.AddComponent<BodyBehaviour>();
                bodyCom.Create(viewCode);
            }
        }

        void MoveNode(GameEntity ent)
        {
            if (ent.view.isEditor)
                return;
  
            var entityType = ent.entityData.entityType;

            switch (entityType)
            {
                case EEntityType.Hero:
                    node.transform.SetParent(EntityManager.GetInstance().heroRoot.transform, false);
                    break;
                case EEntityType.Mob:
                    node.transform.SetParent(EntityManager.GetInstance().mobRoot.transform, false);
                    break;
                case EEntityType.Bullet:
                    node.transform.SetParent(EntityManager.GetInstance().bulletRoot.transform, false);
                    break;
                case EEntityType.Prop:
                    node.transform.SetParent(EntityManager.GetInstance().propRoot.transform, false);
                    break;
                case EEntityType.Wingman:
                    node.transform.SetParent(EntityManager.GetInstance().wingmanRoot.transform, false);
                    break;
            }
        }

        void InitNode(GameEntity ent)
        {
            if (ent.hasTransform)
            {
                if (ent.hasView)
                {
                    if (ent.view.isEditor)
                    {
                        ent.transform.localPosition = node.transform.localPosition;
                        ent.transform.localRotation = node.transform.localEulerAngles;
                    }
                    else
                    {
                        node.transform.localPosition = ent.transform.localPosition;
                        node.transform.localEulerAngles = ent.transform.localRotation;
                    }
                }
            }
        }
    }
}