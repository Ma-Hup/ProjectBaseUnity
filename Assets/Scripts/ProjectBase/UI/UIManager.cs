using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using DG.Tweening;

public class UIManager : SingletonBase<UIManager>
{
    //面板字典，用于获取和回收面板
    public Dictionary<string, BasePanel> pnlDictionary = new Dictionary<string, BasePanel>();
    //面板列表，记录当前面板顺序，用于ESC关闭面板
    private List<BasePanel> pnlList = new List<BasePanel>();

    //准备区动画
    private List<Sequence> preparateSeqList = new List<Sequence>();
    //正在播放的UI动画
    private Sequence playingSeq;

    private RectTransform canvasRect;
    private CanvasScaler canvasScaler;
    //MainCavas层级，MainCanvas结构可以看MainCanvas预制体
    private Transform bot;  //sort order = 0
    private Transform mid;  //sort order = 20
    private Transform top;  //sort order = 40
    private Transform system;   //sort order = 60



    //屏幕长宽
    static private Vector2 _screenSize;
    static public Vector2 ScreenSzie
    {
        get { return _screenSize; }
    }

    static public bool isPortrait
    {
        get => Screen.width < Screen.height;
    }

    /// <summary>
    /// 获取屏幕 长/宽
    /// </summary>
    static public float AspectRatio
    {
        get { return _screenSize.y / _screenSize.x; }
    }
    /// <summary>
    /// 获取长宽像素缩放比例
    /// </summary>
    static public float GetWidthScale
    {
        get
        {
            float s = AspectRatio / (1920f / 1080f);
            return s < 1 ? 1 : s;
        }
    }

    static public float GetHeightScale
    {
        get
        {
            float s = (1920f / 1080f) / AspectRatio;
            return s < 1 ? 1 : s;
        }
    }


    public enum UIM_layer
    {
        Top,
        Mid,
        Bottom,
        System
    }

    public UIManager()
    {
        //查找场景中的MainCanvas，没有的话在Resources/UI下查找MainCanvas
        GameObject canvasObj = GameObject.FindWithTag("MainCanvas");
        GameObject eventSystemObj = GameObject.FindWithTag("MainEventSystem");

        //通过Tag查找，找不到则自动加载Canvas和EventSystem
        if (canvasObj == null)
            canvasObj = ResManager.Instance.Load<GameObject>("UI/MainCanvas");
        if (eventSystemObj == null)
            eventSystemObj = ResManager.Instance.Load<GameObject>("UI/EventSystem");

        GameObject.DontDestroyOnLoad(canvasObj);
        GameObject.DontDestroyOnLoad(eventSystemObj);
        canvasRect = canvasObj.transform as RectTransform;
        canvasScaler = canvasObj.GetComponent<CanvasScaler>();

        bot = canvasRect.Find("bot");
        mid = canvasRect.Find("mid");
        top = canvasRect.Find("top");
        system = canvasRect.Find("system");

        //设置画布渲染模式
        Canvas canvas = canvasObj.GetComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceCamera;
        canvas.worldCamera = Camera.main;

        if (isPortrait)
            _screenSize = new Vector2(Screen.width, Screen.height);
        else
            _screenSize = new Vector2(Screen.height, Screen.width);

        Debug.Log($"ScreenSize = {ScreenSzie}");
        Debug.Log($"AspectRatio = {AspectRatio}");

        //返回键响应
        EventsManager.Instance.AddEventsListener<int>("OnEscapeDown", (s) =>
        {
            BasePanel pnl = pnlList.Count > 0 ? pnlList[pnlList.Count - 1] : null;

            if (pnl != null && !pnl.isTweening)
                pnl.OnEscape();
        });
        //动画列队顺序播放
        MonoController.Instance.AddUpdateListener(() =>
        {
            if (playingSeq == null && preparateSeqList.Count > 0)
            {
                playingSeq = preparateSeqList[0];
                preparateSeqList.RemoveAt(0);
                playingSeq.Play();
            }
        });
    }

    private void AddToPnlList(BasePanel pnl)
    {
        if (!pnl.withoutEscBtn)
            pnlList.Add(pnl);
    }


    public Transform GetLayerFather(UIM_layer layer)
    {
        Transform obj;
        switch (layer)
        {
            case UIM_layer.Top:
                obj = top;
                break;
            case UIM_layer.Mid:
                obj = mid;
                break;
            case UIM_layer.Bottom:
                obj = bot;
                break;
            case UIM_layer.System:
                obj = system;
                break;
            default:
                obj = null;
                break;
        }
        return obj;
    }


