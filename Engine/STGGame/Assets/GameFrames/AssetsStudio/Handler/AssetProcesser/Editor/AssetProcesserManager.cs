using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using XLibrary.Package;

namespace ASEditor
{
    public class AssetProcesserManager : Singleton<AssetProcesserManager>
    {
        public class ProcesserComparer : IComparer<int>
        {
            public int Compare(int x, int y)
            {
                int iResult = (int)x - (int)y;
                if (iResult == 0) iResult = -1;
                return iResult;
            }
        }
        private List<AssetCustomProcesser> m_customProcesserList = new List<AssetCustomProcesser>();

        public void Do(AssetCustomProcesser[] processors = null)
        {
            Clear();

            if (processors != null && processors.Length > 0)
            {
                foreach (var processor in processors)
                {
                    m_customProcesserList.Add(processor);
                }
            }

            Proress();
            Purge();

            AssetDatabase.Refresh();
        }

        public void Clear()
        {
            m_customProcesserList.Clear();
        }

        private void Proress()
        {
            //通过反射获取所有类
            Type[] types = Assembly.GetCallingAssembly().GetTypes();
            List<Type> processerClassList = new List<Type>();
            Type customProcesserType = typeof(AssetCustomProcesser);
            foreach (Type item in types)
            {
                if (item.IsAbstract) continue;
                if (item == customProcesserType) continue;

                if (item.BaseType == customProcesserType)
                {
                    processerClassList.Add(item);
                }
            }


            SortedList<int, AssetCustomProcesser> sortedClassList = new SortedList<int, AssetCustomProcesser>(new ProcesserComparer());
            foreach (Type item in processerClassList)
            {
                object obj = Activator.CreateInstance(item);//创建一个obj对象
                AssetCustomProcesser processer = obj as AssetCustomProcesser;
                processer.Init();

                var processerPriority = processer.GetPriority();
                if (!string.IsNullOrEmpty(processer.GetName()))
                {
                    sortedClassList.Add(processerPriority, processer);
                }
            }

            for (int i = 0; i < m_customProcesserList.Count; i++)
            {
                var oldIns = m_customProcesserList[i];
                sortedClassList.Add(100000 + i, oldIns);
            }
            m_customProcesserList = new List<AssetCustomProcesser>(sortedClassList.Values);

            Proress4Common();
            Proress4Custom();
        }

        private void Proress4Common()
        {

        }

        private void Proress4Custom()
        {
            foreach(var processer in m_customProcesserList)
            {
                processer.Deal();
            }
        }

        private void Purge()
        {
            Purge4Invaild();
        }

        private void Purge4Invaild()
        {
            //遍历输出目录,把关联的目录,文件全部移除
        }
    }

}
