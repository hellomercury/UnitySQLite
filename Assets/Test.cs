using System;
using System.Collections;
using System.Collections.Generic;
using szn;
using UnityEngine;

public class Test : MonoBehaviour
{
    void Start()
    {
        SQLiteHandle.Instance.OpenDB(@"D:\Unity\Self\SqliteNew\Assets\Database\static.db",
       SQLiteOpenFlags.ReadWrite | SQLiteOpenFlags.Create);
    }

    // Update is called once per frame
    void Update()
    {
        //GUI.skin.button.fontSize = 64;
        //if (GUILayout.Button("Open"))
        if(Input.GetKeyUp(KeyCode.F1))
        {
            Person p = new Person();
            IntPtr stmt = SQLiteHandle.Instance.GetTable();
            p.OnNetSyncAll(stmt);

            SQLite3.Finalize(stmt);
            
           // Debug.LogError(p.Id + ", " + p.Name);
        }



        //if (GUILayout.Button("Read"))
        //{

        //}
    }

    void OnApplicationQuit()
    {
        SQLiteHandle.Instance.CloseDB();
    }
}
