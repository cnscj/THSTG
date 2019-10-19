using UnityEngine;
using System.Collections;
using FairyGUI;
using System.Collections.Generic;
using XLibGame;

namespace STGGame.UI
{

    public class FComponent : FObject
    {
        public T GetChild<T>(string name) where T : FComponent, new()
        {
            GObject obj = this._obj.asCom.GetChild(name);
            return FGUIUtil.CreateComponent<T>(obj);
        }


        public void SetViewHeight(float height)
        {
            _obj.asCom.viewHeight = height;
        }
        public float GetViewHeight()
        {
            return _obj.asCom.viewHeight;
        }
    }

}
