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

public struct ClassProperty
{
    public string ClassName;
    public PropertyInfo[] Infos;

    public ClassProperty(string InClassName, PropertyInfo[] inInfos)
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

    private static Dictionary<Type, ClassProperty> typeDict;

    public ClassProperty ClassPropertyInfos
    {
        get { return typeDict[GetType()]; }
    }

    public static ClassProperty GetPropertyInfos(Type InKey)
    {
        ClassProperty property;
        return null != typeDict && typeDict.TryGetValue(InKey, out property)
            ? property : InitProperty(InKey);
    }

    protected Base()
    {
        InitProperty(GetType());
    }

    private static ClassProperty InitProperty(Type InType)
    {
        if (null == typeDict) typeDict = new Dictionary<Type, ClassProperty>();
        ClassProperty property;
        if (!typeDict.TryGetValue(InType, out property))
        {
            PropertyInfo[] properties = InType.GetProperties(BindingFlags.Public
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

            property = new ClassProperty(InType.Name, propertyInfos);
            typeDict.Add(InType, property);
        }

        return property;
    }
}
