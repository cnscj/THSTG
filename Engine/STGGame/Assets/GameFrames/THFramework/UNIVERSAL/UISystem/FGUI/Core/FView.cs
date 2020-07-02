
using System;
using FairyGUI;

namespace THGame.UI
{
	public class FView : FWidget
    {
        protected int _layerOrder = 0;          //层
        protected bool _isFullScreen;           //是否全屏

        protected Action<FView> _onCreated;     //创建回调

        public int layerOrder { get { return _layerOrder; } }

        public static new FView Create(Type cls, object args = null)
        {
            return FWidget.Create(cls, args) as FView;
        }

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
            //XXX:如果有退出动画,必须等动画结束在执行
            ViewInfo viewInfo = ViewManager.GetInstance().GetViewInfo(GetType());
            if (viewInfo != null && viewInfo.view == this)
            {
                ViewManager.GetInstance().Close(this, isDisposed);
            }
            else
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

            SetSortingOrder(_layerOrder);   //设置渲染层级,保证层级低的不覆盖

            return this;
        }

        //TODO:
        protected virtual void OnShowAnimation()
        {

        }

        protected virtual void OnHideAnimation()
        {

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
