using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.Events;

public class TweenPlayer : MonoBehaviour
{
    public enum TweenType
    {
        eFadeTo,
        eScaleTo,
        eScaleFrom,
        eMoveTo,
        eMoveFrom,
        eRotateTo,
        eRotateFrom,
        eShakePosition,
        eShakeScale,
        eShakeRotation,
        ePunchPosition,
        ePunchScale,
        ePunchRotation,
        eValueTo,
    }

    public TweenType Type;
    public iTween.EaseType EaseType = iTween.EaseType.linear;
    public iTween.LoopType LoopType = iTween.LoopType.none;
    public float Time = 1;
    public float Delay = 0;
    public bool IsLocal = true;
    public Vector3 Amount;
    public float Value;
    public float Value1;
    public UnityEvent OnComplete;
    public bool AutoPlay = false;
    public bool StopOtherTweenWhenPlay = true;//当播时停止其他tween动画
    private Hashtable m_Hashtable = new Hashtable();
    private Action m_OnCompleteAction;
    public bool IsIgnoreCallBack = false;//是否忽略回调

    [HideInInspector]//回播前记录原始值
    public Vector3 SrcAmount;
    [HideInInspector]
    public float SrcValue;



    void Start()
    {
        if (AutoPlay)
            Play();

    }

    public void Play()
    {
        Play(null);
    }

    public void Play(Action oncomplete, bool Reserve = false, Action onupdate = null)
    {
        if (StopOtherTweenWhenPlay)
            iTween.Stop(gameObject);
        if (IsIgnoreCallBack)
            m_OnCompleteAction = null;
        else
            m_OnCompleteAction = oncomplete;
        m_Hashtable.Clear();
        //m_Hashtable.Add("time", Time + Delay);
        m_Hashtable.Add("time", Time);
        m_Hashtable.Add("delay", Delay > 0 ? Delay : Delay + .01f);
        m_Hashtable.Add("easetype", EaseType);
        m_Hashtable.Add("looptype", LoopType);
        m_Hashtable.Add("islocal", IsLocal);
        m_Hashtable.Add("oncomplete", (Action)HandleOnComplete);
        //m_Hashtable.Add("ignoretimescale", true);
        if (onupdate != null)
            m_Hashtable.Add("onupdate", onupdate);
        switch (Type)
        {
            case TweenType.eFadeTo:
                m_Hashtable.Add("alpha", Reserve ? SrcValue : Value);
                iTween.FadeTo(gameObject, m_Hashtable);
                break;
            case TweenType.eScaleTo:
                m_Hashtable.Add("scale", Reserve ? SrcAmount : Amount);
                iTween.ScaleTo(gameObject, m_Hashtable);
                break;
            case TweenType.eScaleFrom:
                m_Hashtable.Add("scale", Reserve ? SrcAmount : Amount);
                iTween.ScaleFrom(gameObject, m_Hashtable);
                break;
            case TweenType.eMoveTo:
                m_Hashtable.Add("position", Reserve ? SrcAmount : Amount);
                iTween.MoveTo(gameObject, m_Hashtable);
                break;
            case TweenType.eMoveFrom:
                m_Hashtable.Add("position", Reserve ? SrcAmount : Amount);
                iTween.MoveFrom(gameObject, m_Hashtable);
                break;
            case TweenType.eRotateTo:
                m_Hashtable.Add("rotation", Reserve ? SrcAmount : Amount);
                iTween.RotateTo(gameObject, m_Hashtable);
                break;
            case TweenType.eRotateFrom:
                m_Hashtable.Add("rotation", Reserve ? SrcAmount : Amount);
                iTween.RotateFrom(gameObject, m_Hashtable);
                break;
            case TweenType.eShakePosition:
                m_Hashtable.Add("amount", Reserve ? SrcAmount : Amount);
                iTween.ShakePosition(gameObject, m_Hashtable);
                break;
            case TweenType.eShakeScale:
                m_Hashtable.Add("amount", Reserve ? SrcAmount : Amount);
                iTween.ShakeScale(gameObject, m_Hashtable);
                break;
            case TweenType.eShakeRotation:
                m_Hashtable.Add("amount", Reserve ? SrcAmount : Amount);
                iTween.ShakeRotation(gameObject, m_Hashtable);
                break;
            case TweenType.ePunchPosition:
                m_Hashtable.Add("amount", Reserve ? SrcAmount : Amount);
                iTween.PunchPosition(gameObject, m_Hashtable);
                break;
            case TweenType.ePunchScale:
                m_Hashtable.Add("amount", Reserve ? SrcAmount : Amount);
                iTween.PunchScale(gameObject, m_Hashtable);
                break;
            case TweenType.ePunchRotation:
                m_Hashtable.Add("amount", Reserve ? SrcAmount : Amount);
                iTween.PunchRotation(gameObject, m_Hashtable);
                break;
            case TweenType.eValueTo:
                m_Hashtable.Add("from", Value1);
                m_Hashtable.Add("to", Value);
                iTween.ValueTo(gameObject, m_Hashtable);
                break;
            default:
                break;

        }
    }

    public bool IsPlaying
    {
        get { return null != gameObject.GetComponent<iTween>(); }
    }

    public void Stop()
    {
        iTween.Stop(gameObject);
    }

    private void HandleOnComplete()
    {
        if (IsIgnoreCallBack)
            return;
        if (null != m_OnCompleteAction)
            m_OnCompleteAction.Invoke();
        if (null != OnComplete)
            OnComplete.Invoke();
    }
    public void SetHashtableParams(string key, object value)
    {
        //if(!m_Hashtable.ContainsKey(key))
        //    m_Hashtable.Add(key, value);
        m_Hashtable[key] = value;
    }
    public static void PlayTween(GameObject obj, Action onupdate = null)
    {
        PlayTween(obj, onupdate, false, null);
    }

    public static void PlayTween(GameObject obj, Action oncomplet, bool Reserve = false, Action onupdate = null)
    {
        if (null == obj)
            return;
        var players = obj.GetComponents<TweenPlayer>();
        if (null == players || players.Length <= 0)
            return;
        for (int i = 0; i < players.Length; i++)
        {
            if (players[i].Type == TweenType.ePunchScale && players[i].IsPlaying)//punch系列不能重复播放
                continue;
            players[i].Play(oncomplet, Reserve, onupdate);
        }
    }

    public static void PlayLoopTween(GameObject obj)
    {
        if (null == obj)
            return;
        var players = obj.GetComponents<TweenPlayer>();
        if (null == players || players.Length <= 0)
            return;
        for (int i = 0; i < players.Length; i++)
        {
            if (players[i].LoopType == iTween.LoopType.loop
                || players[i].LoopType == iTween.LoopType.pingPong
                || players[i].Type == TweenType.ePunchScale && players[i].IsPlaying)
                players[i].Play();
        }
    }

    public static void PlayTweenBack(GameObject obj, Action oncomplete, Action onupdate = null)
    {
        PlayTween(obj, oncomplete, true, onupdate);
    }
}
