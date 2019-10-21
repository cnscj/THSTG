using System.Collections;
using XLibrary.Package;
using System.Collections.Generic;
using System;
using FairyGUI;
using STGGame.UI;
using UnityEngine;

namespace STGGame
{
    public class ViewManager : MonoSingleton<ViewManager>
    {
        private Dictionary<Type, ViewInfo> m_viewMaps = new Dictionary<Type, ViewInfo>();
        public void Open<T>(object args = null) where T : FView, new()
        {
            //TODO:
            ViewInfo viewInfo = null;
            if (!m_viewMaps.TryGetValue(typeof(T), out viewInfo))
            {
                T view = new T();
                string packageName = view.package;
                string componentName = view.component;

                //检查包是否有加载
                if (PackageManager.GetInstance().GetPackageInfo(packageName) == null)
                {
                    PackageManager.GetInstance().AddPackage(packageName);
                }
                //

                __CreateView<T>(view, args);
            }
            else
            {

            }

        }

        public void Close<T>() where T : FView
        {
            ViewInfo viewInfo = null;
            if (!m_viewMaps.TryGetValue(typeof(T), out viewInfo))
            {

            }
        }

        public bool IsOpened<T>() where T : FView
        {
            ViewInfo viewInfo = null;
            if (!m_viewMaps.TryGetValue(typeof(T), out viewInfo))
            {

            }
            return false;
        }

        public T GetView<T>() where T : FView
        {
            ViewInfo viewInfo = null;
            if (!m_viewMaps.TryGetValue(typeof(T), out viewInfo))
            {
                return viewInfo as T;
            }
            return null;
        }



        ////
        public void __CreateView<T>(T view, object args = null) where T : FView
        {
            if (view.isAsync)
            {
                UIPackage.CreateObjectAsync(view.package, view.component, (obj) =>
                {
                    _OnCreateSuccess<T>(obj, view);
                });
            }
            else
            {
                GObject obj = UIPackage.CreateObject(view.package, view.component);
                _OnCreateSuccess<T>(obj, view);
            }

        }

        private void _OnCreateSuccess<T>(GObject obj,T view) where T : FView
        {
            if (obj == null)
            {
                Debug.LogError(string.Format("{0} {1} => package not found | 没有加载到包或组件", view.package, view.component));
                return;
            }
            //加载包
            ViewInfo viewInfo = new ViewInfo();
            viewInfo.view = view;

            view.InitWithObj(obj);

            m_viewMaps.Add(typeof(T), viewInfo);
            GRoot.inst.AddChild(obj);
        }
    }

}
