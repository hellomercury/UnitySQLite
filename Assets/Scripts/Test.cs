using System.Text;
using UnityEngine;
using Debug = UnityEngine.Debug;
using Object = System.Object;
using SQLite3;

public class Test : MonoBehaviour
{
    public TextAsset text;
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


    private SQLite3Handle handle;
    void Start()
    {
        handle = new SQLite3Handle(Application.dataPath + @"/Database/existing.db");
        //Player.Instance.RegisterPropertyChanged("Level", (obj, propertyName, value, oldValue) =>
        //{
        //    Debug.LogError(oldValue +" > "+ value);
        //});
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F1))
        {
            for (int i = 0; i < 10000; ++i)
            {
                handle.SelectSingleT<Person>(Random.Range(1, 32));
            }
        }
    }

    //void OnGUI()
    //{
    //    GUI.skin.button.fontSize = 64;
    //    if (GUILayout.Button("Select"))
    //    {
    //        //List<Object[]> objs = SQLite3Handle.Instance.SelectMultiData("Item", "*", "ID > 20000002");
    //        //Debug.LogError(objs.Count);
    //        //for (int i = 0; i < objs.Count; ++i)
    //        //{
    //        //   StringBuilder sb = new StringBuilder();
    //        //    for (int j = 0; j < objs[i].Length; ++j)
    //        //    {
    //        //        sb.Append(objs[i][j]).Append(", ");
    //        //    }
    //        //    Debug.LogError(sb.ToString());
    //        //}

    //        Person item = SQLite3Handle.Instance.SelectSingleT<Person>(2);
    //        Debug.LogError(item);

    //        //Person person = SQLite3Handle.Instance.SelectSingleT<Person>(21);
    //        //Debug.LogError(person);

    //        //SQLite3Handle.Instance.CreateTable("TestTable", "ID INTEGER", "Name TEXT");
    //        //SQLite3Handle.Instance.CreateTable(new Item());

    //        //SQLite3Handle.Instance.CreateTable(new Person());
    //    }

    //    if (GUILayout.Button("Create"))
    //    {
    //        //Person person = new Person(10, "szn");
    //        SQLite3Handle.Instance.CreateTable<Person>();
    //        //Debug.LogError(person);

    //        //SQLite3Handle.Instance.CreateTable("TestTable", "ID INTEGER", "Name TEXT");
    //        //SQLite3Handle.Instance.CreateTable(new Item());

    //        //SQLite3Handle.Instance.CreateTable(new Person());
    //    }

    //    if (GUILayout.Button("Insert"))
    //    {
    //        //Person person = new Person(10, "szn");
    //        //SQLite3Handle.Instance.Insert("Person", 2, "safwn");
    //        SQLite3Handle.Instance.Exec(text.text);
    //    }

    //    if (GUILayout.Button("InsertT"))
    //    {
    //        Person person = new Person(1, "申兆南");
    //        SQLite3Handle.Instance.InsertT(person);
    //    }

    //    if (GUILayout.Button("Update"))
    //    {
    //        SQLite3Handle.Instance.Update("Person", "ID = 2", "Name = '申兆南'");
    //    }

    //    if (GUILayout.Button("UpdateTU"))
    //    {
    //        Person person = new Person(0, "-----------------");
    //        SQLite3Handle.Instance.UpdateSingleT(person, PersonEnum.Name);
    //    }

    //    if (GUILayout.Button("Update Level"))
    //    {
    //        Player.Instance.OnSyncOne(PlayerEnum.Level.GetHashCode(), 1);
    //    }
    //}

    void OnApplicationQuit()
    {
        Debug.LogError("-->Close DB");
        handle.CloseDB();
    }
}
