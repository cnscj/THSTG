using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace STGRuntime.UI
{
    public class RedDotNode
    {
        public string name;
        public Action callback;
        public Dictionary<string, RedDotNode> nodes;
    }
}
