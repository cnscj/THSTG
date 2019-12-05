using System;
using System.Collections.Generic;

namespace XLibrary.Collection
{
    public class Grid2D<T>
    {
        private static readonly List<T> s_empty = new List<T>();
        private List<List<T>> m_grids;
        private Dictionary<T, List<T>> m_enumerator;
        private int m_gridWidth;
        private int m_gridHeight;
        private int m_row;
        private int m_col;

        public Grid2D()
        {

        }

        public Grid2D(int row,int col,int width,int height)
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
                    m_grids = new List<List<T>>(m_row * m_col);
                    for (int i = 0; i < m_grids.Capacity; i++) m_grids.Add(null);

                    m_enumerator = new Dictionary<T, List<T>>();
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
                    nearbyList = new List<T>();
                    m_grids[index] = nearbyList;
                }

                //更新位置
                if (m_enumerator.ContainsKey(obj))
                {
                    var inList = m_enumerator[obj];
                    if (inList == nearbyList) return;
                        
                    inList.Remove(obj);
                    m_enumerator.Remove(obj);
                }
                nearbyList.Add(obj);
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

                inList.Remove(obj);
                m_enumerator.Remove(obj);
            }
            
        }
        public List<T> Gride(int row, int col)
        {
            if (m_grids == null) return s_empty;
            int index = GetGridIndex(row, col);
            if (index >= 0 && index < m_grids.Capacity)
            {
                var nearbyList = m_grids[index] != null ? m_grids[index] : s_empty;
                return nearbyList;
            }
            return s_empty;
        }

        public List<T> Local(int x, int y)
        {
            if (m_grids == null) return s_empty;
            int index = ConvertGridIndex(x, y);
            if (index >= 0 && index < m_grids.Capacity)
            {
                var nearbyList = m_grids[index] != null ? m_grids[index] : s_empty;
                return nearbyList;
            }

            return s_empty;
        }

        public void Clear()
        {
            if (m_grids == null) return;

            m_grids = new List<List<T>>(m_row * m_col);
            foreach(var list in m_grids)
            {
                if (list != null)
                {
                    list.Clear();
                }
            }

            m_enumerator = new Dictionary<T, List<T>>();
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
