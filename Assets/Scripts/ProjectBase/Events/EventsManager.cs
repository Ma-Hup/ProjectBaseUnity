using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// 空接口，用于实现避免，订阅事件执行时产生的装箱拆箱
/// </summary>
public interface IEventInfo
{

}

public class EventInfo<T>:IEventInfo
{
    public UnityAction<T> actions;
    public EventInfo(UnityAction<T> action)
    {
        actions += action;
    }

}

public class EventsManager : SingletonBase<EventsManager>
{
    //存一个接口，因为里式替换原则使用时可以转化成子类，使用子类的字段，达到了可以用使用泛型存储委托的效果。
    private Dictionary<string, IEventInfo> eventsDictionary = new Dictionary<string, IEventInfo>();

    /// <summary>
    /// 添加事件接受者
    /// </summary>
    /// <param name="name">事件名</param>
    /// <param name="action">需要执行的事件委托</param>
    // public void AddEventsListener<T>(string name, UnityAction<T> action)
    // {
    //     if (eventsDictionary.ContainsKey(name))
    //     {
    //         (eventsDictionary[name] as EventInfo<T>).actions += action;
    //     }
    //     else
    //     {
    //         eventsDictionary.Add(name, new EventInfo<T>(action)); 
    //     }
    // }
    public void AddEventsListener<T>(string name, UnityAction<T> action)
    {
        if (eventsDictionary.ContainsKey(name))
        {
            var evt = eventsDictionary[name] as EventInfo<T>;
            if (evt != null)
            {
                evt.actions += action;
            }
            else
            {
                // 如果已有同名事件但泛型不匹配，替换为新的（更安全的行为）
                eventsDictionary[name] = new EventInfo<T>(action);
            }
        }
        else
        {
            eventsDictionary.Add(name, new EventInfo<T>(action)); 
        }
    }
    /// <summary>
    /// 添加事件触发
    /// </summary>
    /// <param name="name">事件名</param>
    /// <param name="info">相关信息</param>
    public void EventTrigger<T>(string name,T info)
    {
        if (eventsDictionary.ContainsKey(name))
        {
            var evt = eventsDictionary[name] as EventInfo<T>;
            if (eventsDictionary[name] != null && evt.actions != null)
            {
                (eventsDictionary[name] as EventInfo<T>).actions.Invoke(info);
            }
        }
    }
    /// <summary>
    /// 移除事件接受者
    /// </summary>
    /// <param name="name">事件名</param>
    /// <param name="action">被要求执行的事件委托</param>
    // public void RemoveListener<T>(string name,UnityAction<T> action)
    // {
    //     if (eventsDictionary.ContainsKey(name))
    //     {
    //         if (eventsDictionary[name] != null)
    //         {
    //             (eventsDictionary[name] as EventInfo<T>).actions -= action;
    //         }
    //     }
    // }
    public void RemoveListener<T>(string name,UnityAction<T> action)
    {
        if (eventsDictionary.ContainsKey(name))
        {
            var evt = eventsDictionary[name] as EventInfo<T>;
            if (evt != null)
            {
                evt.actions -= action;
                // 如果没有订阅者了，清除字典中的条目，避免留下 actions 为 null 的事件
                if (evt.actions == null)
                {
                    eventsDictionary.Remove(name);
                }
            }
            else
            {
                // 如果类型不匹配，直接移除该 key（可选，视需求）
                eventsDictionary.Remove(name);
            }
        }
    }
    /// <summary>
    /// 清空事件
    /// </summary>
    public void EventClear()
    {
        eventsDictionary.Clear();
    }
}
