using UnityEngine;
using UnityEngine.UI;
using System;

public class SimpleInfoUIItem : MonoBehaviour
{

    public Text TextID;
    public Text TextVersion;
    public Text TextNode;
    public Text TextRate;
    public Image BG;
    public Transform StateIcon;
    [HideInInspector]
    public Vector3 TargetPos;
    private RectTransform m_rect;
    private Vector2 m_vPos;

    private Camera m_MainCamera;
    private RectTransform m_UiRoot;
    private Canvas m_Canvas;
    private Vector3 m_scale;
    private void Awake()
    {
        m_rect = transform as RectTransform;
        m_scale = transform.localScale;
    }

    private void Update()
    {
        if (Vector3.zero == TargetPos)
            return;
        if (null == m_MainCamera)
        {
            m_MainCamera = CameraController.GetInstance().MainCamera;
            m_UiRoot = UIManager.GetInstance().transform as RectTransform;
            m_Canvas = UIManager.GetInstance().transform.GetComponent<Canvas>();
        }
        if (!IsAPointInACamera(m_MainCamera, TargetPos))
        {
            if (Vector3.zero != transform.localScale)
                transform.localScale = Vector3.zero;
            return;
        }
        if (Vector3.zero == transform.localScale)
            transform.localScale = m_scale;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(m_UiRoot, m_MainCamera.WorldToScreenPoint(TargetPos), m_MainCamera, out m_vPos);
        m_rect.position = m_vPos;
        transform.position = WorldToUIPoint(m_Canvas, TargetPos);
    }

    public bool IsAPointInACamera(Camera cam, Vector3 wordPos)
    {
        // 是否在视野内
        bool result1 = false;
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

    internal void Show()
    {
        gameObject.SetActive(true);
    }

    public void SetStateIcon(string state)
    {
        if (null != StateIcon)
        {
            for (int i = 0; i < StateIcon.childCount; i++)
            {
                var child = StateIcon.GetChild(i).gameObject;
                child.SetActive(child.name == state);
            }
        }
    }

    internal void Hide()
    {
        gameObject.SetActive(false);
    }

    public Vector3 WorldToUIPoint(Canvas canvas, Vector3 TargetPos)
    {
        Vector3 v_v3 = Camera.main.WorldToScreenPoint(TargetPos);
        Vector3 v_ui = canvas.worldCamera.ScreenToWorldPoint(v_v3);
        Vector3 v_new = new Vector3(v_ui.x, v_ui.y, canvas.GetComponent<RectTransform>().anchoredPosition3D.z);
        return v_new;
    }
}