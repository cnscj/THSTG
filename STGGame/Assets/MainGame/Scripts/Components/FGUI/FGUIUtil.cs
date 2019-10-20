using System.Collections;
using System.Collections.Generic;
using FairyGUI;
using UnityEngine;
using STGGame.UI;
namespace STGGame
{
    public static class FGUIUtil
    {
        public static FObject CreateObject(GObject obj)
        {
            return new FObject().InitWithObj(obj);
        }
        public static T CreateComponent<T>(GObject obj) where T : FComponent,new()
        {
            if (obj != null)
            {
                T com = new T();
                com.InitWithObj(obj);
                return com;
            }
            return null;
        }
    }
}
