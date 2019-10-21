using FairyGUI;
using STGGame.UI;
using System;

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
            T view = new T();
            if (view != null)
            {
                view.ToCreate();
            }
            
            return view;
        }

        public static FView CreateView(Type cls, object args = null)
        {
            System.Reflection.Assembly asm = System.Reflection.Assembly.GetExecutingAssembly();
            FView view = asm.CreateInstance(cls.FullName) as FView;
            if (view != null)
            {
                view.ToCreate();
            }
            
            return view;
        }
    }
}
