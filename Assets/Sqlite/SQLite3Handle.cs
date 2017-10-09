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

            string transaction = "BEGIN TRANSACTION";
            SQLite3Statement beginStmt;
            if (SQLite3Result.OK !=
                SQLite3.Prepare2(handle, transaction, transaction.Length, out beginStmt, IntPtr.Zero))
            {
                Debug.LogError(transaction + " \nError : " + SQLite3.GetErrmsg(beginStmt));
                SQLite3.Finalize(beginStmt);
            }

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
                    obj = GetObjects(stmt, SQLite3.ColumnCount(stmt));
                }
            }
            else
            {
                stringBuilder.Append("\nError : ")
                    .Append(SQLite3.GetErrmsg(handle));

                Debug.LogError(stringBuilder.ToString());
            }

            transaction = "COMMIT";
            SQLite3Statement commitStmt;
            if (SQLite3Result.OK !=
                SQLite3.Prepare2(handle, transaction, transaction.Length, out commitStmt, IntPtr.Zero))
            {
                Debug.LogError(transaction + " \nError : " + SQLite3.GetErrmsg(commitStmt));
                SQLite3.Finalize(commitStmt);
            }

            SQLite3.Finalize(stmt);
            SQLite3.Finalize(beginStmt);
            SQLite3.Finalize(commitStmt);

            return obj;
        }

        public List<Object[]> SelectMultiData(string InTableName, string InSelectColumnName, string InCommon)
        {
            Assert.IsFalse(SQLite3DbHandle.Zero == handle);

            List<Object[]> obj = null;

            string transaction = "BEGIN TRANSACTION";
            SQLite3Statement beginStmt;
            if (SQLite3Result.OK !=
                SQLite3.Prepare2(handle, transaction, transaction.Length, out beginStmt, IntPtr.Zero))
            {
                Debug.LogError(transaction + " \nError : " + SQLite3.GetErrmsg(beginStmt));
                SQLite3.Finalize(beginStmt);
            }

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

                while (SQLite3Result.Row == SQLite3.Step(stmt))
                {
                    obj.Add(GetObjects(stmt, count));
                }
            }
            else
            {
                stringBuilder.Append("\nError : ")
                    .Append(SQLite3.GetErrmsg(handle));

                Debug.LogError(stringBuilder.ToString());
            }

            transaction = "COMMIT";
            SQLite3Statement commitStmt;
            if (SQLite3Result.OK !=
                SQLite3.Prepare2(handle, transaction, transaction.Length, out commitStmt, IntPtr.Zero))
            {
                Debug.LogError(transaction + " \nError : " + SQLite3.GetErrmsg(commitStmt));
                SQLite3.Finalize(commitStmt);
            }

            SQLite3.Finalize(stmt);
            SQLite3.Finalize(beginStmt);
            SQLite3.Finalize(commitStmt);

            return obj;
        }

        private Object[] GetObjects(SQLite3Statement InStmt, int InCount)
        {
            Object[] objs = new object[InCount];

            for (int i = 0; i < InCount; ++i)
            {
                SQLite3DataType type = SQLite3.ColumnType(InStmt, i);

                switch (type)
                {
                    case SQLite3DataType.Integer:
                        objs[i] = SQLite3.ColumnInt(InStmt, i);
                        break;
                    case SQLite3DataType.Real:
                        objs[i] = SQLite3.ColumnDouble(InStmt, i);
                        break;
                    case SQLite3DataType.Text:
                        objs[i] = SQLite3.ColumnText(InStmt, i);
                        break;
                    case SQLite3DataType.Blob:
                        objs[i] = SQLite3.ColumnBlob(InStmt, i);
                        break;
                    case SQLite3DataType.Null:
                        objs[i] = null;
                        break;
                }
            }

            return objs;
        }

        public T SelectSingleT<T>(int InKey) where T : Base, new()
        {
            return SelectSingleT<T>("ID = " + InKey);
        }

        public T SelectSingleT<T>(string InCommand, bool InIsNotFullCommand = true) where T : Base, new()
        {
            Assert.IsFalse(SQLite3DbHandle.Zero == handle);

            T t = new T();

            ClassProperty property = t.ClassPropertyInfos;

            string transaction = "BEGIN TRANSACTION";
            SQLite3Statement beginStmt;
            if (SQLite3Result.OK !=
                SQLite3.Prepare2(handle, transaction, transaction.Length, out beginStmt, IntPtr.Zero))
            {
                Debug.LogError(transaction + " \nError : " + SQLite3.GetErrmsg(beginStmt));
                SQLite3.Finalize(beginStmt);
            }

            SQLite3Statement stmt;
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
                    int count = SQLite3.ColumnCount(stmt);
                    int length = property.Infos.Length;

                    Assert.IsTrue(count == length, property.ClassName + " : 数据库列与类属性个数不一致！");

                    GetT(t, property.Infos, stmt, length);
                }
            }
            else
            {
                stringBuilder.Append("\nError : ")
                    .Append(SQLite3.GetErrmsg(handle));

                Debug.LogError(stringBuilder.ToString());

                return null;
            }

            transaction = "COMMIT";
            SQLite3Statement commitStmt;
            if (SQLite3Result.OK !=
                SQLite3.Prepare2(handle, transaction, transaction.Length, out commitStmt, IntPtr.Zero))
            {
                Debug.LogError(transaction + " \nError : " + SQLite3.GetErrmsg(commitStmt));
                SQLite3.Finalize(commitStmt);
            }

            SQLite3.Finalize(stmt);
            SQLite3.Finalize(beginStmt);
            SQLite3.Finalize(commitStmt);

            return t;
        }

        public List<T> SelectMultiT<T>(string InCommand, bool InIsNotFullCommand = true)
        {
            Assert.IsFalse(SQLite3DbHandle.Zero == handle);

            string transaction = "BEGIN TRANSACTION";
            SQLite3Statement beginStmt;
            if (SQLite3Result.OK !=
                SQLite3.Prepare2(handle, transaction, transaction.Length, out beginStmt, IntPtr.Zero))
            {
                Debug.LogError(transaction + " \nError : " + SQLite3.GetErrmsg(beginStmt));
                SQLite3.Finalize(beginStmt);
            }



            transaction = "COMMIT";
            SQLite3Statement commitStmt;
            if (SQLite3Result.OK !=
                SQLite3.Prepare2(handle, transaction, transaction.Length, out commitStmt, IntPtr.Zero))
            {
                Debug.LogError(transaction + " \nError : " + SQLite3.GetErrmsg(commitStmt));
                SQLite3.Finalize(commitStmt);
            }

            //SQLite3.Finalize(stmt);
            SQLite3.Finalize(beginStmt);
            SQLite3.Finalize(commitStmt);

            return null;
        }

        private void GetT<T>(T InT, PropertyInfo[] InInfos, SQLite3Statement InStmt, int InCount) where T : Base, new ()
        {
            Type type;
            for (int i = 0; i < InCount; ++i)
            {
                type = InInfos[i].PropertyType;

                if (typeof(int) == type)
                    InInfos[i].SetValue(InT, SQLite3.ColumnInt(InStmt, i), null);
                else if (typeof(long) == type)
                    InInfos[i].SetValue(InT, SQLite3.ColumnInt64(InStmt, i), null);
                else if (typeof(float) == type || typeof(double) == type)
                    InInfos[i].SetValue(InT, SQLite3.ColumnDouble(InStmt, i), null);
                else if (typeof(string) == type)
                    InInfos[i].SetValue(InT, SQLite3.ColumnText(InStmt, i), null);
            }
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

            Exec(stringBuilder.ToString());
        }

        public void CreateTable<T>() where T : Base
        {
            Assert.IsFalse(SQLite3DbHandle.Zero == handle);

            ClassProperty classProperty = Base.GetPropertyInfos(typeof(T));
            stringBuilder.Remove(0, stringBuilder.Length);
            stringBuilder.Append("CREATE TABLE ")
                .Append(classProperty.ClassName)
                .Append("(");
            int length = classProperty.Infos.Length;
            for (int i = 0; i < length; ++i)
            {
                stringBuilder.Append(classProperty.Infos[i].Name);

                Type type = classProperty.Infos[i].PropertyType;

                if (type == typeof(int) || type == typeof(long))
                    stringBuilder.Append(" INTEGER, ");
                else if (type == typeof(float) || type == typeof(double))
                    stringBuilder.Append(" REAL, ");
                else if (type == typeof(string))
                    stringBuilder.Append(" TEXT, ");
                else
                    stringBuilder.Append(" BLOB, ");
            }
            stringBuilder.Remove(stringBuilder.Length - 2, 2);
            stringBuilder.Append(")");

            Exec(stringBuilder.ToString());
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

            Exec(stringBuilder.ToString());
        }

        public void InsertT<T>(T InT) where T : Base
        {
            Assert.IsFalse(SQLite3DbHandle.Zero == handle);
            ClassProperty property = InT.ClassPropertyInfos;

            stringBuilder.Remove(0, stringBuilder.Length);
            stringBuilder.Append("INSERT INTO ")
                .Append(property.ClassName)
                .Append(" VALUES(");

            int length = property.Infos.Length;
            for (int i = 0; i < length; i++)
            {
                stringBuilder.Append("'").Append(property.Infos[i].GetValue(InT, null)).Append("', ");
            }
            stringBuilder.Remove(stringBuilder.Length - 2, 2);
            stringBuilder.Append(")");

            Exec(stringBuilder.ToString());
        }

        public void Update(string InTableName, string InCondition, params string[] InValues)
        {
            Assert.IsFalse(SQLite3DbHandle.Zero == handle);

            stringBuilder.Remove(0, stringBuilder.Length);
            stringBuilder.Append("UPDATE ")
                .Append(InTableName)
                .Append(" SET ");

            int length = InValues.Length;
            for (int i = 0; i < length; i++)
            {
                stringBuilder.Append(InValues[i]).Append(", ");
            }
            stringBuilder.Remove(stringBuilder.Length - 2, 2);
            stringBuilder.Append(" WHERE ")
                .Append(InCondition);

            Exec(stringBuilder.ToString());
        }

        public void UpdateT<T>(T InT) where T : Base
        {
            Assert.IsFalse(SQLite3DbHandle.Zero == handle);

            ClassProperty property = InT.ClassPropertyInfos;
            Assert.IsFalse(SQLite3DbHandle.Zero == handle);

            stringBuilder.Remove(0, stringBuilder.Length);
            stringBuilder.Append("UPDATE ")
                .Append(property.ClassName)
                .Append(" SET ");

            int length = property.Infos.Length;
            for (int i = 1; i < length; i++)
            {
                stringBuilder.Append(property.Infos[i].Name)
                    .Append(" = '")
                    .Append(property.Infos[i].GetValue(InT, null))
                    .Append("', ");
            }
            stringBuilder.Remove(stringBuilder.Length - 2, 2);
            stringBuilder.Append(" WHERE ID = ")
                .Append(property.Infos[0].GetValue(InT, null));

            Exec(stringBuilder.ToString());
        }

        public void UpdateSingleT<T, U>(T InT, U InU) where T : Base
        {
            Assert.IsFalse(SQLite3DbHandle.Zero == handle);

            ClassProperty property = InT.ClassPropertyInfos;
            int index = InU.GetHashCode();

            stringBuilder.Remove(0, stringBuilder.Length);
            stringBuilder.Append("UPDATE ")
                .Append(property.ClassName)
                .Append(" SET ")
                .Append(property.Infos[index].Name)
                .Append(" = '")
                .Append(property.Infos[index].GetValue(InT, null))
                .Append("' WHERE ID = ")
                .Append(property.Infos[0].GetValue(InT, null));

            Exec(stringBuilder.ToString());
        }

        public void Exec(string InCommand)
        {
            string transaction = "BEGIN TRANSACTION";
            SQLite3Statement beginStmt;
            if (SQLite3Result.OK !=
                SQLite3.Prepare2(handle, transaction, transaction.Length, out beginStmt, IntPtr.Zero))
            {
                Debug.LogError(transaction + " \nError : " + SQLite3.GetErrmsg(beginStmt));
                SQLite3.Finalize(beginStmt);
            }

            SQLite3Statement stmt;
            if (!(SQLite3Result.OK == SQLite3.Prepare2(handle, InCommand, InCommand.Length, out stmt, IntPtr.Zero)
                && SQLite3Result.Done == SQLite3.Step(stmt)))
                Debug.LogError(stringBuilder + "\nError : " + SQLite3.GetErrmsg(stmt));

            transaction = "COMMIT";
            SQLite3Statement commitStmt;
            if (SQLite3Result.OK !=
                SQLite3.Prepare2(handle, transaction, transaction.Length, out commitStmt, IntPtr.Zero))
            {
                Debug.LogError(transaction + " \nError : " + SQLite3.GetErrmsg(commitStmt));
                SQLite3.Finalize(commitStmt);
            }

            SQLite3.Finalize(stmt);
            SQLite3.Finalize(beginStmt);
            SQLite3.Finalize(commitStmt);
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

