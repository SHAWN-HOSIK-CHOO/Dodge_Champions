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
    /// Dictionary를 Json으로 파싱하기 (리플렉션 사용)
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

            // 값이 Dictionary일 경우 재귀적으로 직렬화
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
    /// Json Data를 다시 Dictionary로 파싱하기 (리플렉션 사용)
    /// </summary>
    /// <param name="jsonData"></param>
    /// <returns></returns>
    public static Dictionary<object, object> FromJson(string jsonData)
    {
        JsonDataArray arrayJson = JsonUtility.FromJson<JsonDataArray>(jsonData);

        Dictionary<object, object> returnDictionary = new Dictionary<object, object>();

        foreach (DataDictionary dictionaryData in arrayJson.data)
        {
            // 값이 JSON 문자열로 직렬화된 Dictionary일 경우, 재귀적으로 역직렬화
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
    /// 값이 Dictionary인지 확인하는 헬퍼 메서드
    /// </summary>
    private static bool IsDictionary(object obj)
    {
        return obj != null && obj.GetType().IsGenericType &&
               obj.GetType().GetGenericTypeDefinition() == typeof(Dictionary<,>);
    }
}
