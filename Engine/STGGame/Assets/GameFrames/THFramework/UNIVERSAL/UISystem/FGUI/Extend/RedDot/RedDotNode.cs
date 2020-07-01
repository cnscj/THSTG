using System;
using System.Collections.Generic;

namespace THGame.UI
{
    public class RedDotNode
    {
        public string name;
        public RedDotNode parent;
        public Dictionary<string, RedDotNode> children;

        public int curStatus;
        public int willStatus;
        public int lightNum;
        public RedDotCallback callback;

        public void RemoveFromParent()
        {
            if (parent != null && parent.children != null)
            {
                callback = null;
                parent.children.Remove(name);
            }
        }
    }
}
