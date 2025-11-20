using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoolData
{
    public GameObject fatherObj;
    public List<GameObject> poolList;

    public PoolData(string name, GameObject poolRoot)
    {
        fatherObj = new GameObject(name);
        fatherObj.transform.SetParent(poolRoot.transform);
        poolList = new List<GameObject>();
    }

    public void PushPoolObj(GameObject obj)
    {
        obj.SetActive(false);
        poolList.Add(obj);
        obj.transform.SetParent(fatherObj.transform);

    }

    public GameObject GetPoolObj(Transform parent = null)
    {
        GameObject obj = null;
        obj = poolList[0];
        poolList.RemoveAt(0);
        obj.SetActive(true);
        obj.transform.SetParent(parent);
        //PushPollObj的时候旋转了屏幕，会导致obj的scale发生变化，GetPoolObj的时候重置为1
        obj.transform.localScale = Vector3.one;
        return obj;
    }



}
