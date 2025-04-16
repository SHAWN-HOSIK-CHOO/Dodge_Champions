using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class DataDictionary
{
    public object Key;
    public object Value;
}

[Serializable]
public class JsonDataArray
{
    public List<DataDictionary> data;
}

public static class DictionaryJsonUtility
{
    /// <summary>
    /// Dictionary�� Json���� �Ľ��ϱ� (���÷��� ���)
    /// </summary>
    /// <param name="jsonDicData"></param>
    /// <returns></returns>
    public static string ToJson(Dictionary<object, object> jsonDicData)
    {
        List<DataDictionary> dataList = new List<DataDictionary>();

        foreach (var keyValue in jsonDicData)
        {
            DataDictionary dictionaryData = new DataDictionary();
            dictionaryData.Key = keyValue.Key;

            // ���� Dictionary�� ��� ��������� ����ȭ
            if (IsDictionary(keyValue.Value))
            {
                var nestedJson = ToJson((Dictionary<object, object>)keyValue.Value);
                dictionaryData.Value = nestedJson;
            }
            else
            {
                dictionaryData.Value = keyValue.Value;
            }

            dataList.Add(dictionaryData);
        }

        JsonDataArray arrayJson = new JsonDataArray();
        arrayJson.data = dataList;

        return JsonUtility.ToJson(arrayJson, true);
    }

    /// <summary>
    /// Json Data�� �ٽ� Dictionary�� �Ľ��ϱ� (���÷��� ���)
    /// </summary>
    /// <param name="jsonData"></param>
    /// <returns></returns>
    public static Dictionary<object, object> FromJson(string jsonData)
    {
        JsonDataArray arrayJson = JsonUtility.FromJson<JsonDataArray>(jsonData);

        Dictionary<object, object> returnDictionary = new Dictionary<object, object>();

        foreach (DataDictionary dictionaryData in arrayJson.data)
        {
            // ���� JSON ���ڿ��� ����ȭ�� Dictionary�� ���, ��������� ������ȭ
            if (dictionaryData.Value is string jsonString && jsonString.StartsWith("{"))
            {
                returnDictionary[dictionaryData.Key] = FromJson(jsonString);
            }
            else
            {
                returnDictionary[dictionaryData.Key] = dictionaryData.Value;
            }
        }

        return returnDictionary;
    }

    /// <summary>
    /// ���� Dictionary���� Ȯ���ϴ� ���� �޼���
    /// </summary>
    private static bool IsDictionary(object obj)
    {
        return obj != null && obj.GetType().IsGenericType &&
               obj.GetType().GetGenericTypeDefinition() == typeof(Dictionary<,>);
    }
}
