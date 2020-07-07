
using System;
using FairyGUI;

namespace THGame.UI
{
	public class FView : FWidget
    {
        protected int _layerOrder = 0;          //层
        protected bool _isFullScreen;           //是否全屏

        public int layerOrder { get { return _layerOrder; } }

        public FView(string package, string component) : base(package, component)
        {

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
    }

}
