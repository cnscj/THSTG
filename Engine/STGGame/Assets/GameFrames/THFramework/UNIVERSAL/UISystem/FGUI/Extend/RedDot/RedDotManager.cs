
using System;
using System.Collections.Generic;
using XLibrary.Package;

namespace THGame.UI
{
    public class RedDotManager : Singleton<RedDotManager>
    {
        private RedDotNode m_treeRoot;      //红点树根节点

        public RedDotManager()
        {
            
        }

        public void Register(RedDotCallback callback, params string[] args)
        {
            if (args == null || args.Length <= 0)
                return;

            if (callback == null)
                return;

            var node = VisitTreeNode(true,args);
            node.callback += callback;

            node.curStatus = node.willStatus;
            SetStatus(node.curStatus, args);
        }

        public void Unregister(RedDotCallback callback, params string[] args)
        {
            if (args == null || args.Length <= 0)
                return;

            if (callback == null)
                return;

            var node = VisitTreeNode(false,args);
            if (node != null)
            {
                node.callback -= callback;
                if(node.children.Count <= 0 && node.callback == null)
                {
                    node.RemoveFromParent();
                }
            }     
        }

        //TODO:
        public void SetStatus(int status, params string[] args)
        {
            //只要子节点有一个亮,父节点就不能为Hide
            //如果子节点全为Hide,则父节点为Hide
            var node = VisitTreeNode(true, args);
            var oldStatus = node.curStatus;

            node.curStatus = status;
            node.willStatus = node.curStatus;
            node.callback?.Invoke(node.curStatus);

            if (oldStatus == status)
                return;

            if (status != RedDotStatus.Hide)
            {
                TraverseBottomTreeNode((child) =>
                {
                    child.lightNum++;
                    child.curStatus = status;
                    child.callback?.Invoke(child.curStatus);
                }, args);
            }
            else
            {
                //如果子节点还有一个亮,就是亮,否则为恢复原状态
                TraverseBottomTreeNode((child) =>
                {
                    child.lightNum--;
                    child.lightNum = Math.Max(0, child.lightNum);
                    if (child.lightNum <= 0)
                    {
                        child.curStatus = child.willStatus;
                        child.callback?.Invoke(child.curStatus);
                    }
                }, args);
            }

        }

        public int GetStatus(params string[] args)
        {
            var node = VisitTreeNode(false, args);
            if (node != null)
            {
                return node.curStatus;
            }

            return RedDotStatus.Hide;
        }

        ///
        private bool IsContainTreeNode(params string[] args)
        {
            var node = VisitTreeNode(false, args);
            return node != null;
        }

        private RedDotNode GetTreeRoot()
        {
            m_treeRoot = m_treeRoot ?? new RedDotNode();
            return m_treeRoot;
        }

        //创建红点树
        private RedDotNode CreateTree(params string[] args)
        {
            if (args == null || args.Length <= 0)
                return null;

            var node = VisitTreeNode(true, args);

            return node;
        }

        //移除红点树
        private bool DeleteTree(params string[] args)
        {
            if (m_treeRoot == null)
                return true;

            var node = VisitTreeNode(false, args);
            if (node == null)
                return false;

            var nodeParent = node.parent;
            nodeParent.children.Remove(node.name);

            return true;
        }

        private RedDotNode VisitTreeNode(bool isCreate, params string[] args)
        {
            if (args == null || args.Length <= 0)
                return null;

            var node = GetTreeRoot();
            foreach (var nodeName in args)
            {
                if (string.IsNullOrEmpty(nodeName))
                    return null;

                if (isCreate)
                {
                    node.children = node.children ?? new Dictionary<string, RedDotNode>();

                    if (!node.children.ContainsKey(nodeName))
                    {
                        //信息附加
                        var newNode = new RedDotNode();
                        newNode.name = nodeName;
                        newNode.parent = node;

                        node.children[nodeName] = newNode;
                    }
                    node.children.TryGetValue(nodeName, out node);
                }
                else
                {
                    if (node == null)
                        return null;

                    if (node.children == null)
                        return null;

                    node.children.TryGetValue(nodeName, out node);
                }
            }
            return node;
        }

        private void TraverseBottomTreeNode(Action<RedDotNode> action, params string[] args)
        {
            if (args == null || args.Length <= 0)
                return;

            var node = GetTreeRoot();
            foreach (var nodeName in args)
            {
                if (string.IsNullOrEmpty(nodeName))
                    return;

                if (node.children == null)
                    return;

                node.children.TryGetValue(nodeName, out node);

                if (node == null)
                    return;

                action?.Invoke(node);

            }
        }

        private void TraverseTopTreeNode(Action<RedDotNode> action, params string[] args)
        {
            var node = VisitTreeNode(false, args);
            while(node != null)
            {
                action?.Invoke(node);
                node = node.parent;
            }
        }
    }
}
