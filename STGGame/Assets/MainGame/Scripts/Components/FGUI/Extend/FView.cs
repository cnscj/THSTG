
using System;
using FairyGUI;

namespace STGGame.UI
{
	public class FView : FWidget
    {


        protected int _layerOrder = 0;          //层
        protected bool _isFullScreen;           //是否全屏
        protected Action<FView> _onCreated;     //创建回调


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

        public virtual void Close(bool isDisposed = true)
        {
            if (isDisposed)
            {
                Dispose();
            }
            else
            {
                RemoveFromParent();
            }
        }


        public override Wrapper<GObject> InitWithObj(GObject obj)
        {
            base.InitWithObj(obj);
            if (_isAsync)
            {
                DoCreated();
            }
            if (_isFullScreen)
            {
                SetSize(GetParent().GetWidth(), GetParent().GetHeight());
                AddRelation(GetParent(), RelationType.Size);
            }

            SetSortingOrder(_layerOrder);

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
