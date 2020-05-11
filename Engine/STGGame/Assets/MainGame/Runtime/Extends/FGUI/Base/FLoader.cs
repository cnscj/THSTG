using FairyGUI;

namespace STGRuntime.UI
{

    public class FLoader : FComponent
    {
        bool _isAsyncLoading = false;

        public bool isAsyncLoading
        {
            get { return _isAsyncLoading; }
            set { _isAsyncLoading = value; }
        }

        public void SetItemUrl(string package, string component)
        {
            string url = UIPackage.GetItemURL(package, component);
            _obj.asLoader.url = url;
        }

        //isSync: 是否同步加载。默认是异步，值为nil，只有同步时才会设置为true。
        public void SetUrl(string url, bool isSync = false)
        {
           isAsyncLoading = !isSync;  //这一行要在设置url之前，因为设置url时马上就加载资源了。
            _obj.asLoader.url = url;
        }

        public string GetUrl()
        {
            return _obj.asLoader.url;
        }

        public void SetTexture(NTexture texture)
        {
            _obj.asLoader.texture = texture;
        }

        public NTexture GetTexture()
        {
            return _obj.asLoader.texture;
        }

        public new void Center()
        {
            _obj.asLoader.Center();
        }

        public void SetPrecent(float value)
        {
            _obj.asLoader.fillAmount = value;
        }
    }

}
