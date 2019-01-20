using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FloatInfoUIDev : MonoBehaviour
{
    private const float ANIMTIME = .5f;

    [SerializeField]
    private Transform m_ListTransform;

    [SerializeField]
    private FloatInfoItem m_ItemInstance;
    [SerializeField]
    private GameObject m_GoToBtn;
    [SerializeField]
    private RectTransform m_Content;
    [SerializeField]
    private Text m_NameLabel;

    [SerializeField]
    private float m_fHight = 20;
    [SerializeField]
    private float m_fSpacing = 0;

    public string ID { get { return null == m_Data ? string.Empty : m_Data.ID; } }

    private float m_AnimTimeIndex;
    private bool m_IsAnimRunning = false;
    private float m_fTargetHight;//目标列表UI长度
    private int m_nNormalInfoCount;//常规数据量
    private int m_nTotalInfoCount;//总数据量
    private InformationData m_Data;
    private List<FloatInfoItem> m_ItemList = new List<FloatInfoItem>();
    private bool m_bIsShowDetail;
    private GameObject m_NodeObj;
    private Camera m_MainCamera;
    private RectTransform m_UiRoot;
    private Canvas m_Canvas;
    private Vector3 m_scale;
    private Vector3 TargetPos;
    private RectTransform m_rect;
    private Vector2 m_vPos;
    private TweenPlayer m_TweenPlayer;

    private void Awake()
    {
        m_scale = transform.localScale;
        m_fSpacing = m_ListTransform.GetComponent<VerticalLayoutGroup>().spacing;
        m_TweenPlayer = transform.GetComponent<TweenPlayer>();
    }

    private void Start()
    {
        m_rect = transform as RectTransform;
    }

    private void Update()
    {
        UpdatePos();

        if (m_IsAnimRunning)
        {
            m_AnimTimeIndex += Time.deltaTime;

            SetContentHight(UnityEngine.Mathf.Lerp(ContentHight, m_fTargetHight, m_AnimTimeIndex));
            if (m_AnimTimeIndex >= ANIMTIME)
            {
                SetContentHight(m_fTargetHight);
                m_IsAnimRunning = false;
            }
        }
    }

    private void UpdatePos()
    {
        if (m_bIsDetal)
            return;
        if (null != m_NodeObj)//
        {
            if (null == m_MainCamera)
            {
                m_MainCamera = GameObject.FindWithTag("MainCamera").GetComponent<Camera>();
                m_UiRoot = GameObject.FindWithTag("UIRoot").GetComponent<RectTransform>();
                m_Canvas = m_UiRoot.GetComponent<Canvas>();
            }
        }
        if (!IsAPointInACamera(m_MainCamera, m_NodeObj.transform.position))
        {
            if (Vector3.zero != transform.localScale)
                transform.localScale = Vector3.zero;
            return;
        }
        if (Vector3.zero == transform.localScale)
            transform.localScale = m_scale;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(m_UiRoot, m_MainCamera.WorldToScreenPoint(m_NodeObj.transform.position), m_MainCamera, out m_vPos);
        m_rect.position = m_vPos;
        transform.position = WorldToUIPoint(m_Canvas, m_NodeObj.transform.position);
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

    public void Initialized(InformationData info)
    {
        m_Data = info;
        var list = info.Contentlist;
        m_nTotalInfoCount = list.Count;
        m_nNormalInfoCount = 0;
        for (int i = 0; i < list.Count; i++)
        {
            if (list[i].Name == "设备名")
            {
                m_NameLabel.text = list[i].Value;
                continue;
            }
            if (list[i].ViewType == EContentViewType.E_Show)
                m_nNormalInfoCount++;
            if (list[i].Name == "状态")
            {
                switch (list[i].Value)
                {
                    case "空闲": list[i].TextColor = Color.gray; break;
                    case "正常": list[i].TextColor = Color.green; break;
                    case "异常": list[i].TextColor = Color.red; break;
                }
            }
            var obj = GameObject.Instantiate(m_ItemInstance.gameObject) as GameObject;
            var dev = obj.GetComponent<FloatInfoItem>();
            obj.transform.SetParent(m_ListTransform, false);
            dev.SetContent(list[i].Name, list[i].Value, list[i].TextColor);
            obj.SetActive(true);
            dev.IsHide = list[i].ViewType == EContentViewType.E_Hide;
            m_ItemList.Add(dev);
        }
        SetContentHight(m_nNormalInfoCount * m_fHight + ((m_nNormalInfoCount - 1) * m_fSpacing));
        SetHidenItemState(false);
    }

    public float ContentHight { get { return m_Content.sizeDelta.y; } }

    private void SetContentHight(float hight)
    {
        var der = m_Content.sizeDelta;
        der.y = hight;
        m_Content.sizeDelta = der;
    }

    public void SetHidenItemState(bool isShow)
    {
        for (int i = 0; i < m_ItemList.Count; i++)
        {
            if (m_ItemList[i].IsHide)
                m_ItemList[i].gameObject.SetActive(isShow);
        }
    }

    /// <summary>
    /// 显示在最前边
    /// </summary>
    public void MoveToUp()
    {
        transform.SetSiblingIndex(transform.parent.childCount - 1);
    }

    public void ShowDetail()
    {
        if (m_bIsShowDetail) return;
        SetHidenItemState(true);
        m_bIsShowDetail = true;

        m_IsAnimRunning = true;
        m_fTargetHight = m_nTotalInfoCount * m_fHight + ((m_nTotalInfoCount - 1) * m_fSpacing);
        m_AnimTimeIndex = 0;
    }

    public void HideDetail()
    {
        if (!m_bIsShowDetail) return;
        SetHidenItemState(false);
        m_bIsShowDetail = false;

        m_IsAnimRunning = true;
        m_fTargetHight = m_nNormalInfoCount * m_fHight + ((m_nNormalInfoCount - 1) * m_fSpacing);
        m_AnimTimeIndex = 0;
    }

    public void BtnGoToLink()
    {
        var link = DataManager.GetInstance().GetLink(ID);
        if (!string.IsNullOrEmpty(link))
            Application.ExternalCall("OnSelect", link);
    }

    /// <summary>
    /// 设置追踪节点
    /// </summary>
    /// <param name="gameObject"></param>
    internal void SetNode(GameObject gameObject)
    {
        m_NodeObj = gameObject;
    }

    internal void Modification(InformationData informationData)
    {
        for (int i = 0; i < m_ItemList.Count; i++)
        {
            DestroyImmediate(m_ItemList[i].gameObject);
        }
        m_ItemList.Clear();
        Initialized(informationData);
    }

    private bool m_bIsDetal = false;

    public void SetDetal(bool isDetal)
    {
        m_bIsDetal = isDetal;
        if (null != m_GoToBtn)
            m_GoToBtn.SetActive(isDetal);

        if (isDetal)
        {
            transform.localPosition = new Vector3(10, -35, 0);
            transform.localScale = Vector3.one;
            m_TweenPlayer.Amount = Vector3.one;
        }
        else
        {
            transform.localScale = Vector3.one * .5f;
            m_TweenPlayer.Amount = Vector3.one * .5f;
        }
    }
}