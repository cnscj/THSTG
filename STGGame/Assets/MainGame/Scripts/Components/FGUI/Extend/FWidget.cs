using FairyGUI;
using System;
using System.Collections.Generic;
using UnityEngine;
using XLibGame;

namespace STGGame.UI
{

    public class FWidget : FComponent
    {
        public string package { get; protected set; }
        public string component { get; protected set; }

        protected object _args;                 //参数
        protected bool _isAsync;                //是否异步加载
        protected float _interval = -1.0f;
        private int __scheduler = -1;
        private List<KeyValuePair<int, EventListener2>> __listener;

        public bool isAsync { get { return _isAsync; } }

        //不受widgetManager管理
        public static FWidget Create(Type cls, object args = null)
        {
            System.Reflection.Assembly asm = System.Reflection.Assembly.GetExecutingAssembly();
            FWidget widget = asm.CreateInstance(cls.FullName) as FWidget;
            if (widget != null)
            {
                string packageName = widget.package;
                string componentName = widget.component;

                //加载包
                if (PackageManager.GetInstance().GetPackageInfo(packageName) == null)
                {
                    var packageInfo = PackageManager.GetInstance().AddPackage(packageName);

                    if (packageInfo == null)
                    {
                        Debug.LogError(string.Format("{0} => package not found | 没有加载到包", widget.package));
                        return null;
                    }
                }

                if (widget.isAsync)
                {
                    UIPackage.CreateObjectAsync(widget.package, widget.component, (obj) =>
                    {
                        OnCreateSuccess(obj, widget, args);
                    });
                }
                else
                {
                    GObject obj = UIPackage.CreateObject(widget.package, widget.component);
                    OnCreateSuccess(obj, widget, args);
                }

            }

            return widget;
        }

        public static T Create<T>(object args = null) where T : FWidget, new()
        {
            return Create(typeof(T), args) as T;
        }

        private static void OnCreateSuccess(GObject obj, FWidget widget, object args = null)
        {
            if (obj == null)
            {
                Debug.LogError(string.Format("{0} {1} => component not found | 没有加载到组件", widget.package, widget.component));
                return;
            }

            //
            widget.SetArgs(args);
            widget.InitWithObj(obj);

            RetainPackage(widget);
        }

        private static void RetainPackage(FWidget widget)
        {
            if (widget != null )
            {
                PackageManager.GetInstance().RetainPackage(widget.package);
            }
        }

        private static void ReleasePackage(FWidget widget)
        {
            if (widget != null)
            {
                PackageManager.GetInstance().ReleasePackage(widget.package);
            }
        }

        public void AddEventListener(int eventId, EventListener2 listener)
        {
            DispatcherManager.GetInstance().AddListener(eventId, listener);
            __listener = (__listener != null) ? __listener : new List<KeyValuePair<int, EventListener2>>();
            __listener.Add(new KeyValuePair<int, EventListener2>(eventId, listener));
        }

        public FWidget(string packageName, string componentName)
        {
            package = packageName;
            component = componentName;
        }

        //
        public override void Dispose()
        {
            base.Dispose();
            ReleasePackage(this);
        }
        //
        public void SetArgs(object args)
        {
            _args = args;
        }

        public object GetArgs()
        {
            return _args;
        }

        //
        protected virtual void OnInitUI()
        {
          
        }

        protected virtual void OnInitEvent()
        {

        }

        protected virtual void OnEnter()
        {

        }

        protected virtual void OnExit()
        {

        }

        protected virtual void OnTick()
        {

        }

        //
        private void _OnAddedToStage()
        {
            OnInitEvent();
            _InitScheduler();

            OnEnter();
        }

        private void _OnEemovedFromStage()
        {
            _RemoveEvent();
            _RemoveSchedule();
            OnExit();
        }

        private void _InitScheduler()
        {
            if (_interval >= 0f)
            {
                __scheduler = SchedulerManager.GetInstance().Schedule(OnTick, _interval);
            }

        }

        private void _RemoveEvent()
        {
            if (__listener != null)
            {
                foreach (var pair in __listener)
                {
                    DispatcherManager.GetInstance().RemoveListener(pair.Key, pair.Value);
                }
            }
        }

        private void _RemoveSchedule()
        {
            if (__scheduler != -1)
            {
                SchedulerManager.GetInstance().Unschedule(__scheduler);
            }
        }

        ///
        public override Wrapper<GObject> InitWithObj(GObject obj)
        {
            base.InitWithObj(obj);
            if (obj != null)
            {
                OnInitUI();

                obj.onAddedToStage.Clear();
                obj.onRemovedFromStage.Clear();
                obj.onAddedToStage.Add(_OnAddedToStage);
                obj.onRemovedFromStage.Add(_OnEemovedFromStage);
                
            }
            
            return this;
        }
    }

}
