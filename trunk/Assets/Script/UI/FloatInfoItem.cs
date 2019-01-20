using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FloatInfoItem : MonoBehaviour
{
    [SerializeField]
    private Text m_NameText;
    [SerializeField]
    private Text m_ValueText;


    public bool IsHide { get; set; }

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void SetContent(string name, string value, Color color)
    {
        if (null == m_NameText) return;
        m_NameText.text = name;
        m_NameText.color = color;
        m_ValueText.text = value;
        m_ValueText.color = color;
    }
}
