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
            ViewInfo viewInfo = null;
            if (!m_viewMaps.TryGetValue(__GetViewKey<T>(), out viewInfo))
            {
                //加载View
                FGUIUtil.CreateView<T>(args).OnCreated((view)=>
                {
                    viewInfo = new ViewInfo();
                    viewInfo.view = view;

                    GRoot.inst.AddChild(view.GetObject());

                    m_viewMaps.Add(__GetViewKey<T>(), viewInfo);
                });  
            }

            //
        }

        public void Close(Type type, bool isDisposed = true)
        {
            ViewInfo viewInfo = null;
            if (m_viewMaps.TryGetValue(type, out viewInfo))
            {
                if (isDisposed)
                {
                    viewInfo.view.Dispose();
                    m_viewMaps.Remove(type);

                    //TODO:释放包的引用
                }
                else
                {
                    viewInfo.view.RemoveFromParent();
                }
            }
        }

        public void Close<T>(bool isDisposed = true) where T : FView
        {
            Close(typeof(T), isDisposed);
        }


        public bool IsOpened<T>() where T : FView
        {
            ViewInfo viewInfo = null;
            if (m_viewMaps.TryGetValue(__GetViewKey<T>(), out viewInfo))
            {

            }
            return false;
        }

        public ViewInfo GetView<T>() where T : FView
        {
            ViewInfo viewInfo = null;
            if (m_viewMaps.TryGetValue(__GetViewKey<T>(), out viewInfo))
            {
                return viewInfo;
            }
            return null;
        }

        ////
        private Type __GetViewKey<T>(FView view = null)
        {
            return typeof(T);
        }
    }

}
