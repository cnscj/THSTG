using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using SqlCipher4Unity3D;

namespace THGame
{
    public class SQLiteDatabase
    {
        public static bool IsExists(string dePath)
        {
            return System.IO.File.Exists(dePath);
        }

        public static SQLiteDatabase Create(string dbPath, string password)
        {
            var sqldatabase = new SQLiteDatabase();
            sqldatabase.Open(dbPath, password, SQLiteOpenFlags.ReadWrite | SQLiteOpenFlags.Create);
            return sqldatabase;
        }

        private SQLiteConnection m_connection;
        public SQLiteConnection Connection
        {
            get { return m_connection; }
        }

        /////////////////////////////////////////////
        public SQLiteDatabase()
        {

        }
        ~SQLiteDatabase()
        {
            Dispose();
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

        public List<T> Queue<T>(string query, params object[] args) where T : new()
        {
            if (m_connection != null)
            {
                return m_connection.Query<T>(query, args);
            }
            return default;
        }
        /////////////////////////////////////////////
        public int CreateTable<T>() where T : new()
        {
            if (m_connection != null)
            {
                return m_connection.CreateTable<T>();
            }
            return 0;
        }

        public int DropTable<T>() where T : new()
        {
            if (m_connection != null)
            {
                return m_connection.DropTable<T>();
            }
            return 0;
        }

        public void RollbackTo(string point)
        {
            if (m_connection != null)
            {
                m_connection.RollbackTo(point);
            }
        }

        public string SavePoint()
        {
            if (m_connection != null)
            {
                return m_connection.SaveTransactionPoint();
            }
            return null;
        }

        public void Commit()
        {
            if (m_connection != null)
            {
                m_connection.Commit();
            }
        }

        //增
        public int InsetItem(object item, bool isReplace = false)
        {
            if (m_connection != null)
            {
                if(isReplace)
                {
                    return m_connection.InsertOrReplace(item);
                }
                else
                {
                    return m_connection.Insert(item);
                }

            }
            return 0;
        }

        public int InsetItems(IEnumerable objs, bool isReplace = false)
        {
            if (m_connection != null)
            {
                if (objs != null)
                {
                    if (isReplace)
                    {
                        int total = 0;
                        foreach(var it in objs)
                        {
                            total += InsetItem(objs, isReplace);
                        }
                        return total;
                    }
                    else
                    {
                        return m_connection.InsertAll(objs);
                    }
                }
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
            int total = 0;
            if (m_connection != null && objs != null)
            {
                foreach(var it in objs)
                {
                    total += DeleteItem(it);
                }
            }
            return total;
        }

        public int DeleteItems<T>(Expression<Func<T, bool>> predExpr) where T : new()
        {
            return DeleteItems(SelectItems(predExpr));
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
        public IEnumerable<T> SelectItems<T>() where T : new()
        {
            if (m_connection != null)
            {
                return m_connection.Table<T>();
            }
            return default;
        }

        public IEnumerable<T> SelectItems<T>(Expression<Func<T, bool>> predExpr) where T : new()
        {
            if (m_connection != null)
            {
                return m_connection.Table<T>().Where(predExpr);
            }
            return default;
        }

        /////////////////////////////////////////////

        public void Open(string dbPath, string password, SQLiteOpenFlags openFlags)
        {
            Dispose();
            m_connection = new SQLiteConnection(dbPath, password, openFlags);
        }

        public void OpenOrCreate(string dbPath, string password)
        {
            Open(dbPath, password, SQLiteOpenFlags.ReadWrite | SQLiteOpenFlags.Create);
        }

        public void Close()
        {
            m_connection?.Close();
        }

        public void Dispose()
        {
            Close();
            m_connection?.Dispose();
        }
    }

}
