using System;
using System.Collections.Generic;
using Entitas;
using UnityEngine;

namespace STGU3D
{
    public class ColliderObject
    {
        private ColliderContent m_result;
        private List<ColliderShape> m_shapes;

        public object data;
        public Vector3 center = Vector3.zero;

        /// <summary>
        /// a与b进行碰撞检测
        /// </summary>
        /// <param name="aObj"></param>
        /// <param name="aPos"></param>
        /// <param name="bObj"></param>
        /// <param name="bPos"></param>
        /// <returns></returns>
        public static ColliderContent Collide(ColliderObject aObj, Vector3 aPos, ColliderObject bObj, Vector3 bPos)
        {
            if (aObj == null || bObj == null)
                return null;

            if (aObj.m_shapes == null || bObj.m_shapes == null)
                return null;

            {
                aObj.center = aPos;
                bObj.center = bPos;

                ColliderContent result = new ColliderContent();
                result.collisions?.Clear();
                bool isCollided = false;
                foreach (var aShape in aObj.m_shapes)
                {
                    foreach (var bShape in bObj.m_shapes)
                    {
                        if (aShape.Check(bShape))
                        {
                            result.collisions = result.collisions ?? new Dictionary<ColliderObject, List<ColliderShape>>();
                            if (!result.collisions.ContainsKey(bObj)) result.collisions[bObj] = new List<ColliderShape>();
                            result.collisions[bObj].Add(bShape);

                            isCollided = true;
                        }
                    }
                }

                if (isCollided)
                {
                    result.owner = aObj;
                    return result;
                }
            }
            return null;
        }
        //碰撞检测开始
        public void BeginCollide(Vector3 aPos)
        {
            m_result = m_result ?? new ColliderContent();
            m_result.Clear();

            this.center = aPos;
            m_result.owner = this;
        }

        //碰撞检测
        public void Collide(ColliderObject bObj, Vector3 bPos)
        {
            ColliderObject aObj = this;

            if (aObj == null || bObj == null)
                return;

            if (aObj.m_shapes == null || bObj.m_shapes == null)
                return;

            {
                bObj.center = bPos;

                foreach (var aShape in aObj.m_shapes)
                {
                    foreach (var bShape in bObj.m_shapes)
                    {
                        if (aShape.Check(bShape))
                        {
                            m_result.collisions = m_result.collisions ?? new Dictionary<ColliderObject, List<ColliderShape>>();
                            if (!m_result.collisions.ContainsKey(bObj)) m_result.collisions[bObj] = new List<ColliderShape>();
                            m_result.collisions[bObj].Add(bShape);

                        }
                    }
                }

            }
        }

        //返回结果
        public ColliderContent GetContent()
        {
            if (m_result != null && m_result.collisions != null)
            {
                if (m_result.collisions.Count > 0)
                    return m_result;
            }
            return null;
        }

        //添加形状
        public ColliderObject AddShape(ColliderShape shape)
        {
            m_shapes = m_shapes ?? new List<ColliderShape>();
            m_shapes.Add(shape);
            shape.parent = this;

            return this;
        }

        public void Clear()
        {
            m_result?.Clear();
            m_shapes?.Clear();
        }

    }
}
