using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EditWinDev : MonoBehaviour
{
    [SerializeField]
    private InputField m_InputField;

    void Start()
    {
        Close();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Home))
        {
            transform.localScale = Vector3.one;
            m_InputField.text = DataManager.GetInstance().DataPath;
        }
    }

    public void Save()
    {
        if (null == m_InputField || string.IsNullOrEmpty(m_InputField.text))
            return;
        DataManager.GetInstance().SaveDataPath(m_InputField.text);
    }

    public void Close()
    {
        transform.localScale = Vector3.zero;
    }
}
