﻿using FairyGUI;
using System;
using System.Collections.Generic;
using UnityEngine;
using XLibGame;
using EventCallback1 = XLibGame.EventCallback1;

namespace THGame.UI
{
    public class FWidget : FComponent
    {
        public string package { get; protected set; }
        public string component { get; protected set; }

        protected object _args;                 //参数
        protected bool _isAsync;                //是否异步加载
        protected float _interval = -1.0f;
        private int __scheduler = -1;
        private bool __isDisposed;
        private bool __isCreating;
        private List<Tuple<int, EventCallback1>> __listener;

        public bool isAsync { get { return _isAsync; } }

        //不受ViewManager管理
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
                    widget.__isCreating = true;
                    UIPackage.CreateObjectAsync(widget.package, widget.component, (obj) =>
                    {
                        if (widget.__isDisposed)
                        {
                            widget.__isCreating = false;
                            widget.__isDisposed = false;
                            obj.Dispose();
                            return;
                        }
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

        public static T Create<T>(FWidget parent = null, object args = null) where T : FWidget, new()
        {
            T view = Create(typeof(T), args) as T;
            if (parent != null)
            {
                parent.AddChild(view);
            }
            return view;
        }

        private static void OnCreateSuccess(GObject obj, FWidget widget, object args = null)
        {
            if (obj == null)
            {
                Debug.LogError(string.Format("{0} {1} => component not found | 没有加载到组件", widget.package, widget.component));
                return;
            }

            //
            widget.__isCreating = false;
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

        public void AddEventListener(int eventId, EventCallback1 listener)
        {
            Dispatcher.GetInstance().AddListener(eventId, listener);
            __listener = (__listener != null) ? __listener : new List<Tuple<int, EventCallback1>>();
            __listener.Add(new Tuple<int, EventCallback1>(eventId, listener));
        }

        public FWidget(string packageName, string componentName)
        {
            package = packageName;
            component = componentName;
        }

        //
        public override void Dispose()
        {
            if (_obj == null)
            {
                __isDisposed = true;
            }
            else
            {
                base.Dispose();
                ReleasePackage(this);
            }
        }

        public bool IsCreated()
        {
            return _obj != null;
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

        private void _OnRemovedFromStage()
        {
            _RemoveEvent();
            _RemoveSchedule();
            OnExit();
        }

        private void _InitScheduler()
        {
            if (_interval >= 0f)
            {
                __scheduler = Timer.GetInstance().Schedule(OnTick, _interval);
            }

        }

        private void _RemoveEvent()
        {
            if (__listener != null)
            {
                foreach (var pair in __listener)
                {
                    Dispatcher.GetInstance().RemoveListener(pair.Item1, pair.Item2);
                }
            }
        }

        private void _RemoveSchedule()
        {
            if (__scheduler != -1)
            {
                Timer.GetInstance().Unschedule(__scheduler);
                __scheduler = -1;
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
                obj.onRemovedFromStage.Add(_OnRemovedFromStage);
                
            }
            
            return this;
        }
    }

}
