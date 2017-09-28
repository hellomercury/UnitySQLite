using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;

[AttributeUsage(AttributeTargets.Property)]
public class SyncAttribute : Attribute
{
    public int SyncID { get; }

    public SyncAttribute(int InSyncID)
    {
        SyncID = InSyncID;
    }
}

public struct PropertyStruct
{
    public string ClassName;
    public PropertyInfo[] Infos;

    public PropertyStruct(string InClassName, PropertyInfo[] inInfos)
    {
        ClassName = InClassName;
        Infos = inInfos;
    }
}

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

    private static Dictionary<Type, PropertyStruct> typeDict;

    public PropertyStruct PropertyInfos
    {
        get { return typeDict[GetType()]; }
    }

    public static PropertyStruct GetPropertyInfos(Type InKey)
    {
        return typeDict[InKey];
    }

    protected Base()
    {
        if (null == typeDict) typeDict = new Dictionary<Type, PropertyStruct>();
        Type type = GetType();
        if (!typeDict.ContainsKey(type))
        {
            UnityEngine.Debug.LogError("----");
            PropertyInfo[] properties = type.GetProperties(BindingFlags.Public
                                                                | BindingFlags.NonPublic | BindingFlags.Instance);
            int length = properties.Length;
            Dictionary<int, PropertyInfo> propertyInfoDict = new Dictionary<int, PropertyInfo>(length);
            Type attrType = typeof(SyncAttribute);
            for (int i = 0; i < length; ++i)
            {
                object[] objs = properties[i].GetCustomAttributes(attrType, false);
                if (objs.Length == 1 && objs[0] is SyncAttribute)
                    propertyInfoDict.Add(((SyncAttribute)objs[0]).SyncID, properties[i]);
            }

            length = propertyInfoDict.Count;
            PropertyInfo[] propertyInfos = new PropertyInfo[length];
            for (int i = 0; i < length; ++i)
            {
                propertyInfos[i] = propertyInfoDict[i];
            }

            typeDict.Add(type, new PropertyStruct(type.Name, propertyInfos));
        }
    }
}
