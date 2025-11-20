using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : SingletonBase<InputManager> 
{
    private bool allowCheck;
    public InputManager()
    {
        MonoController.Instance.AddUpdateListener(InputUpdate);
    }

    public void SwitchInputCheck(bool ifcheck)
    {
        allowCheck = ifcheck;
    }

    void InputUpdate()
    {
        if (!allowCheck)
            return;

        //检测鼠标左键三种状态
        if (Input.GetMouseButtonDown(0))
            EventsManager.Instance.EventTrigger("MouseDown", 0);
        if (Input.GetMouseButtonUp(0))
            EventsManager.Instance.EventTrigger("MouseUp", 0);
        if (Input.GetMouseButton(0))
            EventsManager.Instance.EventTrigger("MousePress", 0);
        //检测鼠标右键的三种状态
        if (Input.GetMouseButtonDown(1))
            EventsManager.Instance.EventTrigger("MouseDown", 1);
        if (Input.GetMouseButtonUp(1))
            EventsManager.Instance.EventTrigger("MouseUp", 1);
        if (Input.GetMouseButton(1))
            EventsManager.Instance.EventTrigger("MousePress", 1);

        if (Input.GetKeyDown(KeyCode.Escape))
            EventsManager.Instance.EventTrigger("OnEscapeDown",0);
    }
}
