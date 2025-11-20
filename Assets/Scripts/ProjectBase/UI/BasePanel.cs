using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using DG.Tweening;


public class BasePanel : MonoBehaviour
{
    private Dictionary<string, List<UIBehaviour>> ctrlDictionary = new Dictionary<string, List<UIBehaviour>>();

    /// <summary>
    /// 正在播放动画
    /// </summary>
    public bool isTweening;

    /// <summary>
    /// 在显示该页面的时候暂停计时器
    /// </summary>
    public bool pauseOnShow;

    /// <summary>
    /// 不会被返回键响应，也不会进入面板队列
    /// </summary>
    public bool withoutEscBtn;

    protected Sequence nowSeq;

    public CanvasGroup cG;

    protected virtual void Awake()
    {
        GetAllControllor();
    }

    protected virtual void Start()
    {

    }

    protected virtual void OnDestroy()
    {
        
    }

    protected virtual void Update()
    {

    }


    private void GetAllControllor()
    {
        //获取panel上所有常见组件（需要可以自己添加）
        FindeChildrenControl<Button>();
        FindeChildrenControl<Image>();
        FindeChildrenControl<Text>();
        FindeChildrenControl<Toggle>();
        FindeChildrenControl<Slider>();
        FindeChildrenControl<Scrollbar>();
        FindeChildrenControl<InputField>();
    }

    public virtual void ShowMe()
    {

    }

    public virtual void HideMe()
    {

    }
    /// <summary>
    /// 添加UI动画到队列
    /// </summary>
    /// <param name="anim">动画</param>
    /// <param name="interactable">在动画过程中UI是否可互动</param>
    protected void AddUiAnimation(Sequence anim,bool interactable = false)
    {
        nowSeq = anim;
        isTweening = true;
        //anim.onPlay+= ()=> 
        anim.onComplete += () =>
        {
            nowSeq = null;
            isTweening = false;
        };

        if (!interactable)
        {
            anim.onPlay += () => cG.interactable = false;
            anim.onComplete += () => cG.interactable = true;
        }

        anim.onKill += () =>
        {
            nowSeq = null;
            isTweening = false;
        };

        UIManager.Instance.AddUiAinimation(anim);
    }

    protected void RemoveUiAnimation(Sequence anim)
    {
        UIManager.Instance.RemoveUiAnimation(anim);
    }

    protected virtual void OnBtnClick(string btnName)
    {

    }
    protected virtual void OnToggleValueChange(string toggleName, bool value)
    {

    }

    public virtual void OnEscape()
    {
        HideMe();
    }


    /// <summary>
    /// 获取目标对象名的控件下的目标类型组件
    /// </summary>
    /// <typeparam name="T">组件类型</typeparam>
    /// <param name="objname">目标控件名</param>
    /// <returns></returns>
    protected T GetControler<T>(string objname) where T : UIBehaviour
    {
        if (ctrlDictionary.ContainsKey(objname))
        {

            for (int i = 0; i < ctrlDictionary[objname].Count; i++)
            {
                if (ctrlDictionary[objname][i] is T)
                    return ctrlDictionary[objname][i] as T;
            }
        }

        return null;
    }


    /// <summary>
    /// 找到面板下所有子控件下的所有该类型的组件，并根据组件的对象名分类存入字典,并同时添加触发函数
    /// </summary>
    /// <typeparam name="T"></typeparam>
    private void FindeChildrenControl<T>() where T : UIBehaviour
    {
        T[] controls = this.GetComponentsInChildren<T>(true);

        for (int i = 0; i < controls.Length; i++)
        {
            string objname = controls[i].gameObject.name;
            if (ctrlDictionary.ContainsKey(objname))
                ctrlDictionary[objname].Add(controls[i]);
            else
                ctrlDictionary.Add(objname, new List<UIBehaviour>() { controls[i] });

            if (controls[i] is Button)
            {
                (controls[i] as Button).onClick.AddListener(() =>
                {
                    OnBtnClick(objname);
                });
            }
            else if (controls[i] is Toggle)
            {
                (controls[i] as Toggle).onValueChanged.AddListener((value) =>
                {
                    OnToggleValueChange(objname, value);
                });
            }

        }

    }
    /// <summary>
    /// 向上查找父节点的组件
    /// </summary>
    /// <typeparam name="T">组件类型</typeparam>
    /// <param name="go">开始查找的物体</param>
    /// <returns></returns>
    protected T FindInParents<T>(GameObject go) where T : Component
    {
        if (go == null) return null;
        var comp = go.GetComponent<T>();

        if (comp != null)
            return comp;

        var t = go.transform.parent;
        while (t != null && comp == null) {
            comp = t.gameObject.GetComponent<T>();
            t = t.parent;
        }
        return comp;
    }
}
