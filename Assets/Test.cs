using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
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

    void Start()
    {
        Debug.LogError("-->Open DB");
        SQLite3Handle.Instance.OpenDB(Application.dataPath + @"/Database/static.db",
            SQLite3OpenFlags.ReadWrite | SQLite3OpenFlags.Create);
    }

    void OnGUI()
    {
        GUI.skin.button.fontSize = 64;
        if (GUILayout.Button("Select"))
        {
            //List<Object[]> objs = SQLite3Handle.Instance.SelectMultiData("Item", "*", "ID > 20000002");
            //Debug.LogError(objs.Count);
            //for (int i = 0; i < objs.Count; ++i)
            //{
            //   StringBuilder sb = new StringBuilder();
            //    for (int j = 0; j < objs[i].Length; ++j)
            //    {
            //        sb.Append(objs[i][j]).Append(", ");
            //    }
            //    Debug.LogError(sb.ToString());
            //}

            Item item = SQLite3Handle.Instance.SelectSingleT<Item>(20000007);
            Debug.LogError(item);

            //Person person = SQLite3Handle.Instance.SelectSingleT<Person>(21);
            //Debug.LogError(person);

            //SQLite3Handle.Instance.CreateTable("TestTable", "ID INTEGER", "Name TEXT");
            //SQLite3Handle.Instance.CreateTable(new Item());

            //SQLite3Handle.Instance.CreateTable(new Person());
        }

        if (GUILayout.Button("Create"))
        {
            //List<Object[]> objs = SQLite3Handle.Instance.SelectMultiData("TestTable", "*", "ID = 0");
            //for (int i = 0; i < objs.Count; ++i)
            //{
            //    for (int j = 0; j < objs[i].Length; ++j)
            //    {
            //        Debug.LogError(objs[i][j]);
            //    }
            //}

            //Item item = SQLite3Handle.Instance.SelectSingleT<Item>(20000006);
            //Debug.LogError(item);

            //Person person = new Person(10, "szn");
            SQLite3Handle.Instance.CreateTable<Person>();
            //Debug.LogError(person);

            //SQLite3Handle.Instance.CreateTable("TestTable", "ID INTEGER", "Name TEXT");
            //SQLite3Handle.Instance.CreateTable(new Item());

            //SQLite3Handle.Instance.CreateTable(new Person());
        }

        if (GUILayout.Button("Insert"))
        {
            //Person person = new Person(10, "szn");
            SQLite3Handle.Instance.Insert("Person", 2, "safwn");
        }

        if (GUILayout.Button("InsertT"))
        {
            Person person = new Person(0, "Insert");
            SQLite3Handle.Instance.InsertT(person);
        }

        if (GUILayout.Button("Update"))
        {
            SQLite3Handle.Instance.Update("Person", "ID = 21", "Name = 'dfasdfsd'");
        }

        if (GUILayout.Button("UpdateTU"))
        {
            Person person = new Person(0, "-----------------");
            SQLite3Handle.Instance.UpdateSingleT(person, PersonEnum.Name);
        }
    }

    void OnApplicationQuit()
    {
        Debug.LogError("-->Close DB");
        SQLite3Handle.Instance.CloseDB();
    }
}
