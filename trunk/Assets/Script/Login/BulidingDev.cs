using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BulidingDev : MonoBehaviour
{
    [SerializeField]
    private int m_SceneIndex = -1;
    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    void OnMouseUp()
    {
        if (m_SceneIndex <= 0)
            return;
        ItemManager.GetInstance().Release();
        SceneManager.LoadScene(m_SceneIndex);
    }
}
