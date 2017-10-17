using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security.Cryptography;
using System.Text;
using Framework.SQLite3;
using UnityEngine;
using UnityEngine.Assertions;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

namespace Framework.Tools
{
    public static class Utility
    {
        public static int RandomRange(int InMin, int InMax)
        {
            return InMin == InMax ? InMin : Random.Range(InMin, InMax + 1);
        }

        public static float RandomRange(float InMin, float InMax)
        {
            return InMin == InMax ? InMin : Random.Range(InMin, InMax + 1);
        }

        public static int Max(params int[] InValues)
        {
            Assert.IsNotNull(InValues);

            int length = InValues.Length;
            if (0 == length) return int.MaxValue;
            else
            {
                int max = InValues[0];
                for (int i = 1; i < length; ++i)
                {
                    if (max < InValues[i]) max = InValues[i];
                }
                return max;
            }
        }

        public static float Max(params float[] InValues)
        {
            Assert.IsNotNull(InValues);

            int length = InValues.Length;
            if (0 == length) return float.MaxValue;
            else
            {
                float max = InValues[0];
                for (int i = 1; i < length; ++i)
                {
                    if (max < InValues[i]) max = InValues[i];
                }
                return max;
            }
        }

        public static int Min(params int[] InValues)
        {
            Assert.IsNotNull(InValues);

            int length = InValues.Length;
            if (0 == length) return int.MinValue;
            else
            {
                int min = InValues[0];
                for (int i = 1; i < length; ++i)
                {
                    if (min > InValues[i]) min = InValues[i];
                }
                return min;
            }
        }

        public static float Min(params float[] InValues)
        {
            Assert.IsNotNull(InValues);

            int length = InValues.Length;
            if (0 == length) return float.MinValue;
            else
            {
                float min = InValues[0];
                for (int i = 1; i < length; ++i)
                {
                    if (min < InValues[i]) min = InValues[i];
                }
                return min;
            }
        }

        public static GameObject AddChild(this GameObject InParent)
        {
            return InParent == null ? new GameObject() : AddChild(InParent.transform);
        }

        public static GameObject AddChild(this Transform InParent)
        {
            GameObject go = new GameObject();

            if (InParent != null)
            {
                Transform trans = go.transform;
                trans.SetParent(InParent);
                trans.localPosition = Vector3.zero;
                trans.localRotation = Quaternion.identity;
                trans.localScale = Vector3.one;
            }

            return go;
        }

        public static GameObject AddChild(this GameObject InParent, GameObject InPrefab)
        {
            return InParent == null ? Object.Instantiate(InPrefab) : AddChild(InParent.transform, InPrefab);
        }

        public static GameObject AddChild(this Transform InParent, GameObject InPrefab)
        {
            GameObject go = Object.Instantiate(InPrefab);

            if (go != null && InParent != null)
            {
                Transform trans = go.transform;
                trans.SetParent(InParent);
                trans.localPosition = Vector3.zero;
                trans.localRotation = Quaternion.identity;
                trans.localScale = Vector3.one;
            }
            return go;
        }

        public static void ClearChild(this GameObject InParent)
        {
            if (null != InParent) ClearChild(InParent.transform);
        }

        public static void ClearChild(this Transform InParent)
        {
            if (null != InParent)
            {
                int count = InParent.childCount;
                for (int i = 0; i < count; i++)
                {
                    Object.Destroy(InParent.GetChild(0).gameObject);
                }
            }
        }

        public static T AddChild<T>(this GameObject InParent) where T : Component
        {
            GameObject go = AddChild(InParent);
            go.name = typeof(T).Name;
            return go.AddComponent<T>();
        }

        public static Vector3 MultVector3(Vector3 InVector3, float InMult)
        {
            InVector3.x *= InMult;
            InVector3.y *= InMult;
            InVector3.z *= InMult;

            return InVector3;
        }

        public static string GetStreamingAssetsPath()
        {
            switch (Application.platform)
            {
                case RuntimePlatform.Android:
                    return "jar:file://" + Application.dataPath + "!/assets/";

                case RuntimePlatform.IPhonePlayer:
                    return Application.dataPath + "/Raw/";

                default:
                    return Application.dataPath + "/StreamingAssets/";
            }
        }

        public static int RandomWithWeight(int[] InValues, int[] InWeights)
        {
            Assert.IsNotNull(InValues);
            Assert.IsNotNull(InWeights);
            Assert.IsTrue(InValues.Length == InWeights.Length);
            int length = InWeights.Length;

            int sum = 0;
            for (int i = 0; i < length; ++i)
            {
                sum += InWeights[i];
            }
            int random = Random.Range(0, sum);
            sum = 0;
            for (int i = 0; i < length; ++i)
            {
                sum += InWeights[i];
                if (random <= sum) return InValues[i];
            }

            return InValues[length - 1];
        }

