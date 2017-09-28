//using System;
//using System.Collections.Generic;
//using System.Reflection;
//using UnityEngine;
//using UnityEngine.Assertions;

//namespace szn
//{
//    [AttributeUsage(AttributeTargets.Property)]
//    public class NetSyncAttribute : Attribute
//    {
//        public int NetAttrID { get; }

//        public NetSyncAttribute(int InAttrID)
//        {
//            NetAttrID = InAttrID;
//        }
//    }

//    public class NetSyncFactory
//    {
//        private Type mType;
//        private List<PropertyInfo> mPropertyInfoList;

//        public NetSyncFactory(Type InType)
//        {
//            mType = InType;

//            Dictionary<int, PropertyInfo> propertiesDict = new Dictionary<int, PropertyInfo>();

//            PropertyInfo[] properties = InType.GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
//            int length = properties.Length;
//            for (int i = 0; i < length; ++i)
//            {
//                object[] objs = properties[i].GetCustomAttributes(typeof(NetSyncAttribute), false);
//                if (objs.Length == 1 && objs[0] is NetSyncAttribute)
//                {
//                    propertiesDict.Add((objs[0] as NetSyncAttribute).NetAttrID, properties[i]);
//                }
//            }

//            mPropertyInfoList = new List<PropertyInfo>();
//            length = propertiesDict.Count;
//            PropertyInfo info;
//            for (int i = 0; i < length; i++)
//            {
//                if (propertiesDict.TryGetValue(i, out info))
//                {
//                    mPropertyInfoList.Add(info);
//                }
//                else
//                {
//                    Debug.LogError(mType.Name + " 少定义了第 " + i + " 个属性，请检查并添加！");
//                }

//            }
//        }

//        public void OnNetSync(object InObject, int InID, object InValue)
//        {
//            if (null == InObject)
//            {
//                Debug.LogError("传入对象为空！");
//                return;
//            }

//            if (InObject.GetType() != mType)
//            {
//                Debug.LogError("传入对象与原对象不符！");
//                return;
//            }

//            if (InID >= mPropertyInfoList.Count)
//            {
//                Debug.Log("字段不一致，请查看：" + mType + " 的第 " + InID + " 个字段！");

//                return;
//            }

//            PropertyInfo property = mPropertyInfoList[InID];

//            object value = ConvertType(InValue, property.PropertyType);
//            object oldValue = property.GetValue(InObject, null);
//            if (!InValue.Equals(oldValue))
//            {
//                property.SetValue(InObject, value, null);

//                if (InObject is bgoBase)
//                {
//                    (InObject as bgoBase).OnPropertyChanged(property.Name, value, oldValue);
//                }
//            }
//        }

//        private object ConvertType(object InObj, Type InType)
//        {
//            object obj = null;

//            if (typeof(byte) == InType)
//            {
//                obj = Convert.ToInt16(InObj);
//            }
//            else if (typeof(int) == InType)
//            {
//                obj = Convert.ToInt32(InObj);
//            }
//            else if (typeof(long) == InType)
//            {
//                obj = Convert.ToInt64(InObj);
//            }
//            else if (typeof(float) == InType)
//            {
//                obj = Convert.ToSingle(InObj);
//            }
//            else if (typeof(double) == InType)
//            {
//                obj = Convert.ToDouble(InObj);
//            }
//            else if (typeof(string) == InType)
//            {
//                obj = Convert.ToString(InObj);
//            }

//            return obj;
//        }
//        private static Dictionary<Type, NetSyncFactory> Factories = new Dictionary<Type, NetSyncFactory>();
//        public static NetSyncFactory CreateOrGetFactory(Type InType)
//        {
//            NetSyncFactory factory;
//            if (!Factories.TryGetValue(InType, out factory))
//            {
//                try
//                {
//                    factory = new NetSyncFactory(InType);
//                }
//                catch (Exception e)
//                {
//                    Debug.LogError(InType + " 创建同步工厂出错：" + e.Message);
//                }

//                Factories.Add(InType, factory);
//            }

//            return factory;
//        }

//    }

//    public class bgoBase
//    {
//        public delegate void DelPropertyChanged(bgoBase InObject, string InPropertyName, object InCurrentValue, object InOldValue);
//        private Dictionary<string, DelPropertyChanged> mPropertyChangedDict;
//        protected NetSyncFactory SyncFactory;

//        public bgoBase()
//        {
//            SyncFactory = NetSyncFactory.CreateOrGetFactory(GetType());

//            mPropertyChangedDict = new Dictionary<string, DelPropertyChanged>();
//        }

//        internal void OnNetSyncAll(object InObj)
//        {
//            if (SyncFactory != null)
//            {
//                FieldInfo[] infos = InObj.GetType().GetFields();
//                int length = infos.Length;
//                for (int i = 0; i < length; i++)
//                {

//                    object obj = infos[i].GetValue(InObj);
//                    Debug.LogError(obj);
//                    SyncFactory.OnNetSync(this, i, obj);
//                }
//            }
//            else
//            {
//                Debug.LogError("Object does not have NetSync！");
//            }
//        }

//        internal void OnNetSyncAll(IntPtr InStmt)
//        {
//            Assert.IsNotNull(SyncFactory);

//            int length = SQLite3.ColumnCount(InStmt);
//            for (int i = 0; i < length; ++i)
//            {
//               System.Object obj = SQLite3.GetData(InStmt, i);

//                SyncFactory.OnNetSync(this, i, obj);
//            }
//        }

//        internal void OnNetSyncOne(int InID, object InValue)
//        {
//            if (SyncFactory != null)
//            {
//                SyncFactory.OnNetSync(this, InID, InValue);
//            }
//            else
//            {
//                Debug.LogError("Object does not have NetSync！");
//            }
//        }

//        /// <param name="InEnumType">当前类对应的Enum的Type</param>
//        /// <param name="InField">变量名</param>
//        /// <param name="InValue"></param>
//        internal void OnNetSyncOne(Type InEnumType, string InField, object InValue)
//        {
//            if (SyncFactory != null)
//            {
//                OnNetSyncOne((Enum.Parse(InEnumType, InField)).GetHashCode(), InValue);
//            }
//            else
//            {
//                Debug.LogError("Object does not have NetSync！");
//            }

//        }

//        public void RegisterPropertyChanged(string InPropertyName, DelPropertyChanged InPropChangedHandler)
//        {
//            if (mPropertyChangedDict.ContainsKey(InPropertyName))
//            {
//                mPropertyChangedDict[InPropertyName] += InPropChangedHandler;
//            }
//            else
//            {
//                mPropertyChangedDict.Add(InPropertyName, InPropChangedHandler);
//            }
//        }

//        public void UnRegisterPropertyChanged(string InPropertyName, DelPropertyChanged InPropChangedHandler)
//        {
//            if (mPropertyChangedDict.ContainsKey(InPropertyName))
//            {
//                mPropertyChangedDict[InPropertyName] -= InPropChangedHandler;
//            }
//        }

//        internal void OnPropertyChanged(string InPropertyName, object InPropertyValue, object InOldValue)
//        {
//            DelPropertyChanged del;
//            if (mPropertyChangedDict.TryGetValue(InPropertyName, out del)
//                    && del != null)
//            {
//                del(this, InPropertyName, InPropertyValue, InOldValue);
//            }
//        }
//    }
//}
