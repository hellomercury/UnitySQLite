﻿using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using UnityEngine;
using UnityEngine.Assertions;
using Sqlite3DatabaseHandle = System.IntPtr;
using Sqlite3Statement = System.IntPtr;
using Object = System.Object;

namespace szn
{
    public class SQLite3Handle
    {
        public static SQLite3Handle Instance
        {
            get
            {
                if (null == instance) instance = new SQLite3Handle();
                return instance;
            }
        }
        private static SQLite3Handle instance;

        private Sqlite3DatabaseHandle handle;
        private StringBuilder stringBuilder;

        public void OpenDB(string InDataBasePath, SQLiteOpenFlags InFlags)
        {
            Assert.raiseExceptions = true;
            Assert.IsFalse(string.IsNullOrEmpty(InDataBasePath), "数据库路径不能为空！");

            SQLite3Result result = SQLite3.Open(ConvertStringToUTF8Bytes(InDataBasePath),
                out handle, InFlags.GetHashCode(), IntPtr.Zero);

            if (result != SQLite3Result.OK)
            {
                SQLite3.Close(handle);
                handle = IntPtr.Zero;
                Debug.LogError("数据库打开失败！");
            }
            else
            {
                stringBuilder = new StringBuilder(1024);
            }
        }

        public Object[] SelectSingleData(string InTableName, int InKey)
        {
            Assert.IsFalse(handle == IntPtr.Zero);

            Object[] obj = null;

            stringBuilder.Remove(0, stringBuilder.Length);
            stringBuilder.Append("SELECT * FROM ")
                .Append(InTableName)
                .Append(" WHERE ID = ")
                .Append(InKey);

            Sqlite3Statement stmt;
            SQLite3Result result = SQLite3.Prepare2(handle, stringBuilder.ToString(), stringBuilder.Length, out stmt, IntPtr.Zero);

            if (SQLite3Result.OK == result)
            {
                if (SQLite3Result.Row == SQLite3.Step(stmt))
                {
                    int count = SQLite3.ColumnCount(stmt);

                    obj = new object[count];

                    for (int i = 0; i < count; ++i)
                    {
                        ColType type = SQLite3.ColumnType(stmt, i);

                        switch (type)
                        {
                            case ColType.Integer:
                                obj[i] = SQLite3.ColumnInt(stmt, i);
                                break;
                            case ColType.Float:
                                obj[i] = SQLite3.ColumnDouble(stmt, i);
                                break;
                            case ColType.Text:
                                obj[i] = SQLite3.ColumnText(stmt, i);
                                break;
                            case ColType.Blob:
                                obj[i] = SQLite3.ColumnBlob(stmt, i);
                                break;
                            case ColType.Null:
                                obj[i] = null;
                                break;
                        }
                    }
                }
            }
            else
            {
                stringBuilder.Append("\nError : ")
                    .Append(SQLite3.GetErrmsg(handle));

                Debug.LogError(stringBuilder.ToString());
            }

            SQLite3.Finalize(stmt);

            return obj;
        }

        public List<Object[]> SelectMultiData(string InTableName, string InSelectColumnName, string InCommon)
        {
            Assert.IsFalse(handle == IntPtr.Zero);

            List<Object[]> obj = null;

            stringBuilder.Remove(0, stringBuilder.Length);
            stringBuilder.Append("SELECT ")
                .Append(InSelectColumnName)
                .Append(" FROM ")
                .Append(InTableName)
                .Append(" WHERE ")
                .Append(InCommon);

            Sqlite3Statement stmt;
            SQLite3Result result = SQLite3.Prepare2(handle, stringBuilder.ToString(), stringBuilder.Length, out stmt, IntPtr.Zero);

            if (SQLite3Result.OK == result)
            {
                obj = new List<object[]>();
                int count = SQLite3.ColumnCount(stmt);
                do
                {
                    SQLite3Result rowResult = SQLite3.Step(stmt);
                    if (SQLite3Result.Row == rowResult)
                    {
                        Object[] objs = new object[count];

                        for (int i = 0; i < count; ++i)
                        {
                            ColType type = SQLite3.ColumnType(stmt, i);

                            switch (type)
                            {
                                case ColType.Integer:
                                    objs[i] = SQLite3.ColumnInt(stmt, i);
                                    break;
                                case ColType.Float:
                                    objs[i] = SQLite3.ColumnDouble(stmt, i);
                                    break;
                                case ColType.Text:
                                    objs[i] = SQLite3.ColumnText(stmt, i);
                                    break;
                                case ColType.Blob:
                                    objs[i] = SQLite3.ColumnBlob(stmt, i);
                                    break;
                                case ColType.Null:
                                    objs[i] = null;
                                    break;
                            }

                            obj.Add(objs);
                        }
                    }
                    else if (SQLite3Result.Done == rowResult) break;

                } while (true);
            }
            else
            {
                stringBuilder.Append("\nError : ")
                    .Append(SQLite3.GetErrmsg(handle));

                Debug.LogError(stringBuilder.ToString());
            }

            SQLite3.Finalize(stmt);

            return obj;
        }

