using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Main : MonoBehaviour
{
    [SerializeField]
    private string m_SceneDataPath;

    private bool m_bIsInit = false;

    private void Awake()
    {
        //预留加载数据位置
    }

    void Start()
    {
        //Application.ExternalCall("MyFunction1", 111, 222);
        //Application.ExternalCall("MyFunction2", 333);
    }

    private void LateUpdate()
    {
        if (!m_bIsInit)
        {
            m_bIsInit = true;
            DataManager.GetInstance().Init(m_SceneDataPath);
            DataManager.GetInstance().Load();
            Application.ExternalCall("OnLoadFinish");
        }
    }

    void Update()
    {
        if (null != DataManager.GetInstance() || m_bIsInit)
            DataManager.GetInstance().Tick(Time.deltaTime);

        if (Input.GetKeyDown(KeyCode.F5))
        {
            DataManager.GetInstance().Load();
        }
        if (Input.GetKeyDown(KeyCode.F9))
        {
            SceneManager.LoadScene(0);
        }
    }

    public void GoToLink()
    {
        //if (null == ItemManager.GetInstance().CurSelectStation)
        //    return;
        //if (!string.IsNullOrEmpty(ItemManager.GetInstance().CurSelectStation.Link))
        //{
        //    Application.ExternalCall("OnSelect", ItemManager.GetInstance().CurSelectStation.Link);
        //}
    }
}
