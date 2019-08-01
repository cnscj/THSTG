using System;
namespace THGame.Package.MVC
{
    public class Cache<T> : BaseCache where T : class,new()
    {
        private static T s_instance;

        public static T GetInstance()
        {
            if (s_instance == null)
            {
                s_instance = new T();
            }
            return s_instance;
        }
    }
}

