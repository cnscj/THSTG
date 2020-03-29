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

            Proress();
            Purge();
        }

        public void Clear()
        {

        }

        private void Proress()
        {
            Proress4Common();
            Proress4Custom();
        }

        private void Proress4Common()
        {

        }

        private void Proress4Custom()
        {

        }

        private void Purge()
        {
            Purge4Invaild();
        }

        private void Purge4Invaild()
        {
            //遍历输出目录,把关联的目录,文件全部移除
        }
    }

}
