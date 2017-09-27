using System;
using UnityEngine;
using UnityEngine.Assertions;
using Sqlite3DatabaseHandle = System.IntPtr;
using Sqlite3Statement = System.IntPtr;

namespace szn
{
    public class SQLiteHandle
    {
        public static SQLiteHandle Instance
        {
            get
            {
                if (null == instance) instance = new SQLiteHandle();
                return instance;
            }
        }
        private static SQLiteHandle instance;

        private Sqlite3DatabaseHandle handle;


        public void OpenDB(string InDataBasePath, SQLiteOpenFlags InFlags)
        {
            Assert.raiseExceptions = true;
            Assert.IsFalse(string.IsNullOrEmpty(InDataBasePath), "数据库路径不能为空！");

            SQLiteResult result = SQLite3.Open(ConvertStringToUTF8Bytes(InDataBasePath),
                out handle, InFlags.GetHashCode(), IntPtr.Zero);

            if (result != SQLiteResult.OK) Debug.LogError("数据库打开失败！");

        }

        public void ReadTable()
        {
            Assert.IsFalse(handle == IntPtr.Zero, "数据库未打开！                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                               ");
            
            string sql = "SELECT * FROM Item WHERE ID = '20000001'";
            Sqlite3Statement stmt;
            SQLiteResult result = SQLite3.Prepare2(handle, sql, sql.Length, out stmt, IntPtr.Zero);

            if (result == SQLiteResult.OK)
            {
                int count = SQLite3.ColumnCount(stmt);
                do
                {
                    SQLiteResult type = SQLite3.Step(stmt);
                    if (SQLiteResult.Row == type)
                    {
                        
                    }
                    else if (type == SQLiteResult.Done)
                    {
                        break;
                    }
                } while (true);
            }



            SQLite3.Finalize(stmt);
        }

        public Sqlite3Statement GetTable()
        {
            Assert.IsFalse(handle == IntPtr.Zero, "数据库未打开！                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                               ");
            
            string sql = "SELECT * FROM Person WHERE Id = 5";
            Sqlite3Statement stmt;
            SQLiteResult result = SQLite3.Prepare2(handle, sql, sql.Length, out stmt, IntPtr.Zero);

            if (result == SQLiteResult.OK)
            {
                if (SQLite3.Step(stmt) == SQLiteResult.Row)
                {
                    return stmt;
                }
            }
            
            SQLite3.Finalize(stmt);

            return IntPtr.Zero;
        }





        public void CloseDB()
        {
            SQLite3.Close(handle);
        }

        private byte[] ConvertStringToUTF8Bytes(string InContent)
        {
            int length = System.Text.Encoding.UTF8.GetByteCount(InContent);
            byte[] bytes = new byte[length + 1];
            System.Text.Encoding.UTF8.GetBytes(InContent, 0, InContent.Length, bytes, 0);

            return bytes;
        }
    }

}