    /// <summary>
    /// 显示面板（创建面板）
    /// </summary>
    /// <typeparam name="T">面板类型</typeparam>
    /// <param name="panelName">面板名称</param>
    /// <param name="layer">面板所在层级</param>
    /// <param name="callback">加载完成时触发的 事件</param>
    public void ShowPanel<T>(string panelName, UIM_layer layer = UIM_layer.Mid, UnityAction<T> callback = null, string panelPath = "UI/") where T : BasePanel
    {
        //字典里查找是否有，有的话直接调用
        if (pnlDictionary.ContainsKey(panelName))/*  */
        {
            pnlDictionary[panelName].ShowMe();
            if (callback != null)
                callback(pnlDictionary[panelName] as T);
            return;
        }

        //当前面板中没有，则通过ResManager在Resources文件夹中加载
        //GameObject obj = ResManager.Instance.Load<GameObject>("UI/" + panelName);
        GameObject obj = ResManager.Instance.Load<GameObject>(panelPath + panelName);
        Transform p = bot;
        switch (layer)
        {
            case UIM_layer.Top:
                p = top;
                break;
            case UIM_layer.Mid:
                p = mid;
                break;
            case UIM_layer.Bottom:
                p = bot;
                break;
            case UIM_layer.System:
                p = system;
                break;
            default:
                break;
        }

        //Debug.Log($"OBJ anchor {obj.transform.rectransform.}");
        //设置所在层级和位置
        obj.transform.SetParent(p);

        // 设置 ActorPress
        obj.GetComponent<RectTransform>().anchorMin = new Vector2(0, 0);
        obj.GetComponent<RectTransform>().anchorMax = new Vector2(1, 1);
        // (obj.transform as RectTransform).offsetMax = new Vector2(1, 1);
        // (obj.transform as RectTransform).offsetMin = Vector2.zero;

        obj.transform.localPosition = Vector3.zero;
        obj.transform.localScale = Vector3.one;
        obj.GetComponent<RectTransform>().sizeDelta = new Vector2(0, 0);

        obj.name = panelName;
        T panelComp = obj.gameObject.GetComponent<T>();

        pnlDictionary.Add(panelName, panelComp);
        //pnlList.Add(panelComp);
        AddToPnlList(panelComp);
        if (callback != null)
            callback(panelComp);

        //调用面板showMe
        panelComp.ShowMe();

        PanelChangeHandle();
    }




    /// <summary>
    /// 添加场景里已有的面板到字典
    /// </summary>
    /// <typeparam name="T">面板脚本类</typeparam>
    /// <param name="panelComp">面板脚本</param>
    /// <param name="layer">需要添加的层级</param>
    public void AddPanel<T>(T panelComp, UIM_layer layer = UIM_layer.Mid) where T : BasePanel
    {
        //传入了null面板
        if (panelComp == null)
        {
            Debug.Log("Add panel not found");
            return;
        }

        //当前面板已被添加
        string panelName = panelComp.name;
        if (GetPanel<T>(panelName) != null)
        {
            Debug.Log("Add panel already exist");
            return;
        }

        GameObject obj = panelComp.gameObject;

        //设置所在位置
        obj.transform.localPosition = Vector3.zero;
        obj.transform.localScale = Vector3.one;

        (obj.transform as RectTransform).offsetMax = Vector2.zero;
        (obj.transform as RectTransform).offsetMin = Vector2.zero;

        //T panelComp = obj.gameObject.GetComponent<T>();
        pnlDictionary.Add(panelName, panelComp);
        AddToPnlList(panelComp);
        //pnlList.Add(panelComp);
    }


    /// <summary>
    /// 隐藏面板，销毁
    /// </summary>
    /// <param name="panelName">面板名</param>
    public void HidePanel(string panelName)
    {
        if (pnlDictionary.ContainsKey(panelName))
        {
            pnlList.Remove(pnlDictionary[panelName]);
            GameObject.Destroy(pnlDictionary[panelName].gameObject);
            pnlDictionary.Remove(panelName);
        }

        PanelChangeHandle();
    }
    
    /// <summary>
    /// 面板发生更变的时候的一些处理
    /// </summary>
    public void PanelChangeHandle()
    {
        // if (pnlList.Count == 1 && pnlList[0].name == "GamingUI")
        // {
        //     AdCtrl.Instance.SetBanner(true);
        //     CardAnimMgr.Instance.ReStartAutoTip();
        // }
        // else
        //     AdCtrl.Instance.SetBanner(false);
    }


    /// <summary>
    /// 获取面板，可以在协程中瀑布流展示界面：yield return new WaitUntil(() => GetPanel == null);
    /// </summary>
    /// <typeparam name="T">面板挂载的脚本类型</typeparam>
    /// <param name="panelName">面板的名称</param>
    /// <returns>返回面板挂载的继承于BasePanel的脚本</returns>
    public T GetPanel<T>(string panelName) where T : BasePanel
    {
        if (pnlDictionary.ContainsKey(panelName))
            return pnlDictionary[panelName] as T;
        else
            return null;
    }

    //动画相关

    //添加一个动画到等待队列
    public void AddUiAinimation(Sequence anim)
    {
        anim.onComplete += () => playingSeq = null;
        if (playingSeq == null)
        {
            playingSeq = anim;
        }
        else
        {
            anim.Pause();
            preparateSeqList.Add(anim);
        }
    }
    //从等待队列移除目标动画
    public void RemoveUiAnimation(Sequence anim)
    {
        preparateSeqList.Remove(anim);
    }

    //跳过当前正在播放动画
    public void SkipUiAnim(bool ifComplete)
    {
        playingSeq.Kill(ifComplete);
        playingSeq = null;
    }



    /// <summary>
    /// 给控件添加自定义监听事件
    /// </summary>
    /// <param name="control">目标控件</param>
    /// <param name="type">监听事件类型</param>
    /// <param name="callBack">事件响应函数</param>
    public static void AddCustomEventListener(UIBehaviour control, EventTriggerType type, UnityAction<BaseEventData> callBack)
    {
        EventTrigger trigger = control.GetComponent<EventTrigger>();
        if (!trigger)
            trigger = control.gameObject.AddComponent<EventTrigger>();

        EventTrigger.Entry entry = new EventTrigger.Entry();
        entry.eventID = type;
        entry.callback.AddListener(callBack);

        trigger.triggers.Add(entry);
    }

    /// <summary>
    /// 清除控件上的所有监听事件
    /// </summary>
    /// <param name="control">目标控件</param>
    public static void ClearCustomEvent(UIBehaviour control)
    {
        EventTrigger trigger = control.GetComponent<EventTrigger>();
        if (trigger != null)
            trigger.triggers.Clear();
    }

}
