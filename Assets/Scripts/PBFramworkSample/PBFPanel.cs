using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public class PBFPanel : BasePanel
{
    private static int eventTestCount = 0;

    protected override void Start()
    {
        base.Start();
        EventsManager.Instance.AddEventsListener<int>(Constants.Event.EVENTS_TEST, EventTest);// 事件添加和移除
    }
    private void OnDestroy()
    {
        EventsManager.Instance.RemoveListener<int>(Constants.Event.EVENTS_TEST, EventTest);// 事件添加和移除
    }

    private void EventTest(int i)
    {
        Debug.LogWarning($"EventTest: {i}");
    }

    public override void ShowMe()
    {
        Debug.Log("ShowMe!!!");
        // cG.DOFade(1, 0.2f).OnComplete(() =>
        // {
        //     isTweening = false;
        // });
    }
    public override void HideMe()
    {
        Debug.Log("HideMe!!!");
        // cG.DOFade(0, 0.2f).OnComplete(() =>
        // {
        //     isTweening = false;
        //     gameObject.SetActive(false);
        // });
    }
    /// <summary>
    /// 按钮点击事件
    /// </summary>
    /// <param name="btnName"></param>
    protected override void OnBtnClick(string btnName)
    {
        base.OnBtnClick(btnName);
        Debug.Log("OnBtnClick!!!");
        switch (btnName)
        {
            case "TextShowBtn":
                TextShowBtnClick();
                break;
            case "EventTestBtn":
                EventsManager.Instance.EventTrigger<int>("EventTest", eventTestCount++);
                break;
            case "CloseBtn":
                UIManager.Instance.HidePanel("PBFPanel");
                PBFSampleGameStart.Instance.StartUIFalls();
                break;
        }
    }

    private void TextShowBtnClick()
    {
        Debug.Log("TextShowBtn Click!!!");
        GameObject textShow = GetControler<Text>("ShowText").gameObject; 
        textShow.SetActive(!textShow.activeSelf);
    }
}
