using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PBWinPanelTest1 : BasePanel
{
    protected override void OnBtnClick(string btnName)
    {
        base.OnBtnClick(btnName);
        switch(btnName)
        {
            case "NextBtn":
                UIManager.Instance.HidePanel(gameObject.name);
                break;
        }
    }
}
