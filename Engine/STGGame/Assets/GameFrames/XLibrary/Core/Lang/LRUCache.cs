using System;
using System.Collections.Generic;
using System.Linq;

namespace XLibrary.Lang
{
    public class LRUCache<TKey,TValue>
    {
        internal sealed class LRUNode
        {
            public TKey Key { get; set; }
            public TValue Value { get; set; }
        }

        private Dictionary<TKey, LinkedListNode<LRUNode>> _dictionary = new Dictionary<TKey, LinkedListNode<LRUNode>>();
        private LinkedList<LRUNode> _linkedList = new LinkedList<LRUNode>();
        private readonly int _capacity;

        public LRUCache(int capacity = 100)
        {
            if (capacity < 1)
            {
                throw new ArgumentException(nameof(capacity));
            }

            _capacity = capacity;
        }

        public void Add(TKey key, TValue value)
        {
            if (_dictionary.ContainsKey(key))
            {
                LinkedListNode<LRUNode> node = _dictionary[key];
                _linkedList.Remove(node);//O(1)
                _linkedList.AddFirst(node);

                node.Value.Value = value;
            }
            else
            {
                LinkedListNode<LRUNode> newNode = null;

                if (_linkedList.Count >= _capacity)
                {
                    newNode = _linkedList.Last;//无需创建新对象
                    _linkedList.RemoveLast();
                    _dictionary.Remove(newNode.Value.Key);
                }
                else
                {
                    newNode = new LinkedListNode<LRUNode>(new LRUNode());
                }

                newNode.Value.Key = key;
                newNode.Value.Value = value;
                _linkedList.AddFirst(newNode);
                _dictionary.Add(key, newNode);
            }
        }

        public TValue Get(TKey key)
        {
            if (_dictionary.ContainsKey(key))
            {
                LinkedListNode<LRUNode> node = _dictionary[key];
                _linkedList.Remove(node);//O(1)
                _linkedList.AddFirst(node);

                return node.Value.Value;
            }

            return default(TValue);
        }

        public void Remove(TKey key)
        {
            if (_dictionary.ContainsKey(key))
            {
                LinkedListNode<LRUNode> node = _dictionary[key];
                _linkedList.Remove(node);//O(1)

                _dictionary.Remove(key);
            }
        }

        public bool ContainsKey(TKey key)
        {
            return _dictionary.ContainsKey(key);
        }

        public void Clear()
        {
            _linkedList.Clear();
            _dictionary.Clear();
        }
    }

 
}
