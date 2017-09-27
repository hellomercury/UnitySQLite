﻿using UnityEngine;
using Object = System.Object;
using System.Runtime.InteropServices;
using Sqlite3DatabaseHandle = System.IntPtr;
using Sqlite3Statement = System.IntPtr;

namespace szn
{
    public static class SQLite3
    {
        public static string GetErrmsg(Sqlite3DatabaseHandle db)
        {
            return Marshal.PtrToStringUni(Errmsg(db));
        }

        public static string ColumnName16(Sqlite3Statement stmt, int index)
        {
            return Marshal.PtrToStringUni(ColumnName16Internal(stmt, index));
        }

        public static Object GetData(Sqlite3Statement stmt, int index)
        {

            ColType type = ColumnType(stmt, index);

            switch (type)
            {
                case ColType.Integer:
                    return ColumnInt(stmt, index);
                case ColType.Float:
                    return ColumnDouble(stmt, index);
                case ColType.Text:
                    return ColumnString(stmt, index);
                case ColType.Blob:
                    return ColumnBlob(stmt, index);

                default:
                    return "null";
            }
        }

        public static string ColumnString(Sqlite3Statement stmt, int index)
        {
            return Marshal.PtrToStringUni(ColumnText16(stmt, index));
        }

        public static byte[] ColumnByteArray(Sqlite3Statement stmt, int index)
        {
            int length = ColumnBytes(stmt, index);
            var result = new byte[length];
            if (length > 0)
                Marshal.Copy(ColumnBlob(stmt, index), result, 0, length);
            return result;
        }

        #region Open
        [DllImport("sqlite3", EntryPoint = "sqlite3_open", CallingConvention = CallingConvention.Cdecl)]
        public static extern SQLiteResult Open([MarshalAs(UnmanagedType.LPStr)] string filename, out Sqlite3DatabaseHandle db);

        [DllImport("sqlite3", EntryPoint = "sqlite3_open_v2", CallingConvention = CallingConvention.Cdecl)]
        public static extern SQLiteResult Open([MarshalAs(UnmanagedType.LPStr)] string filename, out Sqlite3DatabaseHandle db, int flags, Sqlite3DatabaseHandle zvfs);

        [DllImport("sqlite3", EntryPoint = "sqlite3_open_v2", CallingConvention = CallingConvention.Cdecl)]
        public static extern SQLiteResult Open(byte[] filename, out Sqlite3DatabaseHandle db, int flags, Sqlite3DatabaseHandle zvfs);

        [DllImport("sqlite3", EntryPoint = "sqlite3_open16", CallingConvention = CallingConvention.Cdecl)]
        public static extern SQLiteResult Open16([MarshalAs(UnmanagedType.LPWStr)] string filename, out Sqlite3DatabaseHandle db);
        #endregion

        [DllImport("sqlite3", EntryPoint = "sqlite3_prepare_v2", CallingConvention = CallingConvention.Cdecl)]
        public static extern SQLiteResult Prepare2(Sqlite3DatabaseHandle db, [MarshalAs(UnmanagedType.LPStr)] string sql, int numBytes, out Sqlite3Statement stmt, Sqlite3DatabaseHandle pzTail);

        [DllImport("sqlite3", EntryPoint = "sqlite3_column_count", CallingConvention = CallingConvention.Cdecl)]
        public static extern int ColumnCount(Sqlite3Statement stmt);

        [DllImport("sqlite3", EntryPoint = "sqlite3_column_type", CallingConvention = CallingConvention.Cdecl)]
        public static extern ColType ColumnType(Sqlite3Statement stmt, int index);

        [DllImport("sqlite3", EntryPoint = "sqlite3_column_name", CallingConvention = CallingConvention.Cdecl)]
        public static extern Sqlite3DatabaseHandle ColumnName(Sqlite3Statement stmt, int index);

        [DllImport("sqlite3", EntryPoint = "sqlite3_column_name16", CallingConvention = CallingConvention.Cdecl)]
        static extern Sqlite3DatabaseHandle ColumnName16Internal(Sqlite3Statement stmt, int index);



        [DllImport("sqlite3", EntryPoint = "sqlite3_column_int", CallingConvention = CallingConvention.Cdecl)]
        public static extern int ColumnInt(Sqlite3Statement stmt, int index);

        [DllImport("sqlite3", EntryPoint = "sqlite3_column_int64", CallingConvention = CallingConvention.Cdecl)]
        public static extern long ColumnInt64(Sqlite3Statement stmt, int index);

        [DllImport("sqlite3", EntryPoint = "sqlite3_column_double", CallingConvention = CallingConvention.Cdecl)]
        public static extern double ColumnDouble(Sqlite3Statement stmt, int index);

        [DllImport("sqlite3", EntryPoint = "sqlite3_column_text", CallingConvention = CallingConvention.Cdecl)]
        public static extern Sqlite3DatabaseHandle ColumnText(Sqlite3Statement stmt, int index);

        [DllImport("sqlite3", EntryPoint = "sqlite3_column_text16", CallingConvention = CallingConvention.Cdecl)]
        public static extern Sqlite3DatabaseHandle ColumnText16(Sqlite3Statement stmt, int index);

