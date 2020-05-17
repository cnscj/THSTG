using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using SQLite4Unity3d;

namespace THGame
{
    public class SQLiteDatabase
    {
        private SQLiteConnection m_connection;

        public SQLiteConnection Connection
        {
            get { return m_connection; }
        }

        public SQLiteDatabase(string databasePath)
        {

        }

        //基础SQL执行
        public int Execute(string query, params object[] args)
        {
            if (m_connection != null)
            {
                return m_connection.Execute(query, args);
            }
            return 0;
        }
        /////////////////////////////////////////////


        //基础操作:增删改查
        public void CreateTable<T>() where T : new()
        {
            if (m_connection != null)
            {
                m_connection.CreateTable<T>();
            }
        }
        public void DropTable<T>() where T : new()
        {
            if (m_connection != null)
            {
                m_connection.DropTable<T>();
            }
        }

        //增
        public int InsetItems(IEnumerable objs)
        {
            if (m_connection != null)
            {
                return m_connection.InsertAll(objs);
            }
            return 0;
        }
        public int InsetItem(object item)
        {
            if (m_connection != null)
            {
                return m_connection.Insert(item);
            }
            return 0;
        }

        //删
        public int DeleteItem(object item)
        {
            if (m_connection != null)
            {
                return m_connection.Delete(item);
            }
            return 0;
        }

        public int DeleteItems(IEnumerable objs)
        {
            if (m_connection != null)
            {

            }
            return 0;
        }

        public int DeleteAllItems<T>()
        {
            if (m_connection != null)
            {
                return m_connection.DeleteAll<T>();
            }
            return 0;
        }

        //改
        public int UpdateItem(object item)
        {
            if (m_connection != null)
            {
                return m_connection.Update(item);
            }
            return 0;
        }
        public int UpdateItems(IEnumerable objs)
        {
            if (m_connection != null)
            {
                return m_connection.UpdateAll(objs);
            }
            return 0;
        }

        //查
        public IEnumerable<T> QueryItems<T>() where T : new()
        {
            if (m_connection != null)
            {
                return m_connection.Table<T>();
            }
            return null;
        }
        public IEnumerable<T> QueryItems<T>(Expression<Func<T, bool>> predExpr) where T : new()
        {
            if (m_connection != null)
            {
                return m_connection.Table<T>().Where(predExpr);
            }
            return null;
        }

        /////////////////////////////////////////////

        public void Close()
        {
            m_connection.Close();
        }
    }

}
