using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Egg : MonoBehaviour
{
    void Start()
    {
    }

    void Update()
    {

    }
    void OnMouseUp()
    {
        if (null != UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject)//说明点在UI上了
            return;
        UIManager.GetInstance().ShowImgAddOn();
    }
}
