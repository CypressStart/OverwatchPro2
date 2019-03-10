using cakeslice;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BuildingName : MonoBehaviour
{
    [SerializeField]
    private GameObject m_TextObj;
    [SerializeField]
    private int m_SceneIndex;
    [SerializeField]
    private bool m_IsSelect;
    [SerializeField]
    private GameObject m_SclectWin;
    private Outline m_Outline;
    void Start()
    {
        m_Outline = transform.GetComponent<Outline>();
        if (null != m_Outline)
            m_Outline.enabled = false;
        if (null != m_TextObj)
            m_TextObj.SetActive(false);
    }

    void Update()
    {

    }
    public void OnMouseEnter()
    {
        if (null == m_TextObj || null == m_Outline)
            return;
        m_TextObj.SetActive(true);
        m_Outline.enabled = true;
    }

    public void OnMouseExit()
    {
        if (null == m_TextObj || null == m_Outline)
            return;
        m_TextObj.SetActive(false);
        m_Outline.enabled = false;
    }

    public void OnMouseDown()
    {
        if (!m_IsSelect)
        {
            SceneManager.LoadScene(m_SceneIndex);
        }
        else
        {
            if (null != m_SclectWin)
                m_SclectWin.SetActive(true);
        }
    }
}
