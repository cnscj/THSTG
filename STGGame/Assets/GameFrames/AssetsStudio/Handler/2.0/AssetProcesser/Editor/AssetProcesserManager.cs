using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XLibrary.Package;

namespace ASEditor
{
    public class AssetProcesserManager : Singleton<AssetProcesserManager>
    {
        private LinkedList<AssetBaseProcesser> m_processerList = new LinkedList<AssetBaseProcesser>();

        public void Do()
        {
            Clear();
        }

        public void Clear()
        {

        }
    }

}
