using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SingletonMono<T> : MonoBehaviour where T : MonoBehaviour
{
    private static T instance;
    
    /// <summary>
    /// 单例模式
    /// </summary>
    public static T Instance
    {
        get
        {
            if(instance == null)
            {
                GameObject obj = new GameObject(typeof(T).ToString());
                instance =  obj.AddComponent<T>();
#if DEBUG_VERSION
                Debug.Log($"Create obj {obj.name}");
#endif
            }
            return instance;
        }
    }
    
    virtual protected void Awake()
    {
        if (instance == null) {
            instance = GetComponent<T>();
            DontDestroyOnLoad(this);
        }
        else
            DestroyImmediate(gameObject);
    }

    //public void CreateTest()
    //{
    //    Debug.Log("SingletonMono创建成功");
    //}
}
