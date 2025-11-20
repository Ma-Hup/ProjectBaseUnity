using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class MusicManager : SingletonBase<MusicManager>
{
    private AudioSource bgm = null;
    private float bgmVolume = 1;

    GameObject soundObj = null;
    private List<AudioSource> soundList = new List<AudioSource>();
    private float soundVolume = 1;

    public bool soundEnabled;
    public MusicManager()
    {
        MonoController.Instance.AddUpdateListener(RecycleSound);
    }

    /// <summary>
    /// 检查已经播放完的音效 并移除
    /// </summary>
    private void RecycleSound()
    {
        for (int i = 0; i < soundList.Count; i++)
        {
            if (!soundList[i].isPlaying)
            {
                Component.Destroy(soundList[i]);
                soundList.RemoveAt(i);
            }
        }
    }

    public void PlayBGM(string path)
    {
        if (bgm == null)
        {
            GameObject BGM = new GameObject("BGM");
            bgm = BGM.AddComponent<AudioSource>();
        }
        //异步加载
        ResManager.Instance.LoadAsync<AudioClip>(path, (ac) =>
        {
            bgm.clip = ac;
            bgm.loop = true;
            bgm.volume = 1;
            bgm.Play();
        });
    }

    public void StopBGM()
    {
        if (bgm != null)
        {
            bgm.Stop();
        }
    }
    public void PauseBGM()
    {
        if (bgm != null)
        {
            bgm.Pause();
        }
    }

    public void ChangeBGMVolume(float volum)
    {
        bgmVolume = volum;
        bgm.volume = bgmVolume;
    }


    public void PlaySound(string path, bool isLoop = false, UnityAction<AudioSource> callback = null)
    {

        if (soundObj == null)
            soundObj = new GameObject("soundObject");
        if (!soundEnabled)
            return;

        ResManager.Instance.LoadAsync<AudioClip>(path, (ac) =>
        {
            AudioSource source = soundObj.AddComponent<AudioSource>();
            source.clip = ac;
            source.volume = soundVolume;
            source.loop = isLoop;
            source.Play();
            soundList.Add(source);

            //回调函数，如果有就回调
            if (callback != null)
                callback(source);
        });
    }

    /// <summary>
    /// 停止音效。TODO
    /// </summary>
    /// <param name="asource"></param>
    /// <param name="immediate"></param>
    public void StopSound(AudioSource asource, bool immediate = true)
    {
        if (soundList.Contains(asource))
        {
            if (immediate)
            {
                Component.Destroy(asource);
                soundList.Remove(asource);
                asource.Stop();
            }
            else
            {
                asource.loop = false;
            }
        }
    }


    public void ChangeSoundVolume(float v)
    {
        soundVolume = v;
        for (int i = 0; i < soundList.Count; i++)
        {
            soundList[i].volume = soundVolume;
        }
    }

    #region 部分示例
    int move_Idx = 1;

    public void MoveSound()
    {
        string path = $"sounds/move_{move_Idx}";
        PlaySound(path);
        move_Idx = move_Idx > 1 ? 1 : 2;
    }

    //为0到9
    int collect_Idx = 1;
    public void CollectSound()
    {
        string path = $"sounds/collect_{collect_Idx}";
        PlaySound(path);
        collect_Idx++;
        collect_Idx = collect_Idx > 10 ? 10 : collect_Idx;
    }
    public void ResetCollectIdx()
    {
        collect_Idx = 1;
    }

    public void DealSound()
    {
        PlaySound("sounds/deal");
    }

    public void HintSound()
    {
        PlaySound("sounds/hint");
    }

    public void MagicSound()
    {
        PlaySound("sounds/magic");
    }
    public void UndoSound()
    {
        PlaySound("sounds/undo");
    }

    public void AutoCompleteShowSound()
    {
        PlaySound("sounds/auto_complete_show");
    }

    AudioSource autoComplete;
    public void AutoCompleteSound()
    {
        PlaySound("sounds/auto_complete_show", true, (s) => autoComplete = s); 
    }
    public void AutoCompleteSoundStop()
    {
        StopSound(autoComplete);
    }

    public void PanelSound(bool isOpen, bool isWinPanel)
    {
        if (isOpen)
        {
            if (isWinPanel)
                PlaySound("sounds/win");
            else
                PlaySound("sounds/dlg_open");
        }
        else
            PlaySound("sounds/dlg_close");
    }

    public void CrowSound()
    {
        PlaySound("sounds/crown_show");
    }

    public void ErroSound()
    {
        PlaySound("sounds/error");
    }
    #endregion
}
