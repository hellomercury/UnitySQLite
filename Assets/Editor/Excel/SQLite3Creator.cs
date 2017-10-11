using System;
using UnityEngine;
using UnityEditor;
using System.IO;
using System.Text;
using NPOI.SS.UserModel;
using NPOI.HSSF.UserModel;
using NPOI.XSSF.UserModel;
using SQLite3;
using Object = UnityEngine.Object;

public class SQLite3Creator : EditorWindow
{
    private enum ValueType
    {
        INTEGER,
        REAL,
        TEXT,
        BLOB
    }

    private class SheetParameter
    {
        public string SheetName;
        public bool IsEnable;
        public bool IsCreated;

        public ColumnParameter[] ColParameters;
        public ICell[,] SheetData;
    }

    private class ColumnParameter
    {
        public string Name;
        public string Describe;
        public ValueType Type;
        public string OriginalType;
        public bool IsEnable;
        //        public bool IsArray;
    }

    private SheetParameter[] sheetParameters;
    private static SQLite3Creator window;
    private int sheetLength;
    private string databasePath;
    private string dataPath;
    private string scriptSavePath;
    private string excelPath;
    private bool isSingleExcel;

    void OnEnable()
    {
        dataPath = Application.dataPath;
        databasePath = EditorPrefs.GetString("DatabasePath", "Assets/StreamingAssets/Database/static.db");
        scriptSavePath = EditorPrefs.GetString("ScriptSavePath", "Assets/Scripts/Data/");
        excelPath = EditorPrefs.GetString("ExcelPath", "Assets/ExcelData/");
    }

    void OnGUI()
    {
        GUILayout.Label("Excel Importer", EditorStyles.boldLabel);
        
        #region Single Excel
        if (isSingleExcel)
        {
            #region Path
            EditorGUILayout.BeginVertical("box");

            GUILayout.BeginHorizontal();
            databasePath = EditorGUILayout.TextField("Database Path", databasePath);
            if (GUILayout.Button("Open", GUILayout.MaxWidth(45)))
            {
                string path = EditorUtility.SaveFolderPanel("Database Path", dataPath + "/" + databasePath, "");
                if (!string.IsNullOrEmpty(path))
                {
                    databasePath = "Assets" + path.Replace(dataPath, "") + "/";
                    EditorPrefs.SetString("CreatorScriptPath", databasePath);
                }
            }
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            scriptSavePath = EditorGUILayout.TextField("Script Save Path", scriptSavePath);
            if (GUILayout.Button("Open", GUILayout.MaxWidth(45)))
            {
                string path = EditorUtility.SaveFolderPanel("Database Path", dataPath + "/" + scriptSavePath, "");
                if (!string.IsNullOrEmpty(path))
                {
                    scriptSavePath = "Assets" + path.Replace(dataPath, "") + "/";
                    EditorPrefs.SetString("CreatorScriptPath", scriptSavePath);
                }
            }
            GUILayout.EndHorizontal();
            EditorGUILayout.EndVertical();
            #endregion

            #region GUI

            if (null != sheetParameters)
            {
                sheetLength = sheetParameters.Length;
                for (int i = 0; i < sheetLength; i++)
                {
                    if (!sheetParameters[i].IsCreated)
                    {
                        EditorGUILayout.BeginVertical("box");
                        GUILayout.BeginHorizontal();
                        sheetParameters[i].SheetName = EditorGUILayout.TextField("Class Name",
                            sheetParameters[i].SheetName);

                        sheetParameters[i].IsEnable = EditorGUILayout.BeginToggleGroup("Enable",
                            sheetParameters[i].IsEnable);
                        EditorGUILayout.EndToggleGroup();
                        GUILayout.EndHorizontal();

                        if (sheetParameters[i].IsEnable)
                        {
                            int length = sheetParameters[i].ColParameters.Length;
                            for (int j = 0; j < length; j++)
                            {
                                sheetParameters[i].ColParameters[j].IsEnable
                                    = EditorGUILayout.BeginToggleGroup(
                                        ////sheetParameters[i].ColParameters[j].IsArray
                                        //    ? "Enable                                                                                        [Array]"
                                        //    : "Enable",
                                        "Enable",
                                        sheetParameters[i].ColParameters[j].IsEnable);

                                GUILayout.BeginHorizontal();
                                sheetParameters[i].ColParameters[j].Name =
                                    EditorGUILayout.TextField(sheetParameters[i].ColParameters[j].Name,
                                        GUILayout.MaxWidth(160));
                                sheetParameters[i].ColParameters[j].Describe =
                                    EditorGUILayout.TextField(sheetParameters[i].ColParameters[j].Describe,
                                        GUILayout.MaxWidth(240));
                                sheetParameters[i].ColParameters[j].Type =
                                    (ValueType)
                                        EditorGUILayout.EnumPopup(sheetParameters[i].ColParameters[j].Type,
                                            GUILayout.MaxWidth(100));
                                GUILayout.EndHorizontal();

                                EditorGUILayout.EndToggleGroup();
                            }

                        }

                        if (GUILayout.Button("Create"))
                        {
                            try
                            {
                                CreateDatabaseTable(sheetParameters[i].SheetName, sheetParameters[i].ColParameters,
                                    sheetParameters[i].SheetData);

                                CreateScript(sheetParameters[i].SheetName, sheetParameters[i].ColParameters);

                                sheetParameters[i].IsCreated = true;

                                CloseWindows();
                            }
                            catch (Exception e)
                            {
                                throw new Exception(e.Message);
                            }

                        }
                        EditorGUILayout.EndVertical();
                    }
                }
            }

            #endregion
        }
        #endregion

        #region Multiple Excel
        else
        {
            #region Path
            EditorGUILayout.BeginVertical("box");

            GUILayout.BeginHorizontal();
            databasePath = EditorGUILayout.TextField("Database Path", databasePath);
            if (GUILayout.Button("Open", GUILayout.MaxWidth(45)))
            {
                string path = EditorUtility.SaveFolderPanel("Database Path", dataPath + "/" + databasePath, "");
                if (!string.IsNullOrEmpty(path))
                {
                    databasePath = "Assets" + path.Replace(dataPath, "") + "/";
                    EditorPrefs.SetString("CreatorScriptPath", databasePath);
                }
            }
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            scriptSavePath = EditorGUILayout.TextField("Script Save Path", scriptSavePath);
            if (GUILayout.Button("Open", GUILayout.MaxWidth(45)))
            {
                string path = EditorUtility.SaveFolderPanel("Database Path", dataPath + "/" + scriptSavePath, "");
                if (!string.IsNullOrEmpty(path))
                {
                    scriptSavePath = "Assets" + path.Replace(dataPath, "") + "/";
                    EditorPrefs.SetString("CreatorScriptPath", scriptSavePath);
                }
            }
            GUILayout.EndHorizontal();
            EditorGUILayout.EndVertical();
            #endregion
        }
        #endregion
    }

