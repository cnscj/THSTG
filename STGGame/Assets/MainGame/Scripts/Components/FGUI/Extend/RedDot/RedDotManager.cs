
using System;
using XLibrary.Package;

namespace STGGame.UI
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

        public void SetStatus(RedDotParams redDotParams, bool isShow)
        {

        }

        public bool GetStatus(RedDotParams redDotParams)
        {

            return false;
        }
    }
}
