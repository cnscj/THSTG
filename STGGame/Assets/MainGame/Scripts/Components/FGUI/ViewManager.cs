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
        //TODO:常驻View不应该被移除
        private Dictionary<Type, ViewInfo> m_viewMaps = new Dictionary<Type, ViewInfo>();
        private Action<ViewInfo> m_onCreated;

        //TODO:非模态窗口
        public ViewInfo New(Type type,object args =null)
        {
            return null;
        }

        //模态窗口
        public void Open(Type type, object args = null)
        {
            bool isNeedCreate = true;
            ViewInfo viewInfo = null;
            if (m_viewMaps.TryGetValue(type, out viewInfo))
            {
                FView view = viewInfo.view;
                if (!IsOpened(type))
                {
                    if (!view.IsDisposed())
                    {
                        view.SetArgs(args);
                        GRoot.inst.AddChild(view.GetObject());
                        isNeedCreate = false;
                    }
                }
                else
                {
                    isNeedCreate = false;
                }

            }

            //
            if (isNeedCreate)
            {
                //加载View
                FView.Create(type,args).OnCreated((view) =>
                {
                    viewInfo = new ViewInfo();
                    viewInfo.view = view;

                    GRoot.inst.AddChild(view.GetObject());

                    m_viewMaps[type] = viewInfo;

                    if (m_onCreated != null)
                    {
                        m_onCreated(viewInfo);
                    }
                });
            }
        }

        public void Open<T>(object args = null) where T : FView, new()
        {
            Open(__GetViewKey<T>(), args);
        }

        public void Open(string typeName, object args = null)
        {
            Type type = Type.GetType(typeName);
            Open(type, args);
        }


        public void Close(Type type, bool isDisposed = true)
        {
            ViewInfo viewInfo = null;
            if (m_viewMaps.TryGetValue(type, out viewInfo))
            {
                viewInfo.view.Close(isDisposed);
                if (isDisposed)
                {
                    m_viewMaps.Remove(type);
                }
            }
        }

        public void Close<T>(bool isDisposed = true) where T : FView
        {
            Close(typeof(T), isDisposed);
        }

        public void Close(string typeName, bool isDisposed = true)
        {
            Type type = Type.GetType(typeName);
            Close(type, isDisposed);
        }

        public bool IsOpened(Type type)
        {
            ViewInfo viewInfo = null;
            if (m_viewMaps.TryGetValue(type, out viewInfo))
            {
                FView view = viewInfo.view;
                if (!view.IsDisposed())
                {
                    return view.IsVisible();
                }
            }
            return false;
        }

        //TODO:
        public void CloseAll()
        {

        }



        public bool IsOpened<T>() where T : FView
        {
            return IsOpened(__GetViewKey<T>());
        }

        public bool IsOpened(string typeName)
        {
            Type type = Type.GetType(typeName);
            return IsOpened(type);
        }

        public ViewInfo GetViewInfo(Type type)
        {
            ViewInfo viewInfo = null;
            if (m_viewMaps.TryGetValue(type, out viewInfo))
            {
                return viewInfo;
            }
            return null;
        }

        public ViewInfo GetViewInfo<T>() where T : FView
        {
            return GetViewInfo(__GetViewKey<T>());
        }

        public ViewInfo GetViewInfo(string typeName)
        {
            Type type = Type.GetType(typeName);
            return GetViewInfo(type);
        }

        public void OnCreated(Action<ViewInfo> func)
        {
            m_onCreated = func;
        }

        ////
        private Type __GetViewKey<T>()
        {
            return typeof(T);
        }

        private Type __GetViewKey(FView view)
        {
            return view.GetType();
        }
    }

}
