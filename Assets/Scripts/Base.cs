using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public class Base
{
    private static Base instance;

    public static Base Instance
    {
        get
        {
            if (null == instance) instance = new Base();
            return instance;
        }
    }

    private static PropertyInfo[] propertyInfos;

    public PropertyInfo[] PropertyInfos
    {
        get
        {
            if (null == propertyInfos)
            {

                PropertyInfo[] properties = GetType().GetProperties(BindingFlags.Public
                    | BindingFlags.NonPublic | BindingFlags.Instance);
                int length = properties.Length;
                //Debug.LogError("<>" + length);
                Dictionary<int, PropertyInfo> propertyInfoDict = new Dictionary<int, PropertyInfo>(length);
                Type type = typeof(SyncAttribute);
                for (int i = 0; i < length; ++i)
                {
                    object[] objs = properties[i].GetCustomAttributes(type, false);
                    if (objs.Length == 1 && objs[0] is SyncAttribute)
                        propertyInfoDict.Add((objs[0] as SyncAttribute).SyncID, properties[i]);
                }

                length = propertyInfoDict.Count;
                propertyInfos = new PropertyInfo[length];
                for (int i = 0; i < length; ++i)
                {
                    propertyInfos[i] = propertyInfoDict[i];
                }
            }

            return propertyInfos;
        }
    }
}

[AttributeUsage(AttributeTargets.Property)]
public class SyncAttribute : Attribute
{
    public int SyncID { get; }

    public SyncAttribute(int InSyncID)
    {
        SyncID = InSyncID;
    }
}
