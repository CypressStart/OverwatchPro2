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

    }

    public TweenType Type;
    public iTween.EaseType EaseType = iTween.EaseType.linear;
    public iTween.LoopType LoopType = iTween.LoopType.none;
    public float Time = 1;
    public float Delay = 0;
    public bool IsLocal = true;
    public Vector3 Amount;
    public float Value;
    public UnityEvent OnComplete;
    public bool AutoPlay = false;
    private Hashtable m_Hashtable = new Hashtable();
    private Action m_OnCompleteAction;

    void Start()
    {
        if (AutoPlay)
            Play();
    }

    public void Play()
    {
        Play(null);
    }

    public void Play(Action oncomplete)
    {
        iTween.Stop(gameObject);

        m_OnCompleteAction = oncomplete;
        m_Hashtable.Clear();
        m_Hashtable.Add("time", Time);
        m_Hashtable.Add("delay", Delay);
        m_Hashtable.Add("easetype", EaseType);
        m_Hashtable.Add("looptype", LoopType);
        m_Hashtable.Add("islocal", IsLocal);
        m_Hashtable.Add("oncomplete", (Action)HandleOnComplete);
        m_Hashtable.Add("ignoretimescale", true);
        switch (Type)
        {
            case TweenType.eFadeTo:
                m_Hashtable.Add("alpha", Value);
                iTween.FadeTo(gameObject, m_Hashtable);
                break;
            case TweenType.eScaleTo:
                m_Hashtable.Add("scale", Amount);
                iTween.ScaleTo(gameObject, m_Hashtable);
                break;
            case TweenType.eScaleFrom:
                m_Hashtable.Add("scale", Amount);
                iTween.ScaleFrom(gameObject, m_Hashtable);
                break;
            case TweenType.eMoveTo:
                m_Hashtable.Add("position", Amount);
                iTween.MoveTo(gameObject, m_Hashtable);
                break;
            case TweenType.eMoveFrom:
                m_Hashtable.Add("position", Amount);
                iTween.MoveFrom(gameObject, m_Hashtable);
                break;
            case TweenType.eRotateTo:
                m_Hashtable.Add("rotation", Amount);
                iTween.RotateTo(gameObject, m_Hashtable);
                break;
            case TweenType.eRotateFrom:
                m_Hashtable.Add("rotation", Amount);
                iTween.RotateFrom(gameObject, m_Hashtable);
                break;
            case TweenType.eShakePosition:
                m_Hashtable.Add("amount", Amount);
                iTween.ShakePosition(gameObject, m_Hashtable);
                break;
            case TweenType.eShakeScale:
                m_Hashtable.Add("amount", Amount);
                iTween.ShakeScale(gameObject, m_Hashtable);
                break;
            case TweenType.eShakeRotation:
                m_Hashtable.Add("amount", Amount);
                iTween.ShakeRotation(gameObject, m_Hashtable);
                break;
            case TweenType.ePunchPosition:
                m_Hashtable.Add("amount", Amount);
                iTween.PunchPosition(gameObject, m_Hashtable);
                break;
            case TweenType.ePunchScale:
                m_Hashtable.Add("amount", Amount);
                iTween.PunchScale(gameObject, m_Hashtable);
                break;
            case TweenType.ePunchRotation:
                m_Hashtable.Add("amount", Amount);
                iTween.PunchRotation(gameObject, m_Hashtable);
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
        if (null != m_OnCompleteAction)
            m_OnCompleteAction.Invoke();
        if (null != OnComplete)
            OnComplete.Invoke();
    }

    public static void PlayTween(GameObject obj)
    {
        PlayTween(obj, null);
    }

    public static void PlayTween(GameObject obj, Action oncomplet)
    {
        if (null == obj)
            return;
        var player = obj.GetComponent<TweenPlayer>();
        if (null == player)
            return;
        if (player.Type == TweenType.ePunchScale && player.IsPlaying)//punch系列不能重复播放
            return;
        player.Play(oncomplet);
    }
}
