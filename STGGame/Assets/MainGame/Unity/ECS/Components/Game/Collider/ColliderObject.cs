using System;
using System.Collections.Generic;
using Entitas;
using UnityEngine;

namespace STGU3D
{
    public class ColliderObject
    {

        private List<ColliderShape> m_shapes;
        private ColliderContent m_result;

        public object data;
        public Vector3 center = Vector3.zero;
        public void AddShape(ColliderShape shape)
        {
            m_shapes = m_shapes ?? new List<ColliderShape>();
            m_shapes.Add(shape);
            shape.parent = this;
        }
        public void Update(Vector3 center)
        {
            this.center = center;
        }

        public ColliderContent Collide(ColliderObject other)
        {
            if(m_shapes != null && other.m_shapes != null)
            {
                m_result = m_result ?? new ColliderContent();
                m_result.collisions?.Clear();
                bool isCollided = false;
                foreach (var ownShapr in m_shapes)
                {
                    foreach (var otherShapr in other.m_shapes)
                    {
                        if (ownShapr.Check(otherShapr))
                        {
                            m_result.collisions = m_result.collisions ?? new Dictionary<ColliderShape, List<ColliderShape>>();
                            if (!m_result.collisions.ContainsKey(ownShapr)) m_result.collisions[ownShapr] = new List<ColliderShape>();
                            m_result.collisions[ownShapr].Add(otherShapr);


                            isCollided = true;
                        }
                        
                    }
                }

                if (isCollided)
                {
                    m_result.owner = this;
                    m_result.other = other;
                    return m_result;
                }
            }
            return null;
        }

        public bool IsReady()
        {
            return m_shapes != null;
        }
    }
}
