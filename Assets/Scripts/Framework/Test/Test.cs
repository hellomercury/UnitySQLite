using System.Collections.Generic;
using Framework.SQLite3;
using Framework.Tools;
using UnityEngine;

namespace Framework.Test
{
    public class Test : MonoBehaviour
    {

        void OnGUI()
        {
            GUI.skin.button.fontSize = 64;
            if (GUILayout.Button("W"))
            {
                SQLite3Handle handle = new SQLite3Handle(Application.dataPath + "/StreamingAssets/Database/static.db");
                Dictionary<int, Item> item = handle.SelectMultiT<Item>();
                //foreach (KeyValuePair<int, Item> keyValuePair in item)
                //{
                //    Debug.LogError(keyValuePair.Value);
                //}
            }

            if (GUILayout.Button("R"))
            {

            }
        }

        void OnApplicationQuit()
        {
            //DynamicData.GetInstance().CloseDB();
        }
    }

    [System.Serializable]
    public class STest
    {
        public int ID;

        public STest(int InID)
        {
            ID = InID;
        }
    }

    public class NTest
    {

    }
}

