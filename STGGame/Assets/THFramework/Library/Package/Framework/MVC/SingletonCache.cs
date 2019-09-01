

using System;
namespace THGame.Package.MVC
{
    public class SingletonCache<T> : Cache where T : Cache, new()
    {
        private static T s_instance;

        public static T GetInstance()
        {
            if (s_instance == null)
            {
                s_instance = MVCManager.GetInstance().AddCache<T>();
            }
            return s_instance;
        }
    }
}