    static void CloseWindows()
    {
        AssetDatabase.SaveAssets();

        AssetDatabase.Refresh(ImportAssetOptions.ImportRecursive);

        EditorUtility.DisplayDialog("Tips", "生成成功。", "确定");

        window.Close();
    }

    [MenuItem("Assets/Create SQLite3 Table")]
    static void ExportExcelToSQLite3()
    {
        Object obj = Selection.activeObject;
        string objPath = AssetDatabase.GetAssetPath(obj);
        FileInfo info = new FileInfo(objPath);
        if (info.Exists && (info.Extension.Equals(".xlsx") || info.Extension.Equals(".xls")))
        {
            window = CreateInstance<SQLite3Creator>();
            window.minSize = new Vector2(500, 600);
            window.maxSize = new Vector2(500, 2000);
            window.isSingleExcel = true;

            using (FileStream stream = info.Open(FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                IWorkbook book;
                if (info.Extension.Equals(".xlsx")) book = new XSSFWorkbook(stream);
                else book = new HSSFWorkbook(stream);

                int sheetCount = book.NumberOfSheets;
                window.sheetParameters = new SheetParameter[sheetCount];

                for (int i = 0; i < sheetCount; i++)
                {
                    ISheet sheet = book.GetSheetAt(i);

                    int rowCount = sheet.LastRowNum;
                    if (rowCount > 3)
                    {
                        IRow row1 = sheet.GetRow(0);
                        IRow row2 = sheet.GetRow(1);
                        IRow row3 = sheet.GetRow(2);

                        int colCount = row1.LastCellNum;
                        window.sheetParameters[i] = new SheetParameter();
                        window.sheetParameters[i].SheetName = sheet.SheetName;
                        window.sheetParameters[i].IsEnable = true;
                        window.sheetParameters[i].IsCreated = false;
                        window.sheetParameters[i].ColParameters = new ColumnParameter[colCount];

                        for (int j = 0; j < colCount; j++)
                        {
                            window.sheetParameters[i].ColParameters[j] = new ColumnParameter();
                            ICell cell = row1.GetCell(j);
                            window.sheetParameters[i].ColParameters[j].Name = null == cell
                                ? string.Empty
                                : cell.StringCellValue;

                            cell = row2.GetCell(j);
                            string type = cell == null ? "NULL" : cell.StringCellValue;
                            window.sheetParameters[i].ColParameters[j].OriginalType = type;
                            switch (type)
                            {
                                case "int":
                                case "bool":
                                    window.sheetParameters[i].ColParameters[j].Type = ValueType.INTEGER;
                                    break;
                                case "float":
                                case "double":
                                    window.sheetParameters[i].ColParameters[j].Type = ValueType.REAL;
                                    break;
                                case "string":
                                    window.sheetParameters[i].ColParameters[j].Type = ValueType.TEXT;
                                    break;
                                default:
                                    window.sheetParameters[i].ColParameters[j].Type = ValueType.BLOB;
                                    break;
                            }

                            cell = row3.GetCell(j);
                            window.sheetParameters[i].ColParameters[j].Describe = null == cell
                                ? string.Empty
                                : cell.StringCellValue;

                            window.sheetParameters[i].ColParameters[j].IsEnable = true;
                        }

                        window.sheetParameters[i].SheetData = new ICell[rowCount - 3, colCount];
                        for (int j = 3, m = 0; j < rowCount; j++, m++)
                        {
                            for (int k = 0; k < colCount; k++)
                            {
                                window.sheetParameters[i].SheetData[m, k] = sheet.GetRow(j).GetCell(k);
                            }
                        }
                    }
                }
            }

            window.Show();
        }
        else
        {
            EditorUtility.DisplayDialog("Error", "请选择一个Excel文件。", "确定");
        }
    }

    [MenuItem("Tools/Create All SQLite3 Table")]
    static void ExportAllExcelToSQLite3()
    {
        window = CreateInstance<SQLite3Creator>();
        window.minSize = new Vector2(500, 600);
        window.maxSize = new Vector2(500, 2000);
        window.isSingleExcel = false;
        window.Show();
    }



    void CreateScript(string InName, ColumnParameter[] InColParameters)
    {
        StringBuilder sb = new StringBuilder(1024);
        int length = InColParameters.Length;
        sb.Append("public enum ").Append(InName).Append("Enum\n")
            .Append("{\n");
        for (int i = 0; i < length; i++)
        {
            sb.Append("    ").Append(InColParameters[i].Name).Append(",\n");
        }
        sb.Append("    Max\n");
        sb.Append("}\n\n");

        sb.Append("public class ").Append(InName).Append("\n")
            .Append("{\n");

        for (int i = 0; i < length; i++)
        {
            sb.Append("    [Sync((int)").Append(InName).Append("Enum.").Append(InColParameters[i].Name).Append(")]\n")
                .Append("    public ")
                .Append(InColParameters[i].OriginalType)
                .Append(" ")
                .Append(InColParameters[i].Name)
                .Append(" { get; private set; }  //")
                .Append(InColParameters[i].Describe)
                .Append("\n\n");
        }

        sb.Append("}\n\n");

        if (!Directory.Exists(scriptSavePath)) Directory.CreateDirectory(scriptSavePath);
        string filepath = scriptSavePath + InName + "Data.cs";
        if (File.Exists(filepath)) File.Delete(filepath);

        File.WriteAllText(filepath, sb.ToString(), Encoding.UTF8);
    }

    void CreateDatabaseTable(string InName, ColumnParameter[] InColParameters, ICell[,] InCellData)
    {
        SQLite3Handle handle = new SQLite3Handle(databasePath, SQLite3OpenFlags.ReadWrite | SQLite3OpenFlags.Create);
        StringBuilder sb = new StringBuilder(512);

        handle.Exec("DROP TABLE IF EXISTS " + InName);

        sb.Append("CREATE TABLE ")
            .Append(InName)
            .Append("(");
        int length = InColParameters.Length;
        for (int i = 0; i < length; i++)
        {
            if (InColParameters[i].IsEnable)
            {
                sb.Append(InColParameters[i].Name)
                    .Append(" ")
                    .Append(InColParameters[i].Type)
                    .Append(", ");
            }
        }
        sb.Remove(sb.Length - 2, 2);
        sb.Append(")");

        handle.Exec(sb.ToString());

        length = InCellData.GetLength(0);
        int length1 = InCellData.GetLength(1);

        for (int i = 0; i < length; i++)
        {
            sb.Remove(0, sb.Length);
            sb.Append("INSERT INTO ").Append(InName).Append(" VALUES(");
            for (int j = 0; j < length1; j++)
            {
                switch (InColParameters[j].Type)
                {
                    case ValueType.INTEGER:
                        sb.Append(InCellData[i, j] == null ? 0 : (int)InCellData[i, j].NumericCellValue);
                        break;
                    case ValueType.REAL:
                        sb.Append(InCellData[i, j] == null ? 0 : InCellData[i, j].NumericCellValue);
                        break;
                    default:
                        sb.Append("'").Append(InCellData[i, j] == null ? "" : InCellData[i, j].StringCellValue).Append("'");
                        break;
                }
                sb.Append(", ");
            }
            sb.Remove(sb.Length - 2, 2);
            sb.Append(")");
            //Debug.LogError(sb.ToString());
            handle.Exec(sb.ToString());
        }

        handle.CloseDB();
    }
}


