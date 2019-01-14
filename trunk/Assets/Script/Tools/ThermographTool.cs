using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThermographTool : MonoBehaviour
{
    [SerializeField]
    private TextMesh m_TempText;
    [SerializeField]
    private TextMesh m_HumText;
    void Start()
    {
        DataManager.GetInstance().SetThermographTool(this);
        if (null == m_TempText || null == m_HumText)
            return;
        m_TempText.text = string.Empty;
        m_HumText.text = string.Empty;
    }

    void Update()
    {

    }

    public void SetValue()
    {
    }

    internal void SetValue(InformationData infoData)
    {
        if (null == m_TempText || null == m_HumText)
            return;
        if (null == infoData)
            return;
        foreach (var info in infoData.Contentlist)
        {
            if (info.Name == "温度")
                m_TempText.text = info.Name + ":" + info.Value;
            if (info.Name == "湿度")
                m_HumText.text = info.Name + ":" + info.Value;
        }
    }
}
