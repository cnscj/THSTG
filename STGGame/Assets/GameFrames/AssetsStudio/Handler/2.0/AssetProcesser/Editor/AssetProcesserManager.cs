using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using XLibrary.Package;

namespace ASEditor
{
    public class AssetProcesserManager : Singleton<AssetProcesserManager>
    {
        private List<AssetBaseProcesser> m_processerList = new List<AssetBaseProcesser>();

        public void Do(AssetBaseProcesser []processors)
        {
            Clear();

            foreach(var processor in processors)
            {
                m_processerList.Add(processor);
            }
            Proress();
            Purge();
        }

        public void Clear()
        {
            m_processerList.Clear();
        }

        private void Proress()
        {
            //通过反射获取所有类
            Assembly ass = Assembly.GetAssembly(typeof(AssetBaseProcesser));
            Type[] types = ass.GetTypes();
            List<Type> builderClassList = new List<Type>();
            foreach (Type item in types)
            {
                if (item.IsAbstract) continue;

                if (item == typeof(AssetBaseProcesser))
                {
                    builderClassList.Add(item);
                }
            }

            SortedList<int, AssetBaseProcesser> sortedClassList = new SortedList<int, AssetBaseProcesser>();
            foreach (Type item in builderClassList)
            {
                object obj = Activator.CreateInstance(item);//创建一个obj对象
                AssetBaseProcesser builder = obj as AssetBaseProcesser;
                sortedClassList.Add(1, builder);
            }

            for (int i = 0; i < m_processerList.Count; i++)
            {
                var oldIns = m_processerList[i];
                sortedClassList.Add(100000 + i, oldIns);
            }
            m_processerList = new List<AssetBaseProcesser>(sortedClassList.Values);

            Proress4Common();
            Proress4Custom();
        }

        private void Proress4Common()
        {

        }

        private void Proress4Custom()
        {
            foreach(var processer in m_processerList)
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
