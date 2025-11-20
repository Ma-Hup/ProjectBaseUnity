using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PBFSampleGameStart : SingletonMono<PBFSampleGameStart>
{

    // Start is called before the first frame update
    void Start()
    {
        // 显示PBFPanel, 并将其添加到bot层
        // 默认不用添加路径"ProjecBaseSampleUI/UI/"，一般是直接加载Resources/UI下的预制体
        UIManager.Instance.ShowPanel<PBFPanel>("PBFPanel", UIManager.UIM_layer.Bottom, null, "ProjecBaseSampleUI/");
    }
    /// <summary>
    /// 轮流展示PBWinPanelTest面板
    /// </summary>
    public void StartUIFalls()
    {
        Debug.Log("轮流展示界面");
        StartCoroutine(UIFalls());
    }
    IEnumerator UIFalls()
    {
        yield return new WaitUntil( ()=>UIManager.Instance.GetPanel<PBFPanel>("PBFPanel") == null );
        UIManager.Instance.ShowPanel<PBWinPanelTest1>("PBWinPanelTest1");

        yield return new WaitUntil( ()=>UIManager.Instance.GetPanel<PBWinPanelTest1>("PBWinPanelTest1") == null );
        UIManager.Instance.ShowPanel<PBWinPanelTest2>("PBWinPanelTest2");

        yield return new WaitUntil( ()=>UIManager.Instance.GetPanel<PBWinPanelTest2>("PBWinPanelTest2") == null );
        UIManager.Instance.ShowPanel<PBFPanel>("PBFPanel", UIManager.UIM_layer.Bottom, null, "ProjecBaseSampleUI/");
    }
}
