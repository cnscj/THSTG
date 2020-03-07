using System.Collections.Generic;
using UnityEngine;

namespace ASGame
{
    public class AssetInstanceDebugger : AssetBaseDebugger
    {
        Dictionary<string, GameObject> m_resGameObj = new Dictionary<string, GameObject>();

        public override void Push(string key, string msg)
        {
            GameObject msgGObj = new GameObject();
            msgGObj.transform.SetParent(transform);
            msgGObj.name = msg;
        }

        public override void Remove(string key)
        {

        }
    }
}
