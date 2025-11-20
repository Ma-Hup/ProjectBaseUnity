using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;
using UnityEngine.Events;

public class MonoController : SingletonMono<MonoController>
{
    private event UnityAction updateEvent;

    void Start()
    {
        DontDestroyOnLoad(this.gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        if(updateEvent != null)
        {
            updateEvent();
        }
    }

    public void AddUpdateListener(UnityAction fun)
    {
        updateEvent += fun;
    }

    public void RemoveUpdateListener(UnityAction fun)
    {
        updateEvent -= fun;
    }

    //public Coroutine AddStartCoroutine(IEnumerator routine)
    //{
    //    return StartCoroutine(routine);
    //}
    //public Coroutine AddStartCoroutine(string methodName)
    //{
    //    return StartCoroutine(methodName);
    //}
    //public Coroutine AddStartCoroutine(string methodName, [DefaultValue("null")] object value)
    //{
    //    return StartCoroutine(methodName, value);
    //}

    //public void AddStopAllCoroutines()
    //{
    //    StopAllCoroutines();
    //}
}
