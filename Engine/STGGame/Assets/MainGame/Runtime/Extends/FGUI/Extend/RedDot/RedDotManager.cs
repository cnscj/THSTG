
using System;
using XLibrary.Package;

namespace STGRuntime.UI
{
    public class RedDotManager : Singleton<RedDotManager>
    {
        public void Register(int id, Action func, object caller)
        {

        }

        public void UnRegister(int id, Action func, object caller)
        {

        }

        public void SetStatus(bool isShow, params string[] args)
        {

        }

        public bool GetStatus(params string[] args)
        {
            return false;
        }
    }
}