        public T SelectSingleT<T>(int InKey) where T : Base, new()
        {
            Assert.IsFalse(handle == IntPtr.Zero);

            T t = new T();

            PropertyStruct property = t.PropertyInfos;

            stringBuilder.Remove(0, stringBuilder.Length);
            stringBuilder.Append("SELECT * FROM ")
                .Append(property.ClassName)
                .Append(" WHERE ID = ")
                .Append(InKey);

            Sqlite3Statement stmt;
            SQLite3Result result = SQLite3.Prepare2(handle, stringBuilder.ToString(), stringBuilder.Length, out stmt, IntPtr.Zero);

            if (SQLite3Result.OK == result)
            {
                if (SQLite3Result.Row == SQLite3.Step(stmt))
                {
                    int count = SQLite3.ColumnCount(stmt);
                    int length = property.Infos.Length;

                    Assert.IsTrue(count == length, property.ClassName + " : 数据库列与类属性个数不一致！");

                    Type type;
                    for (int i = 0; i < length; ++i)
                    {
                        type = property.Infos[i].PropertyType;

                        if (typeof(int) == type)
                            property.Infos[i].SetValue(t, SQLite3.ColumnInt(stmt, i), null);
                        else if (typeof(long) == type)
                            property.Infos[i].SetValue(t, SQLite3.ColumnInt64(stmt, i), null);
                        else if (typeof(float) == type || typeof(double) == type)
                            property.Infos[i].SetValue(t, SQLite3.ColumnDouble(stmt, i), null);
                        else if (typeof(string) == type)
                            property.Infos[i].SetValue(t, SQLite3.ColumnText(stmt, i), null);
                    }
                }
            }
            else
            {
                stringBuilder.Append("\nError : ")
                    .Append(SQLite3.GetErrmsg(handle));

                Debug.LogError(stringBuilder.ToString());

                return null;
            }

            SQLite3.Finalize(stmt);

            return t;

            //return SelectSingleT<T>("ID = " + InKey);
        }

        public T SelectSingleT<T>(string InCommand, bool InIsNotFullCommand = true) where T : Base, new()
        {
            Assert.IsFalse(handle == IntPtr.Zero);

            T t = new T();

            PropertyStruct property = t.PropertyInfos;

            Sqlite3Statement stmt;
            SQLite3Result result;

            if (InIsNotFullCommand)
            {
                stringBuilder.Remove(0, stringBuilder.Length);
                stringBuilder.Append("SELECT * FROM ")
                    .Append(property.ClassName)
                    .Append(" WHERE ")
                    .Append(InCommand);

                result = SQLite3.Prepare2(handle, InCommand, InCommand.Length, out stmt, IntPtr.Zero);
            }
            else
            {
                result = SQLite3.Prepare2(handle, stringBuilder.ToString(), stringBuilder.Length, out stmt, IntPtr.Zero);
            }
            
            if (SQLite3Result.OK == result)
            {
                if (SQLite3Result.Row == SQLite3.Step(stmt))
                {
                    PropertyInfo[] infos = t.PropertyInfos.Infos;

                    int count = SQLite3.ColumnCount(stmt);
                    int length = infos.Length;

                    Assert.IsTrue(count == length, property.ClassName + " : 数据库列与类属性个数不一致！");

                    Type type;
                    for (int i = 0; i < length; ++i)
                    {
                        type = infos[i].PropertyType;

                        if (typeof(int) == type)
                            infos[i].SetValue(t, SQLite3.ColumnInt(stmt, i), null);
                        else if (typeof(long) == type)
                            infos[i].SetValue(t, SQLite3.ColumnInt64(stmt, i), null);
                        else if (typeof(float) == type || typeof(double) == type)
                            infos[i].SetValue(t, SQLite3.ColumnDouble(stmt, i), null);
                        else if (typeof(string) == type)
                            infos[i].SetValue(t, SQLite3.ColumnText(stmt, i), null);
                    }
                }
            }
            else
            {
                stringBuilder.Append("\nError : ")
                    .Append(SQLite3.GetErrmsg(handle));

                Debug.LogError(stringBuilder.ToString());

                return null;
            }

            SQLite3.Finalize(stmt);

            return t;
        }

        public void CreateTable(string InTableName, params string[] InColumnNameAndType)
        {
            Assert.IsFalse(handle == IntPtr.Zero);

            stringBuilder.Remove(0, stringBuilder.Length);
            stringBuilder.Append("CREATE TABLE ")
                .Append(InTableName)
                .Append(" (");
            int length = InColumnNameAndType.Length;
            for (int i = 0; i < length; i++)
            {
                stringBuilder.Append(InColumnNameAndType[i]).Append(", ");
            }
            stringBuilder.Remove(stringBuilder.Length - 2, 2);
            stringBuilder.Append(")");

            Sqlite3Statement stmt;
            SQLite3Result result = SQLite3.Prepare2(handle, stringBuilder.ToString(), stringBuilder.Length, out stmt,
                IntPtr.Zero);

            if (!(SQLite3Result.OK == result && SQLite3Result.Done == SQLite3.Step(stmt)))
                Debug.LogError(stringBuilder + "/nError : " + SQLite3.GetErrmsg(stmt));

            SQLite3.Finalize(stmt);
        }

        public void CreateTable<T>(T InT) where T : Base
        {

        }


        public void CloseDB()
        {
            SQLite3.Close(handle);
        }

        private byte[] ConvertStringToUTF8Bytes(string InContent)
        {
            int length = Encoding.UTF8.GetByteCount(InContent);
            byte[] bytes = new byte[length + 1];
            Encoding.UTF8.GetBytes(InContent, 0, InContent.Length, bytes, 0);

            return bytes;
        }
    }
}
