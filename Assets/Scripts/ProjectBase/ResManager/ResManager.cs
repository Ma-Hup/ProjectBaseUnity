using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking;

public class ResManager : SingletonBase<ResManager>
{
    /// <summary>
    /// 同步加载Resources下的资源
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="path">在Rescources文件夹下的路径</param>
    /// <param name="parent">实例化后设置的父对象</param>
    /// <returns></returns>
    public T Load<T>(string path, Transform parent = null) where T : Object
    {
        T res = Resources.Load<T>(path);
        if (res is GameObject)
            return GameObject.Instantiate(res, parent);
        else
            return res;
    }
    /// <summary>
    /// 异步加载Resources下的资源
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="path">在Rescources文件夹下的路径</param>
    /// <param name="callback">加载完成后的回调函数</param>
    /// <param name="parent">实例化后设置的父对象</param>
    public void LoadAsync<T>(string path, UnityAction<T> callback, Transform parent = null) where T : Object
    {
        MonoController.Instance.StartCoroutine(RealLoadAsync<T>(path, callback, parent));
    }

    private IEnumerator RealLoadAsync<T>(string path, UnityAction<T> callback, Transform parent) where T : Object
    {
        ResourceRequest r = Resources.LoadAsync<T>(path);
        yield return r;
        if (r.asset is GameObject)
            callback(GameObject.Instantiate(r.asset, parent) as T);
        else
            callback(r.asset as T);
    }
    /// <summary>
    /// 从StreamingAssets文件夹下加载文件
    /// </summary>
    /// <param name="path"></param>
    /// <returns></returns>
    public string LoadFromStreamingAssets(string path) {
        //path = Path.Combine(Application.streamingAssetsPath, path);
        //MonoController.Instance.StartCoroutine(ILoadFromStreamingAssets(path, onComplete));

        string fileContent = "";

        // 使用StreamReader来读取txt文件内容
        try {
            using (StreamReader sr = new StreamReader(path)) {
                fileContent = sr.ReadToEnd();
            }
        } catch (System.Exception e) {
            Debug.LogError("Error reading the txt file: " + e.Message);
        }

        return fileContent;
    }

    IEnumerator ILoadFromStreamingAssets(string path, System.Action<string> onComplete) {
#if UNITY_IOS && !UNITY_EDITOR
        path = "file://" + path;
#endif
        UnityWebRequest request = UnityWebRequest.Get(path);

        yield return request.SendWebRequest();

        if (request.error == null) {
            string text = System.Text.Encoding.UTF8.GetString(request.downloadHandler.data);
            onComplete?.Invoke(text);
        }
    }

}
