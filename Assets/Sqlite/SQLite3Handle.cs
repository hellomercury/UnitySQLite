using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using UnityEngine;
using UnityEngine.Assertions;
using SQLite3DbHandle = System.IntPtr;
using SQLite3Statement = System.IntPtr;
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

        public SQLite3DbHandle DatabaseHandle { get { return handle; } }
        private SQLite3DbHandle handle;

        private StringBuilder stringBuilder;

        public void OpenDB(string InDataBasePath, SQLite3OpenFlags InFlags)
        {
            Assert.raiseExceptions = true;
            Assert.IsFalse(string.IsNullOrEmpty(InDataBasePath), "数据库路径不能为空！");

            if (SQLite3Result.OK != SQLite3.Open(ConvertStringToUTF8Bytes(InDataBasePath),
                out handle, InFlags.GetHashCode(), IntPtr.Zero))
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
            Assert.IsFalse(SQLite3DbHandle.Zero == handle);

            Object[] obj = null;

            stringBuilder.Remove(0, stringBuilder.Length);
            stringBuilder.Append("SELECT * FROM ")
                .Append(InTableName)
                .Append(" WHERE ID = ")
                .Append(InKey);

            SQLite3Statement stmt;
            if (SQLite3Result.OK == SQLite3.Prepare2(handle, stringBuilder.ToString(), stringBuilder.Length, out stmt, IntPtr.Zero))
            {
                if (SQLite3Result.Row == SQLite3.Step(stmt))
                {
                    int count = SQLite3.ColumnCount(stmt);

                    obj = new object[count];

                    for (int i = 0; i < count; ++i)
                    {
                        SQLite3DataType type = SQLite3.ColumnType(stmt, i);

                        switch (type)
                        {
                            case SQLite3DataType.Integer:
                                obj[i] = SQLite3.ColumnInt(stmt, i);
                                break;
                            case SQLite3DataType.Real:
                                obj[i] = SQLite3.ColumnDouble(stmt, i);
                                break;
                            case SQLite3DataType.Text:
                                obj[i] = SQLite3.ColumnText(stmt, i);
                                break;
                            case SQLite3DataType.Blob:
                                obj[i] = SQLite3.ColumnBlob(stmt, i);
                                break;
                            case SQLite3DataType.Null:
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
            Assert.IsFalse(SQLite3DbHandle.Zero == handle);

            List<Object[]> obj = null;

            stringBuilder.Remove(0, stringBuilder.Length);
            stringBuilder.Append("SELECT ")
                .Append(InSelectColumnName)
                .Append(" FROM ")
                .Append(InTableName)
                .Append(" WHERE ")
                .Append(InCommon);

            SQLite3Statement stmt;
            if (SQLite3Result.OK == SQLite3.Prepare2(handle, stringBuilder.ToString(), stringBuilder.Length, out stmt, IntPtr.Zero))
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
                            SQLite3DataType type = SQLite3.ColumnType(stmt, i);

                            switch (type)
                            {
                                case SQLite3DataType.Integer:
                                    objs[i] = SQLite3.ColumnInt(stmt, i);
                                    break;
                                case SQLite3DataType.Real:
                                    objs[i] = SQLite3.ColumnDouble(stmt, i);
                                    break;
                                case SQLite3DataType.Text:
                                    objs[i] = SQLite3.ColumnText(stmt, i);
                                    break;
                                case SQLite3DataType.Blob:
                                    objs[i] = SQLite3.ColumnBlob(stmt, i);
                                    break;
                                case SQLite3DataType.Null:
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
            Assert.IsFalse(SQLite3DbHandle.Zero == handle);

            T t = new T();

            ClassProperty classProperty = t.ClassPropertyInfos;

            stringBuilder.Remove(0, stringBuilder.Length);
            stringBuilder.Append("SELECT * FROM ")
                .Append(classProperty.ClassName)
                .Append(" WHERE ID = ")
                .Append(InKey);

            SQLite3Statement stmt;
            if (SQLite3Result.OK == SQLite3.Prepare2(handle, stringBuilder.ToString(), stringBuilder.Length, out stmt, IntPtr.Zero))
            {
                if (SQLite3Result.Row == SQLite3.Step(stmt))
                {
                    int count = SQLite3.ColumnCount(stmt);
                    int length = classProperty.Infos.Length;

                    Assert.IsTrue(count == length, classProperty.ClassName + " : 数据库列与类属性个数不一致！");

                    Type type;
                    for (int i = 0; i < length; ++i)
                    {
                        type = classProperty.Infos[i].PropertyType;

                        if (typeof(int) == type)
                            classProperty.Infos[i].SetValue(t, SQLite3.ColumnInt(stmt, i), null);
                        else if (typeof(long) == type)
                            classProperty.Infos[i].SetValue(t, SQLite3.ColumnInt64(stmt, i), null);
                        else if (typeof(float) == type || typeof(double) == type)
                            classProperty.Infos[i].SetValue(t, SQLite3.ColumnDouble(stmt, i), null);
                        else if (typeof(string) == type)
                            classProperty.Infos[i].SetValue(t, SQLite3.ColumnText(stmt, i), null);
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
            Assert.IsFalse(SQLite3DbHandle.Zero == handle);

            T t = new T();

            ClassProperty classProperty = t.ClassPropertyInfos;

            SQLite3Statement stmt;
            SQLite3Result result;

            if (InIsNotFullCommand)
            {
                stringBuilder.Remove(0, stringBuilder.Length);
                stringBuilder.Append("SELECT * FROM ")
                    .Append(classProperty.ClassName)
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
                    PropertyInfo[] infos = t.ClassPropertyInfos.Infos;

                    int count = SQLite3.ColumnCount(stmt);
                    int length = infos.Length;

                    Assert.IsTrue(count == length, classProperty.ClassName + " : 数据库列与类属性个数不一致！");

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
            Assert.IsFalse(SQLite3DbHandle.Zero == handle);

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

            SQLite3Statement stmt;
            if (!(SQLite3Result.OK == SQLite3.Prepare2(handle, stringBuilder.ToString(), stringBuilder.Length, out stmt, IntPtr.Zero)
                && SQLite3Result.Done == SQLite3.Step(stmt)))
                Debug.LogError(stringBuilder + "/nError : " + SQLite3.GetErrmsg(stmt));

            SQLite3.Finalize(stmt);
        }

        public void CreateTable<T>() where T : Base
        {
            Assert.IsFalse(SQLite3DbHandle.Zero == handle);

            ClassProperty classProperty = Base.GetPropertyInfos(typeof (T));
            stringBuilder.Remove(0, stringBuilder.Length);
            stringBuilder.Append("CREATE TABLE ")
                .Append(classProperty.ClassName)
                .Append("(");
            int length = classProperty.Infos.Length;
            for (int i = 0; i < length; ++i)
            {
                stringBuilder.Append(classProperty.Infos[i].Name);

                Type type = classProperty.Infos[i].PropertyType;

                if (type == typeof (int) || type == typeof (long))
                    stringBuilder.Append(" INTEGER, ");
                else if (type == typeof (float) || type == typeof (double))
                    stringBuilder.Append(" REAL, ");
                else if (type == typeof (string))
                    stringBuilder.Append(" TEXT, ");
                else
                    stringBuilder.Append(" BLOB, ");
            }
            stringBuilder.Remove(stringBuilder.Length - 2, 2);
            stringBuilder.Append(")");

            SQLite3Statement stmt;
            if (!(SQLite3Result.OK == SQLite3.Prepare2(handle, stringBuilder.ToString(), stringBuilder.Length, out stmt, IntPtr.Zero)
                && SQLite3Result.Done == SQLite3.Step(stmt)))
                Debug.LogError(stringBuilder + "\nError : " + SQLite3.GetErrmsg(stmt));

            SQLite3.Finalize(stmt);
        }

        public void Insert(string InTableName, params object[] InValues)
        {
            Assert.IsFalse(SQLite3DbHandle.Zero == handle);

            stringBuilder.Remove(0, stringBuilder.Length);
            stringBuilder.Append("INSERT INTO ")
                .Append(InTableName)
                .Append(" VALUES(");

            int length = InValues.Length;
            for (int i = 0; i < length; ++i)
            {
                stringBuilder.Append("'").Append(InValues[i]).Append("', ");
            }
            stringBuilder.Remove(stringBuilder.Length - 2, 2);
            stringBuilder.Append(")");

            SQLite3Statement stmt;
            if (!(SQLite3Result.OK == SQLite3.Prepare2(handle, stringBuilder.ToString(), stringBuilder.Length, out stmt, IntPtr.Zero)
                && SQLite3Result.Done == SQLite3.Step(stmt)))
                Debug.LogError(stringBuilder + "\nError : " + SQLite3.GetErrmsg(stmt));

            SQLite3.Finalize(stmt);
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

