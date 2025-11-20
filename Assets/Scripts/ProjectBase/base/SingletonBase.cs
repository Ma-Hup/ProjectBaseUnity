using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SingletonBase<T> where T:new()
{

    private static T instance;

    //private SingletonBase() { }

    public static T Instance
    {
        get
        {
            if (instance == null)
                instance = new T();
            return instance;
        }
    }

    public void CreateTest()
    {
        Debug.Log("SingletonBase创建成功");
    }



}



