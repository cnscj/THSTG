using System;
using System.Collections.Generic;

namespace XLibrary.Package
{
    public class Subject
    {
        private List<IObserver> m_ObsList = new List<IObserver>();
        private bool m_changed;


        //添加,删除观察者
        public void AddObserver(IObserver observer)
        {
            if (observer != null)
            {
                m_ObsList.Add(observer);
            }
                
        }
        public void RemoveObserver(IObserver observer)
        {
            if (observer != null)
            {
                m_ObsList.Remove(observer);
            }
        }

        //主题数据是否发生改变
        public bool HasChanged()
        {
            return m_changed;
        }

        //通知观察者更新数据
        public void NotifyObserver(Object data)
        {
            foreach(var obs in m_ObsList)
            {
                obs.Notified(this, data);
            }
        }
        public void NotifyObservers()
        {
            foreach (var obs in m_ObsList)
            {
                obs.Notified(this, null);
            }
        }

        //设置主题变化标志
        protected void SetChanged(bool bChange = true)
        {
            m_changed = bChange;
        }

        //清除主题变化变化标志
        protected void ClearChanged()
        {
            m_changed = false;
        }
    }
}
