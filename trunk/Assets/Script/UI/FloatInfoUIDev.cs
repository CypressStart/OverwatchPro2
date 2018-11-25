using System;
using System.Collections.Generic;
using UnityEngine;

public class FloatInfoUIDev : MonoBehaviour
{
    private const float ANIMTIME = .5f;

    [SerializeField]
    private Transform m_ListTransform;

    [SerializeField]
    private FloatInfoItem m_ItemInstance;

    [SerializeField]
    private RectTransform m_Content;

    [SerializeField]
    private float m_fHight = 20;

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

    private void Start()
    {
        m_rect = transform as RectTransform;
    }

    private void Update()
    {
        UpdatePos();
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            var data = new InformationData();
            data.ID = "11";
            data.Contentlist = new List<InfoContent>();
            data.Contentlist.Add(new InfoContent()
            {
                Name = "aaa",
                Value = "111",
                ViewType = EContentViewType.E_Show
            });

            data.Contentlist.Add(new InfoContent()
            {
                Name = "bbb",
                Value = "222",
                ViewType = EContentViewType.E_Show
            });

            data.Contentlist.Add(new InfoContent()
            {
                Name = "ccc",
                Value = "333",
                ViewType = EContentViewType.E_Show
            });

            data.Contentlist.Add(new InfoContent()
            {
                Name = "aaa1",
                Value = "a111",
                ViewType = EContentViewType.E_Hide
            });

            data.Contentlist.Add(new InfoContent()
            {
                Name = "bbb2",
                Value = "b222",
                ViewType = EContentViewType.E_Hide
            });

            data.Contentlist.Add(new InfoContent()
            {
                Name = "ccc3",
                Value = "c333",
                ViewType = EContentViewType.E_Hide
            });
            Initialized(data);
        }

        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            ShowDetail();
        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            HideDetail();
        }

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
        if (null != m_NodeObj)//
        {
            if (null == m_MainCamera)
            {
                m_MainCamera = GameObject.FindWithTag("MainCamera").GetComponent<Camera>();
                m_UiRoot = GameObject.FindWithTag("UIRoot").GetComponent<RectTransform>();
                m_Canvas = m_UiRoot.GetComponent<Canvas>();
            }
        }
        if (!IsAPointInACamera(m_MainCamera, TargetPos))
        {
            //if (Vector3.zero != transform.localScale)
            //    transform.localScale = Vector3.zero;
            //return;
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
        for (int i = 0; i < list.Count; i++)
        {
            if (list[i].ViewType == EContentViewType.E_Show)
                m_nNormalInfoCount++;

            var obj = GameObject.Instantiate(m_ItemInstance.gameObject) as GameObject;
            var dev = obj.GetComponent<FloatInfoItem>();
            obj.transform.SetParent(m_ListTransform, false);
            dev.SetContent(list[i].Name, list[i].Value);
            obj.SetActive(true);
            m_ItemList.Add(dev);
        }
        SetContentHight(m_nNormalInfoCount * m_fHight);
    }

    public float ContentHight { get { return m_Content.sizeDelta.y; } }

    private void SetContentHight(float hight)
    {
        var der = m_Content.sizeDelta;
        der.y = hight;
        m_Content.sizeDelta = der;
    }

    public void Show(bool detail = false)
    {
    }

    public void Hide()
    {
    }

    public void ShowDetail()
    {
        if (m_bIsShowDetail) return;
        m_bIsShowDetail = true;

        m_IsAnimRunning = true;
        m_fTargetHight = m_nTotalInfoCount * m_fHight;
        m_AnimTimeIndex = 0;
    }

    public void HideDetail()
    {
        if (!m_bIsShowDetail) return;
        m_bIsShowDetail = false;

        m_IsAnimRunning = true;
        m_fTargetHight = m_nNormalInfoCount * m_fHight;
        m_AnimTimeIndex = 0;
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
}