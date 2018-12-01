using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 能够来回播放 并且在东西结束能隐藏自己
/// 目前支持物体scale方式的出现隐藏 0-1
/// </summary>
[RequireComponent(typeof(TweenPlayer))]
public class TweenToggle : MonoBehaviour
{
    public bool IsHideSelf = true;//在结束时隐藏自己
    private TweenPlayer[] m_Players;
    private List<TweenPlayer.TweenType> m_HasTargetValue = new List<TweenPlayer.TweenType>();//拥有目标值得不设置值 播放一次清除
    private bool m_bIsDelay = false;//仅限隐藏
    private float m_fDelayTime;

    private void Awake()
    {
        if (null == m_Players)
        {
            m_Players = transform.GetComponents<TweenPlayer>();
            if (null == m_Players)
                return;
        }
    }

    /// <summary>
    /// 给指定组件设置值
    /// </summary>
    public void SetValue(TweenPlayer.TweenType tween, Vector3 amount)
    {
        Awake();
        if (null == m_Players)
            return;
        if (!m_HasTargetValue.Contains(tween))
            m_HasTargetValue.Add(tween);
        for (int i = 0; i < m_Players.Length; i++)
        {
            if (m_Players[i].Type == tween)
                m_Players[i].Amount = amount;
        }
    }
    public void Show()
    {
        Awake();

        if (null == m_Players)
            return;
        for (int i = 0; i < m_Players.Length; i++)
        {
            var m_Player = m_Players[i];

            gameObject.SetActive(true);
            m_Player.StopOtherTweenWhenPlay = false;
            if (!m_HasTargetValue.Contains(m_Player.Type))
                if (m_Player.Type == TweenPlayer.TweenType.eFadeTo && null != transform.GetComponent<Image>())
                {
                    var color = transform.GetComponent<Image>().color;
                    color.a = 0;
                    transform.GetComponent<Image>().color = color;
                    m_Player.Value = 1;
                }
                else if (m_Player.Type == TweenPlayer.TweenType.eScaleTo)
                {
                    transform.localScale = Vector3.zero;
                    m_Player.Amount = Vector3.one;
                }
            m_Player.Play();
        }
        m_HasTargetValue.Clear();
    }

    public void Hide(float delayTime = 0)
    {
        Awake();
        if (delayTime > 0)
        {
            m_fDelayTime = delayTime;
            m_bIsDelay = true;
            return;
        }

        if (null == m_Players)
            return;
        for (int i = 0; i < m_Players.Length; i++)
        {
            var m_Player = m_Players[i];
            m_Player.StopOtherTweenWhenPlay = false;
            if (!m_HasTargetValue.Contains(m_Player.Type))
                if (m_Player.Type == TweenPlayer.TweenType.eFadeTo && null != transform.GetComponent<Image>())
                {
                    var color = transform.GetComponent<Image>().color;
                    color.a = 1;
                    transform.GetComponent<Image>().color = color;
                    m_Player.Value = 0;
                }
                else if (m_Player.Type == TweenPlayer.TweenType.eScaleTo)
                {
                    transform.localScale = Vector3.one;
                    m_Player.Amount = Vector3.zero;
                }
            m_Player.Play(_HideSelf);
        }
        m_HasTargetValue.Clear();
    }

    /// <summary>
    /// 强制关闭
    /// </summary>
    public void ForceHide()
    {
        iTween.Stop(gameObject);
        _HideSelf();
    }

    private void _HideSelf()
    {
        if (IsHideSelf)
            gameObject.SetActive(false);
    }

    private void Update()
    {
        if (m_bIsDelay)
        {
            if (m_fDelayTime > 0)
                m_fDelayTime -= Time.deltaTime;
            else
            {
                m_bIsDelay = false;
                Hide();
            }
        }
    }
}
