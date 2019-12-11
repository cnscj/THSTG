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
                    entityLink.Unlink();
                }
                if(body != null)
                {
                    body.Destroy();
                }
                //TODO:应该送入缓存区
                GameObject.Destroy(viewGO);
            }
        }

        public void Create(GameEntity entity)
        {
            Clear();
            viewGO = new GameObject("View");

            var entityLink = viewGO.AddComponent<EntityLink>();
            entityLink.Link(entity);
        }

        public void SetRotation(in float x, in float y, in float z)
        {
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

        public object Command(int operate, object data = null)
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
