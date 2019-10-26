
using System;
using FairyGUI;

namespace STGGame.UI
{
	public class FView : FWidget
    {
        protected object _args;                 //参数
        protected bool _isAsync;                //是否异步加载
        protected int _layerOrder = 0;          //层
        protected bool _isFullScreen;           //是否全屏
        protected Action<FView> _onCreated;     //创建回调

        public bool isAsync { get { return _isAsync; } }


        public FView(string package, string component):base(package, component)
        {

        }

        public FView OnCreated(Action<FView> onFunc)
        {
            _onCreated = onFunc;
            if(_obj != null)
            {
                if (!_isAsync)
                {
                    DoCreated();
                }
            }
            return this;
        }


        public virtual void Close()
        {
            ViewManager.GetInstance().Close(this.GetType());
        }

        public override Wrapper<GObject> InitWithObj(GObject obj)
        {
            base.InitWithObj(obj);
            if (_isAsync)
            {
                DoCreated();
            }
            return this;
        }

        private void DoCreated()
        {
            if (_onCreated != null)
            {
                _onCreated(this);
                _onCreated = null;
            }
        }
    }

}
