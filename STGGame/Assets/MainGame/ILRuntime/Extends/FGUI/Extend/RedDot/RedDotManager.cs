
using System;
using XLibrary.Package;

namespace STGService.UI
{
    public class RedDotManager : Singleton<RedDotManager>
    {
        public void Register(int id, Action func, object caller)
        {

        }

        public void UnRegister(int id, Action func, object caller)
        {

        }

        public void Post(int id)
        {

        }

        public void Update(RedDotParams redDotParams, bool isShow)
        {

        }

        public bool IsShow(RedDotParams redDotParams)
        {

            return false;
        }
    }
}
