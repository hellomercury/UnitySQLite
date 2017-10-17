using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using System.Text;
using NPOI.SS.UserModel;
using NPOI.HSSF.UserModel;
using NPOI.XSSF.UserModel;
using Framework.SQLite3;
using Framework.Tools;
using UnityEngine.Assertions;
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
        public List<ICell[]> SheetData;
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

    private static SQLite3Creator window;
    private Vector2 scrollPos;

    private List<SheetParameter> sheetParameters;
    private int sheetLength;
    private string databasePath;
    private string dataPath;
    private string scriptSavePath;
    private string excelPath;
    private bool isSingleExcel;
    private bool isPreviewBtnEnabled;
    private bool isLoadFinished;
    private int createdSheetCount;

    void OnEnable()
    {
        dataPath = Application.dataPath;
        databasePath = EditorPrefs.GetString("DatabasePath", "DB/static.db");
        scriptSavePath = EditorPrefs.GetString("ScriptSavePath", "Assets/Scripts/Data/");
        excelPath = EditorPrefs.GetString("ExcelPath", dataPath + "/ExcelData/");
        isPreviewBtnEnabled = true;
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
            scriptSavePath = EditorGUILayout.TextField("Script Save Path", scriptSavePath);
            if (GUILayout.Button("Open", GUILayout.MaxWidth(45)))
            {
                string path = EditorUtility.SaveFolderPanel("Database Path", dataPath + "/" + scriptSavePath, "");
                if (!string.IsNullOrEmpty(path))
                {
                    scriptSavePath = path.Replace(dataPath, "") + "/";
                    EditorPrefs.SetString("ScriptSavePath", scriptSavePath);
                }
            }
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            databasePath = EditorGUILayout.TextField("Database Path", databasePath);
            if (GUILayout.Button("Open", GUILayout.MaxWidth(45)))
            {
                string path = Application.dataPath + databasePath;
                if (!File.Exists(path)) path = Application.dataPath;
                path = EditorUtility.SaveFilePanel("Database Path", path, "Static.db", "db");
                if (!string.IsNullOrEmpty(path))
                {
                    databasePath = path.Replace(Application.dataPath, "");
                    EditorPrefs.SetString("DatabasePath", databasePath);
                }
            }
            GUILayout.EndHorizontal();
            EditorGUILayout.EndVertical();
            #endregion

            #region GUI

            if (null != sheetParameters)
            {
                scrollPos = EditorGUILayout.BeginScrollView(scrollPos);
                sheetLength = sheetParameters.Count;
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
                                CreateScript(sheetParameters[i].SheetName, sheetParameters[i].ColParameters,
                                    scriptSavePath);

                                CreateDatabaseTable(sheetParameters[i].SheetName, sheetParameters[i].ColParameters,
                                    sheetParameters[i].SheetData,
                                    databasePath);

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

                EditorGUILayout.EndScrollView();
            }

            #endregion
        }
        #endregion

        #region Multiple Excel
        else
        {
            #region Tittle
            EditorGUILayout.BeginVertical("box");

            GUILayout.BeginHorizontal();
            GUI.enabled = isPreviewBtnEnabled;
            excelPath = EditorGUILayout.TextField("Excel Path", excelPath);
            if (GUILayout.Button("Open", GUILayout.MaxWidth(45)))
            {
                string path = EditorUtility.OpenFolderPanel("Excle Path", excelPath, "");
                if (!string.IsNullOrEmpty(path))
                {
                    EditorPrefs.SetString("ExcelPath", excelPath);
                }
            }
            GUILayout.EndHorizontal();

            GUI.enabled = true;
            if (isPreviewBtnEnabled && GUILayout.Button("Preview"))
            {
                isPreviewBtnEnabled = false;
                LoadAllExcel(excelPath);
            }

            if (!isPreviewBtnEnabled && GUILayout.Button("Reset"))
            {
                isPreviewBtnEnabled = true;
                isLoadFinished = false;
                sheetParameters = null;
            }
            #endregion
            if (isLoadFinished)
            {
                if (null != sheetParameters)
                {
                    #region Path
                    EditorGUILayout.BeginVertical("box");
                    GUILayout.BeginHorizontal();
                    scriptSavePath = EditorGUILayout.TextField("Script Save Path", scriptSavePath);
                    if (GUILayout.Button("Open", GUILayout.MaxWidth(45)))
                    {
                        string path = EditorUtility.SaveFolderPanel("Database Path", dataPath + "/" + scriptSavePath, "");
                        if (!string.IsNullOrEmpty(path))
                        {
                            scriptSavePath = path.Replace(dataPath, "") + "/";
                            EditorPrefs.SetString("ScriptSavePath", scriptSavePath);
                        }
                    }
                    GUILayout.EndHorizontal();

                    GUILayout.BeginHorizontal();
                    databasePath = EditorGUILayout.TextField("Database Path", databasePath);
                    if (GUILayout.Button("Open", GUILayout.MaxWidth(45)))
                    {
                        string path = Application.dataPath + databasePath;
                        if (!File.Exists(path)) path = Application.dataPath;
                        path = EditorUtility.SaveFilePanel("Database Path", path, "Static.db", "db");
                        if (!string.IsNullOrEmpty(path))
                        {
                            databasePath = path.Replace(dataPath, "") + "/";
                            EditorPrefs.SetString("DatabasePath", databasePath);
                        }
                    }
                    GUILayout.EndHorizontal();
                    if (GUILayout.Button("Create All"))
                    {
                        for (int i = 0; i < sheetLength; ++i)
                        {
                            if (!sheetParameters[i].IsCreated)
                            {
                                CreateScript(sheetParameters[i].SheetName, sheetParameters[i].ColParameters,
                                    scriptSavePath);

                                CreateDatabaseTable(sheetParameters[i].SheetName, sheetParameters[i].ColParameters,
                                    sheetParameters[i].SheetData,
                                    databasePath);

                                sheetParameters[i].IsCreated = true;
                            }
                        }

                        CloseWindows();
                    }
                    EditorGUILayout.EndVertical();
                    #endregion

                    #region GUI
                    scrollPos = EditorGUILayout.BeginScrollView(scrollPos);
                    sheetLength = sheetParameters.Count;
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
                                    CreateScript(sheetParameters[i].SheetName, sheetParameters[i].ColParameters,
                                        scriptSavePath);

                                    CreateDatabaseTable(sheetParameters[i].SheetName, sheetParameters[i].ColParameters,
                                        sheetParameters[i].SheetData,
                                        databasePath);

                                    sheetParameters[i].IsCreated = true;

                                    if (++createdSheetCount == sheetLength) CloseWindows();
                                }
                                catch (Exception e)
                                {
                                    throw new Exception(e.Message);
                                }

                            }
                            EditorGUILayout.EndVertical();
                        }
                    }
                    EditorGUILayout.EndScrollView();
                    #endregion
                }
            }
            EditorGUILayout.EndVertical();

            #endregion
        }

    }

    static void CloseWindows()
    {
        AssetDatabase.SaveAssets();

        AssetDatabase.Refresh(ImportAssetOptions.ImportRecursive);

        EditorUtility.DisplayDialog("Tips", "生成成功。", "确定");

        if (null != window) window.Close();
    }

    [MenuItem("Assets/Create SQLite3 Table")]
    static void ExportExcelToSQLite3()
    {
        Object obj = Selection.activeObject;
        List<SheetParameter> parameters = LoadOneExcel(AssetDatabase.GetAssetPath(obj));

        if (null != parameters)
        {
            window = CreateInstance<SQLite3Creator>();
            window.minSize = new Vector2(500, 600);
            window.maxSize = new Vector2(500, 2000);
            window.isSingleExcel = true;
            window.sheetParameters = parameters;
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

    static void LoadAllExcel(string InExcelDirectory)
    {
        DirectoryInfo dirInfos = new DirectoryInfo(InExcelDirectory);
        if (dirInfos.Exists)
        {
            FileInfo[] fileInfos = dirInfos.GetFiles();
            int length = fileInfos.Length;
            window.sheetParameters = new List<SheetParameter>(3 * length);
            List<SheetParameter> parameters;
            for (int i = 0; i < length; ++i)
            {
                try
                {
                    parameters = LoadOneExcel(fileInfos[i].FullName);
                    if (null != parameters) window.sheetParameters.AddRange(parameters);
                }
                catch (Exception e)
                {
                    throw new Exception(fileInfos[i].Name + "\nError : " + e.Message);
                }
                window.isLoadFinished = true;
            }
        }
    }

    static List<SheetParameter> LoadOneExcel(string InExcelPath)
    {
        FileInfo info = new FileInfo(InExcelPath);
        if (info.Exists && !info.Name.StartsWith("~")
            && (info.Extension.Equals(".xlsx") || info.Extension.Equals(".xls")))
        {
            List<SheetParameter> sheetParameters;
            using (FileStream stream = info.Open(FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                IWorkbook book;
                if (info.Extension.Equals(".xlsx")) book = new XSSFWorkbook(stream);
                else book = new HSSFWorkbook(stream);

                int sheetCount = book.NumberOfSheets;
                sheetParameters = new List<SheetParameter>(sheetCount);
                SheetParameter parameter = null;
                for (int i = 0; i < sheetCount; i++)
                {
                    ISheet sheet = book.GetSheetAt(i);

                    int rowCount = sheet.LastRowNum + 1;
                    if (rowCount > 2)
                    {
                        IRow row1 = sheet.GetRow(0);
                        IRow row2 = sheet.GetRow(1);
                        IRow row3 = sheet.GetRow(2);
                        int row1Count = row1.LastCellNum,
                            row2Count = row2.LastCellNum;

                        int colCount = row1Count == row2Count
                            ? row1Count
                            : Utility.Min(row1Count, row2Count);

                        parameter = new SheetParameter();
                        parameter.SheetName = sheet.SheetName.Equals("Sheet1") || sheet.SheetName.Equals("工作表1")
                            ? info.Name.Replace(info.Extension, "") : sheet.SheetName;
                        parameter.IsEnable = true;
                        parameter.IsCreated = false;
                        parameter.ColParameters = new ColumnParameter[colCount];

                        for (int j = 0; j < colCount; j++)
                        {
                            parameter.ColParameters[j] = new ColumnParameter();
                            ICell cell = row1.GetCell(j);
                            parameter.ColParameters[j].Name = null == cell
                                ? string.Empty
                                : cell.StringCellValue;

                            cell = row2.GetCell(j);
                            string type = cell == null ? "NULL" : cell.StringCellValue;
                            parameter.ColParameters[j].OriginalType = type;
                            switch (type)
                            {
                                case "int":
                                case "bool":
                                    parameter.ColParameters[j].Type = ValueType.INTEGER;
                                    break;
                                case "float":
                                case "double":
                                    parameter.ColParameters[j].Type = ValueType.REAL;
                                    break;
                                case "string":
                                    parameter.ColParameters[j].Type = ValueType.TEXT;
                                    break;
                                default:
                                    parameter.ColParameters[j].Type = ValueType.BLOB;
                                    break;
                            }

                            if (null != row3)
                            {
                                cell = row3.GetCell(j);

                                parameter.ColParameters[j].Describe = null == cell
                                    ? string.Empty
                                    : cell.StringCellValue;
                            }

                            parameter.ColParameters[j].IsEnable = true;
                        }

                        if (rowCount > 3)
                        {
                            parameter.SheetData = new List<ICell[]>(rowCount - 3);
                            IRow row;
                            ICell[] cells;
                            for (int j = 3; j < rowCount; j++)
                            {
                                row = sheet.GetRow(j);
                                if (null != row)
                                {
                                    cells = new ICell[colCount];
                                    for (int k = 0; k < colCount; k++)
                                    {
                                        cells[k] = row.GetCell(k);
                                    }

                                    if (null != cells[0] && cells[0].CellType == CellType.Numeric)
                                        parameter.SheetData.Add(cells);
                                }
                            }
                        }
                    }

                    if (null != parameter) sheetParameters.Add(parameter);
                    parameter = null;
                }

                stream.Close();
            }

            return sheetParameters;
        }
        else
        {
            return null;
        }
    }

    static void CreateScript(string InName, ColumnParameter[] InColParameters,
        string InScriptSavePath)
    {
        StringBuilder sb = new StringBuilder(1024);
        int length = InColParameters.Length;
        sb.Append("/*\n")
            .Append(" * 数据库数据表结构类\n")
            .Append(" * --->次类为代码自动生成，请勿手动更改<---\n")
            .Append(" * 如需进行修改，请修改脚步 SQLiteCreator 中的 CreateScript 方法\n")
            .Append(" *                                                                                                                 --szn\n")
            .Append(" */\n\n")
            .Append("using Framework.DataStruct;\n\n");
        //            .Append("namespace SQLite3.Data\n")
        //            .Append("{\n");

        sb.Append("    public enum ").Append(InName).Append("Enum\n")
            .Append("    {\n");
        for (int i = 0; i < length; i++)
        {
            sb.Append("        ").Append(InColParameters[i].Name).Append(",\n");
        }
        sb.Append("        Max\n");
        sb.Append("    }\n\n");

        sb.Append("    public class ").Append(InName).Append(" : Base").Append("\n")
            .Append("    {\n");

        sb.Append("        private readonly int hashCode;\n\n");
        for (int i = 0; i < length; i++)
        {
            sb.Append("        [Sync((int)").Append(InName).Append("Enum.").Append(InColParameters[i].Name).Append(")]\n")
                .Append("        public ")
                .Append(InColParameters[i].OriginalType)
                .Append(" ")
                .Append(InColParameters[i].Name)
                .Append(0 == i ? " { get; private set; }" : " { get; set; }");

            if (string.IsNullOrEmpty(InColParameters[i].Describe))
                sb.Append("\n\n");
            else
                sb.Append("  //").Append(InColParameters[i].Describe).Append("\n\n");
        }

        sb.Append("        public ").Append(InName).Append("()\n")
            .Append("        {\n")
            .Append("        }\n\n");

        sb.Append("        public ").Append(InName).Append("(");
        for (int i = 0; i < length; ++i)
        {
            sb.Append(InColParameters[i].OriginalType)
                .Append(" In").Append(InColParameters[i].Name)
                .Append(", ");
        }
        sb.Remove(sb.Length - 2, 2);
        sb.Append(")\n");
        sb.Append("        {\n")
            .Append("            hashCode = InID;\n");

        for (int i = 0; i < length; ++i)
        {
            sb.Append("            ").Append(InColParameters[i].Name)
                .Append(" = In").Append(InColParameters[i].Name)
                .Append(";\n");
        }
        sb.Append("        }\n\n");

        sb.Append("        public override int GetHashCode()\n")
            .Append("        {\n")
            .Append("            return hashCode;\n")
            .Append("        }\n\n");

        sb.Append("        public override string ToString()\n")
            .Append("        {\n")
            .Append("            return \"").Append(InName).Append(" : ID = \" + ID");

        for (int i = 1; i < length; ++i)
        {
            sb.Append("+ \", ").Append(InColParameters[i].Name).Append(" = \" + ").
            Append(InColParameters[i].Name);
        }
        sb.Append(";\n");
        sb.Append("        }\n\n");

        sb.Append("        public override bool Equals(object InObj)\n")
            .Append("        {\n")
            .Append("            if (null == InObj) return false;\n")
            .Append("            else return InObj is ").Append(InName).Append(" && (InObj as ")
            .Append(InName).Append(").ID == ID;\n")
            .Append("        }\n");

        sb.Append("    }\n");
        //        sb.Append("}");

        InScriptSavePath = Application.dataPath + "/" + InScriptSavePath;
        if (!Directory.Exists(InScriptSavePath)) Directory.CreateDirectory(InScriptSavePath);
        string filepath = InScriptSavePath + InName + ".cs";
        if (File.Exists(filepath)) File.Delete(filepath);

        File.WriteAllText(filepath, sb.ToString(), Encoding.UTF8);
    }

    static void CreateDatabaseTable(string InName, ColumnParameter[] InColParameters, List<ICell[]> InCellData,
        string InDatabasePath)
    {
        SQLite3Handle handle = new SQLite3Handle(Application.dataPath + "/" + InDatabasePath, SQLite3OpenFlags.Create | SQLite3OpenFlags.ReadWrite);

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

        if (null != InCellData)
        {
            length = InCellData.Count;
            int length1 = InCellData[0].Length;
            ICell cell;
            for (int i = 0; i < length; i++)
            {
                sb.Remove(0, sb.Length);
                sb.Append("INSERT INTO ").Append(InName).Append(" VALUES(");
                for (int j = 0; j < length1; j++)
                {
                    cell = InCellData[i][j];
                    switch (InColParameters[j].Type)
                    {
                        case ValueType.INTEGER:
                            if (null == cell)
                                sb.Append(0);
                            else
                            {
                                switch (cell.CellType)
                                {
                                    case CellType.Numeric:
                                        sb.Append((int)cell.NumericCellValue);
                                        break;

                                    case CellType.String:
                                        int result;
                                        sb.Append(int.TryParse(cell.StringCellValue, out result)
                                            ? result
                                            : 0);
                                        break;

                                    case CellType.Boolean:
                                        sb.Append(cell.BooleanCellValue ? 1 : 0);
                                        break;

                                    default:
                                        sb.Append(0);
                                        break;
                                }
                            }
                            break;

                        case ValueType.REAL:
                            if (null == cell)
                                sb.Append(0);
                            else
                            {
                                switch (cell.CellType)
                                {
                                    case CellType.Numeric:
                                        sb.Append(cell.NumericCellValue);
                                        break;

                                    case CellType.String:
                                        double result;
                                        sb.Append(double.TryParse(cell.StringCellValue, out result)
                                            ? result
                                            : 0);
                                        break;

                                    case CellType.Boolean:
                                        sb.Append(cell.BooleanCellValue ? 1 : 0);
                                        break;

                                    default:
                                        sb.Append(0);
                                        break;
                                }
                            }
                            break;

                        default:
                            if (null == cell)
                                sb.Append("''");
                            else
                            {
                                switch (cell.CellType)
                                {
                                    case CellType.Numeric:
                                        sb.Append("\'")
                                            .Append(cell.NumericCellValue)
                                            .Append("\'");
                                        break;

                                    case CellType.String:
                                        sb.Append("\'")
                                            .Append(cell.StringCellValue.Replace("'", "''"))
                                            .Append("\'");
                                        break;

                                    case CellType.Boolean:
                                        sb.Append("\'")
                                            .Append(cell.BooleanCellValue.ToString())
                                            .Append("\'");
                                        break;

                                    default:
                                        sb.Append("''");
                                        break;
                                }
                            }
                            break;
                    }
                    sb.Append(", ");
                }
                sb.Remove(sb.Length - 2, 2);
                sb.Append(")");
                //Debug.LogError(sb.ToString());
                handle.Exec(sb.ToString());
            }
        }

        handle.CloseDB();
    }

    static int GetMin(params int[] InParams)
    {
        Assert.IsNotNull(InParams);

        int length = InParams.Length;
        int min = length == 0 ? 0 : InParams[0];
        for (int i = 0; i < length; ++i)
        {
            if (min > InParams[i])
            {
                min = InParams[i];
            }
        }

        return min;
    }
}


