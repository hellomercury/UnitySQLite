using System;
using System.Diagnostics;
using szn;
using UnityEngine;
using Debug = UnityEngine.Debug;
using Object = System.Object;

public class Test : MonoBehaviour
{
    //void Update()
    //{
    //    if (Input.GetMouseButtonUp(0))
    //    {
    //       // Stopwatch sp = new Stopwatch();
    //        int length = 90000;
    //        //sp.Start();
    //        for (int i = 0; i < length; ++i)
    //        {
    //            TestInt(i);
    //        }
    //        //sp.Stop();
    //        //Debug.LogError(sp.ElapsedMilliseconds);
    //        //sp.Reset();
    //        //sp.Start();
    //        for (int i = 0; i < length; ++i)
    //        {
    //            TestObj(i);
    //        }
    //        //sp.Stop();
    //        //Debug.LogError(sp.ElapsedMilliseconds);
    //    }

    //}


    //void TestInt(int InValue)
    //{
    //    int a = InValue;
    //}

    //void TestObj(object InValue)
    //{
    //    int a = (int)InValue;
    //}

    // Update is called once per frame
    void OnGUI()
    {
        GUI.skin.button.fontSize = 64;
        if (GUILayout.Button("Open"))
        {
            SQLiteHandle.Instance.OpenDB(@"D:\Unity\Self\SqliteNew\Assets\Database\static.db",
       SQLiteOpenFlags.ReadWrite | SQLiteOpenFlags.Create);

            //List<Object[]> objs = SQLiteHandle.Instance.SelectMultiData("Item", "*", "ID = 20000007");
            //for (int i = 0; i < objs.Count; ++i)
            //{
            //    for (int j = 0; j < objs[i].Length; ++j)
            //    {
            //        Debug.LogError(objs[i][j]);
            //    }
            //}

            Item item = SQLiteHandle.Instance.SelectSingleT<Item>("Item", 20000007);
            //            Debug.LogError(item);

            SQLiteHandle.Instance.CloseDB();
        }

        //if (GUILayout.Button("Read"))
        //{

        //}
    }

    //    //void OnApplicationQuit()
    //    //{
    //    //    SQLiteHandle.Instance.CloseDB();
    //    //}
}
