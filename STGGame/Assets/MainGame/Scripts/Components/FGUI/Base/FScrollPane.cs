using FairyGUI;

namespace STGGame.UI
{

    public class FScrollPane
    {
        protected ScrollPane _obj;
        private FComponent __header;
        private FComponent __footer;

        public FScrollPane(ScrollPane scrollPane)
        {
            _obj = scrollPane;
        }

        public ScrollPane GetObject()
        {
            return _obj;
        }

        public FComponent GetHeader()
        {
            var obj = _obj.header;
            __header = (__header != null) ? (obj != null ? __header : null) : new FComponent().InitWithObj(obj) as FComponent;
            return __header;
        }

        public FComponent GetFooter()
        {
            var obj = _obj.footer;
            __footer = (__footer != null) ? (obj != null ? __footer : null) : new FComponent().InitWithObj(obj) as FComponent;
            return __footer;
        }

        public void LockHeader(int size)
        {
            _obj.LockHeader(size);
        }

        public void LockFooter(int size)
        {
            _obj.LockFooter(size);
        }


        public void ScrollTop()
        {
            _obj.ScrollTop();
        }

        public void ScrollBottom()
        {
            _obj.ScrollBottom();
        }

        public bool IsOnTop()
        {
            return _obj.percY <= 0;
        }

        public bool IsOnBottom()
        {
            return _obj.percY >= 1;
        }

        public void OnScroll(EventCallback1 func)
        {
            _obj.onScroll.Add(func);
        }

        public void OnScrollEnd(EventCallback1 func)
        {
            _obj.onScrollEnd.Add(func);
        }

        public void OnPullDownRelease(EventCallback1 func)
        {
            _obj.onPullDownRelease.Add(func);
        }

        public void OnPullUpRelease(EventCallback1 func)
        {
            _obj.onPullUpRelease.Add(func);
        }

        public float GetContentWidth()
        {
            return _obj.contentWidth;
        }

        public float GetContentHeight()
        {
            return _obj.contentHeight;
        }

        public float GetViewWidth()
        {
            return _obj.viewWidth;
        }

        public float GetViewHeight()
        {
            return _obj.viewHeight;
        }

    }

}
