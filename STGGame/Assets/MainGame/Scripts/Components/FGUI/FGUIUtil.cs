using FairyGUI;
using STGGame.UI;
using System;
using UnityEngine;

namespace STGGame
{
    public static class FGUIUtil
    {
        public static FObject CreateObject(GObject obj)
        {
            var fObj = new FObject();
            if (fObj != null)
            {
                fObj.InitWithObj(obj);
            }
            return fObj;
        }

        public static T CreateComponent<T>(GObject obj) where T : FComponent,new()
        {
            if (obj != null)
            {
                T com = new T();
                if (com != null)
                {
                    com.InitWithObj(obj);
                }

                return com;
            }
            return null;
        }

        public static FComponent CreateComponent(GObject obj)
        {
            return CreateComponent<FComponent>(obj);
        }


        public static T CreateView<T>(object args = null) where T : FView, new()
        {
            return CreateView(typeof(T),args) as T;
        }

        public static FView CreateView(Type cls, object args = null)
        {
            System.Reflection.Assembly asm = System.Reflection.Assembly.GetExecutingAssembly();
            FView view = asm.CreateInstance(cls.FullName) as FView;
            if (view != null)
            {
                string packageName = view.package;
                string componentName = view.component;

                //加载包
                if (PackageManager.GetInstance().GetPackageInfo(packageName) == null)
                {
                    var packageInfo = PackageManager.GetInstance().AddPackage(packageName);

                    if (packageInfo == null)
                    {
                        Debug.LogError(string.Format("{0} => package not found | 没有加载到包", view.package));
                        return null;
                    }
                }

                if (view.isAsync)
                {
                    UIPackage.CreateObjectAsync(view.package, view.component, (obj) =>
                    {
                        OnCreateSuccess(obj, view);
                    });
                }
                else
                {
                    GObject obj = UIPackage.CreateObject(view.package, view.component);
                    OnCreateSuccess(obj, view);
                }

            }

            return view;
        }

        private static void OnCreateSuccess(GObject obj, FView view)
        {
            if (obj == null)
            {
                Debug.LogError(string.Format("{0} {1} => component not found | 没有加载到组件", view.package, view.component));
                return;
            }

            //
            view.InitWithObj(obj);
        }
    }
}
