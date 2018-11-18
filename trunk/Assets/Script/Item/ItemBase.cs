using cakeslice;
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

    protected bool m_bIsShowOutLine = true;
    protected bool m_bIsSelect;


    protected virtual void Awake()
    {
        m_aOutline = gameObject.GetComponent<Outline>();
        if (null != m_aOutline)
            m_aOutline.enabled = false;
    }

    protected virtual void Start()
    {
        if (null != m_aOutline)
            m_aOutline.enabled = false;
    }

    void Update()
    {

    }

    protected virtual void OnMouseEnter()//外发光
    {
        if (null != m_aOutline && m_bIsShowOutLine)
            m_aOutline.enabled = true;
    }

    protected virtual void OnMouseExit()
    {
        if (null != m_aOutline)
            m_aOutline.enabled = false;
    }

    protected virtual void OnMouseUp()
    {
        if (null != m_aOutline)
            m_aOutline.enabled = false;

    }
    
}
