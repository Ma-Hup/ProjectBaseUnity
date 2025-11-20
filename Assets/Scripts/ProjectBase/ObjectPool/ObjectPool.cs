using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ObjectPool : SingletonBase<ObjectPool>
{
    private Dictionary<string, PoolData> pool = new Dictionary<string, PoolData>();
    private GameObject objPool;
    public ObjectPool()
    {
        if (!objPool)
            objPool = new GameObject("GameObject Pool");
    }
    /// <summary>
    /// 从缓存池获取对象
    /// </summary>
    /// <param name="path">对象资源路径</param>
    /// <param name="callback">回调函数</param>
    public void GetObj(string path,UnityAction<GameObject> callback,Transform parent,bool ifAsync = true)
    {
        string objName = path.Substring(path.LastIndexOf('/') + 1);

        if (pool.ContainsKey(objName) && pool[objName].poolList.Count > 0)
        {
            
            callback(pool[objName].GetPoolObj(parent));
        }
        else
        {
            if (ifAsync)
            {
                ResManager.Instance.LoadAsync<GameObject>(path, (loadobj) => {

                    loadobj.name = objName;
                    callback(loadobj);
                }, parent);
            }
            else
            {
                GameObject loadobj = ResManager.Instance.Load<GameObject>(path, parent);
                loadobj.name = objName;
                callback(loadobj);
            }
        }
    }
    /// <summary>
    /// 用完的物品存入缓存池
    /// </summary>
    /// <param name="objName">对象的名字</param>
    /// <param name="obj">对象</param>
    public void TStoreObj(string objName, GameObject obj)
    {
        if (!pool.ContainsKey(objName))
        {
            pool.Add(objName,new PoolData(objName + " obj",objPool));
        }
        pool[objName].PushPoolObj(obj);

    }

    public void Clear()
    {
        pool.Clear();

    }

}
