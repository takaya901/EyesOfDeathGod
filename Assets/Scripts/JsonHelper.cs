using System;
using System.Collections.Generic;
using UnityEngine;

//ルートがArrayのJSONをパースしてListを返す
//http://blog.cluster.mu/2016/08/jsonutility_instead_of_litjson/
public class JsonHelper
{
    public static List<T> ListFromJson<T>(string json)
    {
        var newJson = "{\"list\": " + json + "}";
        var wrapper = JsonUtility.FromJson<Wrapper<T>>(newJson);
        return wrapper.list;
    }

    [Serializable]
    class Wrapper<T>
    {
        public List<T> list;
    }
}