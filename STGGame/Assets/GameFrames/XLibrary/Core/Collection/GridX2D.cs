using System;
using System.Collections.Generic;

namespace XLibrary.Collection
{
    public class GridX2D<T>
    {
        private static readonly List<T> s_empty = new List<T>();
        private List<Dictionary<T,int>> m_grids;
        private Dictionary<T, Dictionary<T, int>> m_enumerator;
        private Dictionary<int, bool> m_gridDirty;
        private Dictionary<int, List<T>> m_listCache;
        private int m_gridWidth;
        private int m_gridHeight;
        private int m_row;
        private int m_col;

        public GridX2D()
        {

        }

        public GridX2D(int row,int col,int width,int height)
        {
            Init(row, col, width, height);
        }

        public bool Init(int row, int col, int width, int height)
        {
            if (row > 0 && col > 0 && width > 0 && height > 0)
            {
                m_row = row;
                m_col = col;
                m_gridWidth = width / m_col;
                m_gridHeight = height / m_row;
                if (m_gridWidth > 0 && m_gridHeight > 0)
                {
                    m_grids = new List<Dictionary<T, int>>(m_row * m_col);
                    for (int i = 0; i < m_grids.Capacity; i++) m_grids.Add(null);

                    m_gridDirty = new Dictionary<int, bool>();
                    m_listCache = new Dictionary<int, List<T>>();
                    m_enumerator = new Dictionary<T, Dictionary<T, int>>();
                    return true;
                }

            }
            return false;
        }

        public void Update(T obj, int x, int y)
        {
            if (m_grids == null) return;

            int index = ConvertGridIndex(x, y);
            if (index >= 0 && index < m_grids.Capacity)
            {
                var nearbyList = m_grids[index];
                if (nearbyList == null)
                {
                    nearbyList = new Dictionary<T, int>();
                    m_grids[index] = nearbyList;
                }

                //更新位置
                if (m_enumerator.ContainsKey(obj))
                {
                    var inList = m_enumerator[obj];
                    if (inList == nearbyList) return;

                    m_gridDirty[inList[obj]] = true;
                    inList.Remove(obj);
                    m_enumerator.Remove(obj);
                }

                m_gridDirty[index] = true;
                nearbyList.Add(obj, index);
                m_enumerator.Add(obj, nearbyList);
                
            }
            else
            {
                if (m_enumerator.ContainsKey(obj))
                {
                    Remove(obj);
                }
            }
        }

        public void Remove(T obj)
        {
            if (m_grids == null) return;
            if (m_enumerator.ContainsKey(obj))
            {
                var inList = m_enumerator[obj];

                m_gridDirty.Remove(inList[obj]);
                inList.Remove(obj);
                m_enumerator.Remove(obj);
            }
            
        }

        public List<T> Get(int index)
        {
            if (m_grids == null) return s_empty;
            List<T> retList = s_empty;
            if (index >= 0 && index < m_grids.Capacity)
            {
                bool isNeedUpdate = true;
                if (m_listCache.TryGetValue(index, out retList))    //直接从Cache里取
                {
                    bool isDirty = false;
                    m_gridDirty.TryGetValue(index, out isDirty);
                    isNeedUpdate = isDirty;
                }
               
                if (isNeedUpdate)
                {
                    var gridMap = m_grids[index];
                    if (gridMap != null)
                    {
                        if (gridMap.Count > 0)
                        {
                            retList = new List<T>(gridMap.Keys);    //大量高速移动的话,还是会很频繁
                            m_listCache[index] = retList;
                        }
                    }
                }
            }

            m_gridDirty[index] = false;
            return retList;
        }

        public List<T> Gride(int row, int col)
        { 
            int index = GetGridIndex(row, col);
            return Get(index);
        }

        public List<T> Local(int x, int y)
        {
            int index = ConvertGridIndex(x, y);
            return Get(index);
        }

        public void Clear()
        {
            if (m_grids == null) return;

            foreach(var list in m_grids)
            {
                if (list != null)
                {
                    list.Clear();
                }
            }
            m_gridDirty.Clear();
            m_listCache.Clear();
            m_enumerator.Clear();
        }

        //
       
        public int GetGridIndex(int row, int col)
        {
            return Math.Max(0,row - 1) * m_col + col;
        }

        public int ConvertGridIndex(int x,int y)
        {
            int xIndex = x / m_gridWidth;
            int yIndex = y / m_gridHeight;

            return GetGridIndex(xIndex, yIndex);
        }


    }

}
