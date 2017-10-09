using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEditor;

public class Listener : AssetPostprocessor
{
    static void OnPostprocessAllAssets(string[] InImportedAssets,
        string[] InDeletedAssets, string[] InMovedAssets, string[] InMovedFromAssetPaths)
    {
        int length = InImportedAssets.Length;
        for (int i = 0; i < length; i++)
        {
            if (waitDo.ContainsKey(InImportedAssets[i]))
            {
                key = InImportedAssets[i];
            }
        }
    }

    private static string key;
    private static Dictionary<string, string> waitDo = new Dictionary<string, string>(10);
    public static void WaitToDo(string InPath, string InData)
    {
        EditorApplication.update += Update;
        if (waitDo.ContainsKey(InPath)) waitDo[InPath] = InData;
        else waitDo.Add(InPath, InData);
    }

    static void Update()
    {
        if (!EditorApplication.isCompiling)
        {
            if (waitDo.Count > 0)
            {
                MethodInfo info = Type.GetType(waitDo[key] + "Creator").GetMethod("Create", BindingFlags.Static);
                info.Invoke(null, null);

                waitDo.Remove(key);
                key = string.Empty;

                Debug.LogError("<--->");
            }
        }
    }
}