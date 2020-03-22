
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using XLibrary.Package;
using Object = UnityEngine.Object;
namespace ASGame
{
    public class AssetRefManager : MonoSingleton<AssetRefManager>
    {
        //延迟释放
        private Queue<IAssetRef> m_releaseQueue = new Queue<IAssetRef>();

        public void DelayRelease(IAssetRef refObj)
        {
            m_releaseQueue.Enqueue(refObj);
        }
        private void Update()
        {
            while(m_releaseQueue.Count > 0)
            {
                var refObj = m_releaseQueue.Dequeue();
                refObj.Release();
            }
        }
    }
}