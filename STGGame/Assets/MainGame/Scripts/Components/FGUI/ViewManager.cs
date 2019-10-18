using System.Collections;
using XLibrary.Package;
using System.Collections.Generic;
using System;
using FairyGUI;

namespace STGGame
{
    public class ViewManager : MonoSingleton<ViewManager>
    {
        private Dictionary<Type, ViewInfo> m_viewMaps = new Dictionary<Type, ViewInfo>();
        public void Open<T>(Dictionary<string, Object> args = null) where T : FView, new()
        {
            ViewInfo viewInfo = null;
            if (!m_viewMaps.TryGetValue(typeof(T), out viewInfo))
            {
                FView view = new T();
                string packageName = view.package;
                string componentName = view.component;

                //检查包是否有加载
                if (PackageManager.GetInstance().GetPackageInfo(packageName) == null)
                {
                    PackageManager.GetInstance().AddPackage(packageName);
                }

                //加载包
                viewInfo = new ViewInfo();
                viewInfo.view = view;

                view.Create(args);
                
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
    }

}
