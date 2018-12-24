using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BulidingDev : MonoBehaviour
{
    [SerializeField]
    private GameObject m_TextObj;
    private Camera m_MainCamera;
    Vector3 rot = new Vector3(0, 180, 0);
    // Use this for initialization
    void Start()
    {
        m_MainCamera = GameObject.FindWithTag("MainCamera").GetComponent<Camera>();
    }

    // Update is called once per frame
    void Update()
    {
        if (null != m_TextObj)
        {

            m_TextObj.SetActive(IsAPointInACamera(m_MainCamera, transform.position));

            var forward = Vector3.MoveTowards(m_TextObj.transform.forward, m_TextObj.transform.position - m_MainCamera.transform.position, Time.deltaTime * 5);
            m_TextObj.transform.forward = forward;
        }
    }

    public bool IsAPointInACamera(Camera cam, Vector3 wordPos)
    {
        // 是否在视野内
        bool result1 = false;
        if (null == cam)
            return true;
        Vector3 posViewport = cam.WorldToViewportPoint(wordPos);
        Rect rect = new Rect(0, 0, 1, 1);
        result1 = rect.Contains(posViewport);
        // 是否在远近平面内
        bool result2 = false;
        if (posViewport.z >= cam.nearClipPlane && posViewport.z <= cam.farClipPlane)
        {
            result2 = true;
        }
        // 综合判断
        bool result = result1 && result2;
        return result;
    }

    private Vector3 WorldToUIPoint(Canvas canvas, Vector3 TargetPos)
    {
        Vector3 v_v3 = Camera.main.WorldToScreenPoint(TargetPos);
        Vector3 v_ui = canvas.worldCamera.ScreenToWorldPoint(v_v3);
        Vector3 v_new = new Vector3(v_ui.x, v_ui.y, canvas.GetComponent<RectTransform>().anchoredPosition3D.z);
        return v_new;
    }
}
