using System;
using UnityEngine;
using XLibrary.MVC;

namespace XSTGGame
{
    public static class Main
    {
        public static void Init()
        {
            Debug.Log("静态初始化函数");
            MVCManager.GetInstance().AddController<TestController>();
        }
    }
}
