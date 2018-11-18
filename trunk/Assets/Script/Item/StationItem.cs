using UnityEngine;
using System;
using UnityEngine.EventSystems;
using cakeslice;
using System.Collections.Generic;

/// <summary>
/// 工位
/// </summary>
public class StationItem : ItemBase
{
    const string STYLE = "Style";
    const string EQUIPMENT = "equipment";

    public int Index;//序号

    private SimpleInfoUIItem m_SimpleInfo;
    private StationData m_Data;
    public string Link { get { return m_Data.Link; } }
    public string ID { get { return m_Data.ID; } }
    private Transform m_ManObjList;
    private List<Outline> m_Outlinelist;

    private List<PartItem> m_ManObjs = new List<PartItem>();

    private Transform m_UIPoint;

    private List<PartItem> m_ManList = new List<PartItem>();
    private List<PartItem> m_CameraList = new List<PartItem>();
    private List<PartItem> m_CabinetList = new List<PartItem>();

    private GameObject m_CurStyle;
    private GameObject m_CurEquipment;
    private Vector3 m_vHudPos;
    public Vector3 HudPos { get { return m_vHudPos; } }

    protected override void Awake()
    {
        base.Awake();
        var str = gameObject.name.Replace("Station", "");
        Index = int.Parse(str);
    }

    protected override void Start()
    {

        base.Start();
        m_UIPoint = transform.Find("point");
        m_vHudPos = m_UIPoint.position;
        ItemManager.GetInstance().AddStation(this);
        m_SimpleInfo = UIManager.GetInstance().AddSimpleInfo();
        m_SimpleInfo.TargetPos = HudPos;
        m_Outlinelist = new List<Outline>();
        m_Outlinelist.AddRange(gameObject.GetComponentsInChildren<Outline>());

        m_Outlinelist.AddRange(transform.Find("Style1").GetComponentsInChildren<Outline>());
        m_Outlinelist.AddRange(transform.Find("Style2").GetComponentsInChildren<Outline>());
        m_Outlinelist.AddRange(transform.Find("Style3").GetComponentsInChildren<Outline>());

        var s1 = transform.Find("Style1/zhuozi_01");
        m_Outlinelist.AddRange(s1.GetComponentsInChildren<Outline>());

        s1 = transform.Find("Style2/zhuozi_02");
        m_Outlinelist.AddRange(s1.GetComponentsInChildren<Outline>());

        s1 = transform.Find("Style3/zhuozi_03");
        m_Outlinelist.AddRange(s1.GetComponentsInChildren<Outline>());

        s1 = transform.Find("equipment1");
        m_Outlinelist.Add(s1.GetComponent<Outline>());

        s1 = transform.Find("equipment2");
        m_Outlinelist.Add(s1.GetComponent<Outline>());

        SetOutLine(false);
    }

    internal void Select()
    {
        m_bIsShowOutLine = false;
        m_bIsSelect = true;

        var cameraPos = transform.position;
        cameraPos -= CameraController.GetInstance().Forward * 8;
        if (null == ItemManager.GetInstance().CurSelectStation)
            CameraController.GetInstance().Record();
        CameraController.GetInstance().MoveTo(cameraPos);
        CameraController.GetInstance().LockMovtion = true;
        CameraController.GetInstance().RotPoint = transform.position;
        UIManager.GetInstance().HideSimpleInfo();
        UIManager.GetInstance().ShowDetalInfo(m_Data);
        if (null != ItemManager.GetInstance().CurSelectStation)
            ItemManager.GetInstance().CurSelectStation.CancelSelect();
        ItemManager.GetInstance().CurSelectStation = this;
        SetOutLine(false);

        SetHUD(true);
    }

    void Update()
    {
    }

    protected override void OnMouseUp()
    {
        if (null != EventSystem.current.currentSelectedGameObject)//说明点在UI上了
            return;
        base.OnMouseUp();
        Select();
    }

    protected override void OnMouseEnter()
    {
        base.OnMouseEnter();
        SetColor(true);
        SetOutLine(true);
    }

