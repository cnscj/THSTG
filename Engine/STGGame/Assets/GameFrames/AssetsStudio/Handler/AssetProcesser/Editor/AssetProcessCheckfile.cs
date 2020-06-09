using UnityEngine;

namespace ASEditor
{
    public class AssetProcessCheckfile : ScriptableObject
    {
        public string md5;

        public AssetProcessCheckfile() { }
        public AssetProcessCheckfile(AssetProcessCheckfile other)
        {
            this.md5 = other.md5;
        }
    }

}
