using cakeslice;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// base class of item
/// station/camera/cabinet     human?
/// </summary>
public class ItemBase : MonoBehaviour
{
    private Outline m_aOutline;
    protected List<Outline> m_Outlinelist;

    protected bool m_bIsShowOutLine = true;
    protected bool m_bIsSelect;

    public Action OnMouseEnterCallBack;
    public Action OnMouseExitCallBack;
    public Action OnMouseClickCallBack;

    protected virtual void Awake()
    {
        m_aOutline = gameObject.GetComponent<Outline>();
        if (null != m_aOutline)
            m_aOutline.enabled = false;
        m_Outlinelist = new List<Outline>();
        m_Outlinelist.AddRange(gameObject.GetComponentsInChildren<Outline>());
    }

    protected virtual void Start()
    {
        if (null != m_aOutline)
            m_aOutline.enabled = false;
        SetOutlineList(false);
    }

    void Update()
    {

    }

    protected virtual void OnMouseEnter()//外发光
    {
        if (null != m_aOutline && m_bIsShowOutLine)
        {
            m_aOutline.enabled = true;
        }
        if (m_bIsShowOutLine)
            SetOutlineList(true);
        if (null != OnMouseEnterCallBack)
            OnMouseEnterCallBack.Invoke();
    }

    protected virtual void OnMouseExit()
    {
        if (null != m_aOutline)
            m_aOutline.enabled = false;
        SetOutlineList(false);
        if (null != OnMouseExitCallBack)
            OnMouseExitCallBack.Invoke();
    }

    protected virtual void OnMouseUp()
    {
        if (null != m_aOutline)
            m_aOutline.enabled = false;
        if (null != OnMouseClickCallBack)
            OnMouseClickCallBack.Invoke();
    }

    protected void SetOutlineList(bool isShow)
    {
        if (isShow && null != ItemManager.GetInstance().CurSelectPartItem)
            return;
        if (isShow && !m_bIsShowOutLine)
            return;
        if (null == m_Outlinelist || m_Outlinelist.Count <= 0)
            return;
        for (int i = 0; i < m_Outlinelist.Count; i++)
        {
            m_Outlinelist[i].enabled = isShow;
        }
    }
}
