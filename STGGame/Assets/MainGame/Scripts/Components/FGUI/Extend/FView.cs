using System.Collections.Generic;
using FairyGUI;
using UnityEngine;

namespace STGGame.UI
{
	public class FView : FWidget
    {
        protected object _args;                 //参数
        protected bool _isAsync;                //是否异步加载
        protected int _layerOrder = 0;          //层
        protected bool _isFullScreen;           //是否全屏

        public bool isAsync { get { return _isAsync; } }


        public FView(string package, string component):base(package, component)
        {

        }

        public void ForCreate()
        {

        }

        public void ForAdd()
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
            //TODO:
            //ViewManager.GetInstance().Close(this);
        }
    }

}
