using XLibrary.Package;
using System.Collections.Generic;

namespace ASGame
{
    public class NodeLevelManager : Singleton<NodeLevelManager>
    {
        public int Level { get; private set; }
        private HashSet<NodeLevelNode> m_allNodes;

        public void AddNode(NodeLevelNode node)
        {
            if (node == null)
                return;

            m_allNodes = m_allNodes ?? new HashSet<NodeLevelNode>();
            if (!m_allNodes.Contains(node))
            {
                m_allNodes.Add(node);
            }
        }

        public void RemoveNode(NodeLevelNode node)
        {
            if (node == null)
                return;

            if (m_allNodes == null)
                return;

            if (m_allNodes.Contains(node))
            {
                m_allNodes.Remove(node);
            }
        }

        public List<NodeLevelNode> GetNodes()
        {
            if (m_allNodes == null)
                return null;

            if (m_allNodes.Count <= 0)
                return null;

            return new List<NodeLevelNode>(m_allNodes);
        }

        public void AdjustNodes(int lv)
        {
            if (m_allNodes == null || m_allNodes.Count <= 0)
                return;

            foreach (var node in m_allNodes)
            {
                if (node == null)
                    continue;

                node.Adjust(lv);
            }
            Level = lv;
        }
    }

}