        [DllImport("sqlite3", EntryPoint = "sqlite3_column_blob", CallingConvention = CallingConvention.Cdecl)]
        public static extern Sqlite3DatabaseHandle ColumnBlob(Sqlite3Statement stmt, int index);

        [DllImport("sqlite3", EntryPoint = "sqlite3_column_bytes", CallingConvention = CallingConvention.Cdecl)]
        public static extern int ColumnBytes(Sqlite3Statement stmt, int index);


        [DllImport("sqlite3", EntryPoint = "sqlite3_step", CallingConvention = CallingConvention.Cdecl)]
        public static extern SQLiteResult Step(Sqlite3Statement stmt);
        [DllImport("sqlite3", EntryPoint = "sqlite3_initialize", CallingConvention = CallingConvention.Cdecl)]
        public static extern SQLiteResult Initialize();

        [DllImport("sqlite3", EntryPoint = "sqlite3_close", CallingConvention = CallingConvention.Cdecl)]
        public static extern SQLiteResult Close(Sqlite3DatabaseHandle db);


        [DllImport("sqlite3", EntryPoint = "sqlite3_enable_load_extension", CallingConvention = CallingConvention.Cdecl)]
        public static extern SQLiteResult EnableLoadExtension(Sqlite3DatabaseHandle db, int onoff);

        [DllImport("sqlite3", EntryPoint = "sqlite3_shutdown", CallingConvention = CallingConvention.Cdecl)]
        public static extern SQLiteResult Shutdown();

        [DllImport("sqlite3", EntryPoint = "sqlite3_config", CallingConvention = CallingConvention.Cdecl)]
        public static extern SQLiteResult Config(ConfigOption option);


        [DllImport("sqlite3", EntryPoint = "sqlite3_win32_set_directory", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        public static extern int SetDirectory(uint directoryType, string directoryPath);


        [DllImport("sqlite3", EntryPoint = "sqlite3_busy_timeout", CallingConvention = CallingConvention.Cdecl)]
        public static extern SQLiteResult BusyTimeout(Sqlite3DatabaseHandle db, int milliseconds);


        [DllImport("sqlite3", EntryPoint = "sqlite3_changes", CallingConvention = CallingConvention.Cdecl)]
        public static extern int Changes(Sqlite3DatabaseHandle db);



        [DllImport("sqlite3", EntryPoint = "sqlite3_reset", CallingConvention = CallingConvention.Cdecl)]
        public static extern SQLiteResult Reset(Sqlite3Statement stmt);

        [DllImport("sqlite3", EntryPoint = "sqlite3_finalize", CallingConvention = CallingConvention.Cdecl)]
        public static extern SQLiteResult Finalize(Sqlite3Statement stmt);

        [DllImport("sqlite3", EntryPoint = "sqlite3_last_insert_rowid", CallingConvention = CallingConvention.Cdecl)]
        public static extern long LastInsertRowid(Sqlite3DatabaseHandle db);

        [DllImport("sqlite3", EntryPoint = "sqlite3_errmsg16", CallingConvention = CallingConvention.Cdecl)]
        public static extern Sqlite3DatabaseHandle Errmsg(Sqlite3DatabaseHandle db);

        [DllImport("sqlite3", EntryPoint = "sqlite3_bind_parameter_index", CallingConvention = CallingConvention.Cdecl)]
        public static extern int BindParameterIndex(Sqlite3Statement stmt, [MarshalAs(UnmanagedType.LPStr)] string name);

        [DllImport("sqlite3", EntryPoint = "sqlite3_bind_null", CallingConvention = CallingConvention.Cdecl)]
        public static extern int BindNull(Sqlite3Statement stmt, int index);

        [DllImport("sqlite3", EntryPoint = "sqlite3_bind_int", CallingConvention = CallingConvention.Cdecl)]
        public static extern int BindInt(Sqlite3Statement stmt, int index, int val);

        [DllImport("sqlite3", EntryPoint = "sqlite3_bind_int64", CallingConvention = CallingConvention.Cdecl)]
        public static extern int BindInt64(Sqlite3Statement stmt, int index, long val);

        [DllImport("sqlite3", EntryPoint = "sqlite3_bind_double", CallingConvention = CallingConvention.Cdecl)]
        public static extern int BindDouble(Sqlite3Statement stmt, int index, double val);

        [DllImport("sqlite3", EntryPoint = "sqlite3_bind_text16", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        public static extern int BindText(Sqlite3Statement stmt, int index, [MarshalAs(UnmanagedType.LPWStr)] string val, int n, Sqlite3DatabaseHandle free);

        [DllImport("sqlite3", EntryPoint = "sqlite3_bind_blob", CallingConvention = CallingConvention.Cdecl)]
        public static extern int BindBlob(Sqlite3Statement stmt, int index, byte[] val, int n, Sqlite3DatabaseHandle free);

        [DllImport("sqlite3", EntryPoint = "sqlite3_extended_errcode", CallingConvention = CallingConvention.Cdecl)]
        public static extern ExtendedResult ExtendedErrCode(Sqlite3DatabaseHandle db);

        [DllImport("sqlite3", EntryPoint = "sqlite3_libversion_number", CallingConvention = CallingConvention.Cdecl)]
        public static extern int LibVersionNumber();
    }

}