    protected override void OnMouseExit()
    {
        base.OnMouseExit();
        SetColor(false);
        SetOutLine(false);
    }

    internal void CancelSelect()
    {
        m_bIsShowOutLine = true;
        m_bIsSelect = false;
        SetColor(false);
        SetOutLine(false);
        SetHUD(false);
    }

    internal void Refresh(StationData stationData)
    {
        if (!string.IsNullOrEmpty(stationData.Style))
        {
            var obj = transform.Find(STYLE + stationData.Style);
            if (null != m_CurStyle && obj.gameObject != m_CurStyle)
                m_CurStyle.SetActive(false);
            m_CurStyle = obj.gameObject;
            m_CurStyle.SetActive(true);
            m_CurStyle = obj.gameObject;
        }
        else if (null != m_CurStyle)
            m_CurStyle.SetActive(false);
        if (null != m_CurStyle)
            m_ManObjList = m_CurStyle.transform.Find("ManList");
        else
            m_ManObjList = null;

        transform.localEulerAngles = new Vector3(0, 90 * stationData.RotateIndex, 0);
        if (null != m_ManObjList)
        {
            m_ManObjs.AddRange(m_ManObjList.GetComponentsInChildren<PartItem>());
            for (int i = 0; i < m_ManObjs.Count; i++)
            {
                m_ManObjs[i].SetState(false);
            }
        }

        if (!string.IsNullOrEmpty(stationData.MissionType) && stationData.StateType != 3)
        {
            var obj = transform.Find(EQUIPMENT + stationData.MissionType);
            if (null != m_CurEquipment && obj.gameObject != m_CurEquipment)
                m_CurEquipment.SetActive(false);
            m_CurEquipment = obj.gameObject;
            m_CurEquipment.SetActive(true);
        }
        else if (null != m_CurEquipment)
        {
            m_CurEquipment.SetActive(false);
        }

        m_Data = stationData;
        m_SimpleInfo.TextID.text = m_Data.ID.ToString();
        m_SimpleInfo.TextVersion.text = m_Data.Version;
        m_SimpleInfo.TextNode.text = m_Data.Node;
        m_SimpleInfo.TextRate.text = string.IsNullOrEmpty(m_Data.Rate) ? "" : m_Data.Rate + "%";
        m_SimpleInfo.SetStateIcon(m_Data.StateType.ToString());
        switch (m_Data.StateType)
        {
            case 0: m_SimpleInfo.BG.color = Color.green; break;
            case 1: m_SimpleInfo.BG.color = Color.red; break;
            case 2: m_SimpleInfo.BG.color = Color.yellow; break;
            case 3: m_SimpleInfo.BG.color = Color.white; break;
        }
        SetColor(false);

        LoadCamera();
        LoadCabinet();

        if (!m_bIsSelect)
            SetHUD(false);


    }

    public void SetColor(bool isA)
    {
        var color = m_SimpleInfo.BG.color;
        color.a = isA ? 1 : .8f;
        m_SimpleInfo.BG.color = color;
    }

    private void SetOutLine(bool isShow)
    {
        if (isShow && !m_bIsShowOutLine)
            return;
        if (null == m_Outlinelist || m_Outlinelist.Count <= 0)
            return;
        for (int i = 0; i < m_Outlinelist.Count; i++)
        {
            m_Outlinelist[i].enabled = isShow;
        }
    }


    private void SetHUD(bool isShow)
    {
        if (m_Data.StateType != 3)
            for (int i = 0; i < m_ManList.Count; i++)
            {
                m_ManList[i].SetHUD(isShow);
            }

        for (int i = 0; i < m_CabinetList.Count; i++)
        {
            m_CabinetList[i].SetHUD(isShow);
        }

        for (int i = 0; i < m_CameraList.Count; i++)
        {
            m_CameraList[i].SetHUD(isShow);
        }
    }

    private void LoadCamera()
    {
        m_CameraList = ItemManager.GetInstance().GetCameraById(Index.ToString());
        //List<PartData> dataList
    }

    private void LoadCabinet()
    {
        m_CabinetList = ItemManager.GetInstance().GetCabinetById(Index.ToString());
    }
}