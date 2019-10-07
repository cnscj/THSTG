
using System.Collections.Generic;
using UnityEngine;
using XLibrary.Package;

namespace STGGame
{
    //场景管理器
    public class ViewManager : MonoSingleton<ViewManager>
    {
       
        private Dictionary<string, GameObject> m_viewMap = new Dictionary<string, GameObject>();
        public void Awake()
        {
        }

        public void Open(string name)
        {
            if(m_viewMap.ContainsKey(name))
            {
                //重新打开
                //标志打开
            }
            else
            {
                //加载进缓冲
                //并打开
            }
        }

        public void Close(string name)
        {
            //送回缓冲
        }

        public bool IsOpen(string name)
        {
            return false;
        }
    }
}
