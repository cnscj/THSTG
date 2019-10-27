
using System;
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
        protected Action<FView> _onCreated;     //创建回调

        public bool isAsync { get { return _isAsync; } }

        //不受ViewManager管理
        public static FView Create(Type cls, object args = null)
        {
            System.Reflection.Assembly asm = System.Reflection.Assembly.GetExecutingAssembly();
            FView view = asm.CreateInstance(cls.FullName) as FView;
            if (view != null)
            {
                string packageName = view.package;
                string componentName = view.component;

                //加载包
                if (PackageManager.GetInstance().GetPackageInfo(packageName) == null)
                {
                    var packageInfo = PackageManager.GetInstance().AddPackage(packageName);

                    if (packageInfo == null)
                    {
                        Debug.LogError(string.Format("{0} => package not found | 没有加载到包", view.package));
                        return null;
                    }
                }

                if (view.isAsync)
                {
                    UIPackage.CreateObjectAsync(view.package, view.component, (obj) =>
                    {
                        OnCreateSuccess(obj, view);
                    });
                }
                else
                {
                    GObject obj = UIPackage.CreateObject(view.package, view.component);
                    OnCreateSuccess(obj, view);
                }

            }

            return view;
        }

        public static T Create<T>(object args = null) where T : FView, new()
        {
            return Create(typeof(T), args) as T;
        }

        private static void OnCreateSuccess(GObject obj, FView view)
        {
            if (obj == null)
            {
                Debug.LogError(string.Format("{0} {1} => component not found | 没有加载到组件", view.package, view.component));
                return;
            }

            //
            view.InitWithObj(obj);
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
