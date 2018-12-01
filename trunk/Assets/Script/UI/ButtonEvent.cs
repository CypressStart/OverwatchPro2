using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ButtonEvent : MonoBehaviour
{
    public int IntValue;
    public string StringValue;

    /// <summary>
    /// 跳转场景
    /// </summary>
    /// <param name="sceneIndex"></param>
    public void GotoScene(int sceneIndex)
    {
        if (sceneIndex >= 0)
        {
            ItemManager.GetInstance().Release();
            SceneManager.LoadScene(sceneIndex);
        }
    }
}