        public static void ResetPivot(this RectTransform InRectTrans, Vector2 InPivot)
        {
            Vector2 pivot = InRectTrans.pivot;
            if (!InPivot.Equals(pivot))
            {
                InRectTrans.pivot = InPivot;

                Vector2 sizeDelta = InRectTrans.sizeDelta;
                Vector3 pos = InRectTrans.localPosition;
                pos.x += sizeDelta.x * (InPivot.x - pivot.x);
                pos.y += sizeDelta.y * (InPivot.y - pivot.y);

                InRectTrans.localPosition = pos;
            }
        }

        public static List<T> CloneListSerializable<T>(this List<T> InList)
        {
            using (MemoryStream memoryStream = new MemoryStream())
            {
                IFormatter formatter = new BinaryFormatter();
                formatter.Serialize(memoryStream, InList);
                memoryStream.Seek(0, SeekOrigin.Begin);

                return formatter.Deserialize(memoryStream) as List<T>;
            }
        }

        public static Dictionary<T, List<TU>> CloneDictionary<T, TU>(Dictionary<T, List<TU>> InDictionary)
        {
            using (MemoryStream memoryStream = new MemoryStream())
            {
                IFormatter formatter = new BinaryFormatter();
                formatter.Serialize(memoryStream, InDictionary);
                memoryStream.Seek(0, SeekOrigin.Begin);
                return formatter.Deserialize(memoryStream) as Dictionary<T, List<TU>>;
            }
        }

        private static Dictionary<int, int> timer;
        private static WaitForSeconds wait;
        private static int uniqueID;
        public static int GetUniqueID()
        {
            return ++uniqueID;
        }

        public static int TimePause(int InKey)
        {
            int result = 0;
            if (null != timer && timer.ContainsKey(InKey))
            {
                result = timer[InKey];
                timer[InKey] = -1;
            }

            return result;
        }

        public static void Timer(this MonoBehaviour InBehaviour, int InTime, Action<bool> InAction, int InUniqueID = int.MaxValue)
        {
            Assert.IsNotNull(InAction);

            InBehaviour.StartCoroutine(Timer(InTime, InAction, InUniqueID));
        }

        public static IEnumerator Timer(int InTime, Action<bool> InAction, int InUniqueID = int.MaxValue)
        {
            Assert.IsNotNull(InAction);

            if (null == wait) wait = new WaitForSeconds(1);
            if (null == timer) timer = new Dictionary<int, int>(10);
            int key = InUniqueID;
            if (InUniqueID == int.MaxValue) key = timer.Count + 1;
            if (timer.ContainsKey(key))
            {
                Debug.LogError("传入的ID不唯一，请确认不会覆盖别的倒计时！");
                yield break;
            }
            timer[key] = InTime;

            while (timer[key] > 0)
            {
                yield return wait;
                --timer[key];
                --InTime;
            }

            timer.Remove(key);

            InAction(0 == InTime);
        }

        public static void Countdown(this MonoBehaviour InBehaviour, int InTime,
            Action<int> InAction, Action<bool> InOnFinished = null,
            int InUniqueID = int.MaxValue)
        {
            Assert.IsNotNull(InAction);

            InBehaviour.StartCoroutine(Countdown(InTime, InAction, InOnFinished, InUniqueID));
        }

        public static IEnumerator Countdown(int InTime,
            Action<int> InAction, Action<bool> InOnFinished = null,
            int InUniqueID = int.MaxValue)
        {
            Assert.IsNotNull(InAction);

            if (null == wait) wait = new WaitForSeconds(1);
            if (null == timer) timer = new Dictionary<int, int>(10);
            int key = InUniqueID;
            if (InUniqueID == int.MaxValue) key = timer.Count + 1;
            if (timer.ContainsKey(key))
            {
                Debug.LogError("传入的ID不唯一，请确认不会覆盖别的倒计时！");
                yield break;
            }
            timer[key] = InTime;

            while (timer[key] > 0)
            {
                InAction(InTime);
                yield return wait;
                --timer[key];
                --InTime;
            }
            if (0 == InTime) InAction(0);

            InOnFinished(0 == InTime);
            timer.Remove(key);
        }

        public enum TimeType
        {
            hhmmss,
            mmss,
            ss
        }

        public static void Countdown(this MonoBehaviour InBehaviour, int InTime,
            Action<string> InAction, Action<bool> InOnFinished = null,
            TimeType InType = TimeType.ss, int InUniqueID = int.MaxValue)
        {
            Assert.IsNotNull(InAction);

            InBehaviour.StartCoroutine(Countdown(InTime, InAction, InOnFinished, InType, InUniqueID));
        }

