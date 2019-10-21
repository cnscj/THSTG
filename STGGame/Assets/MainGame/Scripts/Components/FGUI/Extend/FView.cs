using System.Collections.Generic;
using FairyGUI;
using UnityEngine;

namespace STGGame.UI
{
	public class FView : FWidget
    {
        protected object _args;                         //参数
        protected bool _isAsync = false;                //是否异步加载
        protected int _layerOrder = 0;                  //层
        protected bool _isFullScreen = false;           //是否全屏

        public bool isAsync { get { return _isAsync; } }


        public FView(string package, string component):base(package, component)
        {

        }

        public void ToCreate()
        {

        }

        public void ToAdd()
        {
            if (HasParent())
            {
                if (!GetParent().IsDisposed())
                {
                    GetParent().AddChild(this);
                }
            }
        }

        public virtual void Close()
        {

        }
    }

}
