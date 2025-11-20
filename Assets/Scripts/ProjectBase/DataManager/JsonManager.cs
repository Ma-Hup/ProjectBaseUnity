using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using JsonUtilityExt;
using System.IO;
using System;

public class JsonManager : SingletonBase<JsonManager>
{
    /// <summary>
    /// 使用JsonUtility存储数据为json形式
    /// </summary>
    /// <param name="data">需要存储的对象</param>
    /// <param name="fileName">文件名</param>
    public void SaveJson(object data, string fileName)
    {
        //获得存储路径名
        string path = Application.persistentDataPath + $"/{fileName}.json";

        string jsonStr = "";

        Type dataType = data.GetType();

        //使用JsonUtility将类转换为字符串(注意 不能直接对List和Dictionary进行转换)
        jsonStr = JsonUtility.ToJson(data, true);

        //存储为json文件
        File.WriteAllText(path, jsonStr);
    }



    public T LoadJson<T>(string fileName) where T : new()
    {
        string path;
        //获取文件路径
        // path = Application.streamingAssetsPath + $"/{fileName}.json";
        string jsonStr;
        path = Application.persistentDataPath + $"/{fileName}.json";
        if (!File.Exists(path))
            return new T();
        
        jsonStr = File.ReadAllText(path);

        return JsonUtility.FromJson<T>(jsonStr); 
    }

    public void DeletJson(string fileName)
    {
        string path;
        path = Application.persistentDataPath + $"/{fileName}.json";

        if (File.Exists(path))
            File.Delete(path);
    }


}
