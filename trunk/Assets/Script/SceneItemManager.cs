using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneItemManager : MonoBehaviour
{
    private static SceneItemManager _instance;

    private void Awake()
    {
        _instance = this;
    }

    private void OnDestroy()    
    {
        _instance = null;
    }

    void Start()
    {

    }

    void Update()
    {

    }
}