        public static IEnumerator Countdown(int InTime,
            Action<string> InAction, Action<bool> InOnFinished = null,
            TimeType InType = TimeType.ss, int InUniqueID = int.MaxValue)
        {
            Assert.IsNotNull(InAction);

            if (null == wait) wait = new WaitForSeconds(1);
            if (null == timer) timer = new Dictionary<int, int>(10);
            int key = InUniqueID;

            if (InUniqueID == int.MaxValue) key = timer.Count + 1;
            if (timer.ContainsKey(key))
            {
                Debug.LogError("传入的ID不唯一，请确认不会覆盖别的倒计时！");
                yield break;
            }
            timer[key] = InTime;

            if (TimeType.mmss == InType)
            {
                int minute, second;
                while (timer[key] > 0)
                {
                    minute = InTime % 3600 / 60;
                    second = InTime % 60;
                    InAction((minute > 9 ? "" : "0") + minute + (second > 9 ? ":" : ":0") + second);
                    yield return wait;
                    --timer[key];
                    --InTime;
                }
                if (0 == InTime) InAction("00:00");
            }
            else if (TimeType.hhmmss == InType)
            {
                while (timer[key] > 0)
                {
                    InAction(InTime > 9 ? InTime.ToString() : "0" + InTime);
                    yield return wait;
                    --timer[key];
                    --InTime;
                }
                if (0 == InTime) InAction("00:00:00");
            }
            else if (TimeType.ss == InType)
            {
                int hour, minute, second;
                while (timer[key] > 0)
                {
                    hour = InTime / 3600;
                    minute = InTime % 3600 / 60;
                    second = InTime % 60;
                    InAction((hour > 9 ? "" : "0") + hour + (minute > 9 ? ":" : ":0") + minute + (second > 9 ? ":" : ":0") + second);
                    yield return wait;
                    --timer[key];
                    --InTime;
                }
                if (0 == InTime) InAction("00");
            }

            InOnFinished(0 == InTime);

            timer.Remove(key);
        }

        public static string ConvertToTime(int InTime, TimeType InType = TimeType.hhmmss)
        {
            if (TimeType.hhmmss == InType)
            {
                int hour = InTime / 3600;
                int minute = InTime % 3600 / 60;
                int second = InTime % 60;
                return (hour > 9 ? "" : "0") + hour + (minute > 9 ? ":" : ":0") + minute + (second > 9 ? ":" : ":0") + second;
            }
            else if (TimeType.mmss == InType)
            {
                int minute = InTime % 3600 / 60;
                int second = InTime % 60;
                return (minute > 9 ? "" : "0") + minute + (second > 9 ? ":" : ":0") + second;
            }

            return InTime > 9 ? InTime.ToString() : "0" + InTime;
        }

        public static SQLite3Handle GetSQLite3Handle(string InAssetsDBPath, string InLocalDBPath)
        {
            string assetDbPath = GetStreamingAssetsPath() + InAssetsDBPath;
            string localDbPath = Application.persistentDataPath + "/" + InLocalDBPath;
            bool isNeedUpdate = !(File.Exists(localDbPath)
                && GetFileMD5(assetDbPath).Equals(GetFileMD5(localDbPath)));
            if (isNeedUpdate)
            {
                switch (Application.platform)
                {
                    case RuntimePlatform.IPhonePlayer:
                    case RuntimePlatform.Android:
                        WWW www = new WWW(assetDbPath);
                        while (!www.isDone)
                        {
                        }

                        if (string.IsNullOrEmpty(www.error)) File.WriteAllBytes(localDbPath, www.bytes);
                        else Debug.LogError(www.error);

                        break;

                    default:
                        File.Copy(assetDbPath, localDbPath, true);
                        break;
                }
            }
            return new SQLite3Handle(localDbPath);
        }

        public static string GetFileMD5(string InFilePath)
        {
            try
            {
                if (null == md5)
                {
                    md5 = new MD5CryptoServiceProvider();
                }

                FileInfo file = new FileInfo(InFilePath);
                if (file.Exists)
                {
                    byte[] data;

                    using (FileStream stream = file.OpenRead())
                    {
                        int len = (int)stream.Length;
                        data = new byte[len];
                        stream.Read(data, 0, len);
                        stream.Close();
                    }

                    byte[] result = md5.ComputeHash(data);
                    StringBuilder sb = new StringBuilder(32);
                    for (int i = 0; i < result.Length; i++)
                    {
                        sb.Append(result[i].ToString("x2"));
                    }

                    return sb.ToString();
                }

                return "";
            }
            catch (FileNotFoundException e)
            {
                Debug.LogError(e.Message);
                return "";
            }
        }
        private static MD5 md5;
    }
}
