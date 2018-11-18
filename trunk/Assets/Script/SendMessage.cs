using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SendMessage : MonoBehaviour
{
    void Awake()
    {
    }
    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
    //SendMessage("SendMessage", "SetDataUrl", url);
    public void SetDataUrl(string url)
    {
        if (string.IsNullOrEmpty(url))
            return;
        DataManager.GetInstance().SaveDataPath(url);
        DataManager.GetInstance().Load();
    }

    public void MyFunction(int v, int v2, int v3)
    {
        //TODO
    }
    public void MyFunction2(int v)
    {
        //TODO
    }
    public void MyFunction3(System.Object obj)
    {
        UIManager.GetInstance().DebugText(obj.ToString());
        //TODO
    }
}
