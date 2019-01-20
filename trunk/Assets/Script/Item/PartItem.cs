using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using cakeslice;

public class PartItem : ItemBase
{
    public TextMesh HUD;
    public string ID { get { return m_ID; } }
    public string ItemType { get; set; }
    private string m_URL;
    private string m_ID;
    private string HUDName;
    private _LookAtCamera m_str;
    [HideInInspector]
    public bool IsPublicCamera = false;//公共摄像头点击相应不同

    private Material m_DisplayMaterial;
    private List<TextMesh> m_DisplayText = new List<TextMesh>();
    private Transform m_ViewPoint;

    protected override void Awake()
    {
        base.Awake();

        var display = transform.Find("Display");
        if (null != display)
        {
            m_DisplayMaterial = display.GetComponent<Renderer>().material;
            m_DisplayText.AddRange(display.GetComponentsInChildren<TextMesh>());
        }
        m_ViewPoint = transform.Find("ViewPoint");
    }

    public void SetState(bool isShow)
    {
        if (null != gameObject)
            gameObject.SetActive(isShow);
    }

    internal void Refresh(PartData partData)
    {
        m_ID = partData.ID;
        m_URL = partData.Link;
        if (IsPublicCamera)
            return;
        transform.position = partData.Pos;
        transform.rotation = partData.Rot;
        HUDName = partData.Name;
        if (null != HUD)
        {
            HUD.text = HUDName;
            if (null == m_str)
            {
                m_str = HUD.transform.parent.gameObject.AddComponent<_LookAtCamera>();
                m_str.CameraTran = CameraController.GetInstance().MainCamera.transform;
            }
        }
    }

    internal void Select()
    {
        ItemManager.GetInstance().SwitchFloatUIState(false);
        ItemManager.GetInstance().ShowDetalInfo(m_ID, true);
        ItemManager.GetInstance().ShowDetalInfo(m_ID, true);
        m_bIsShowOutLine = false;
        m_bIsSelect = true;

        var cameraPos = transform.position;
        cameraPos -= CameraController.GetInstance().Forward * 8;
        if (null == ItemManager.GetInstance().CurSelectPartItem)
            CameraController.GetInstance().Record();
        CameraController.GetInstance().LockMovtion = true;
        if (null != m_ViewPoint)
            CameraController.GetInstance().MoveTo(m_ViewPoint.transform.position);
        else
            CameraController.GetInstance().MoveTo(cameraPos);

        if (null != m_ViewPoint)
            CameraController.GetInstance().RotateTo(m_ViewPoint.transform.rotation);
        else
            CameraController.GetInstance().RotPoint = transform.position;

        UIManager.GetInstance().HideSimpleInfo();
        //UIManager.GetInstance().ShowDetalInfo(m_Data);
        if (null != ItemManager.GetInstance().CurSelectPartItem)
            ItemManager.GetInstance().CurSelectPartItem.CancelSelect();
        ItemManager.GetInstance().CurSelectPartItem = this;

        SetOutlineList(false);

        //SetHUD(true);
    }

    internal void CancelSelect()
    {
        m_bIsShowOutLine = true;
        m_bIsSelect = false;
        //SetHUD(false);
    }

    protected override void OnMouseUp()
    {

        Select();

        return;

        //base.OnMouseDown();
        if (string.IsNullOrEmpty(m_URL))
            return;

        //上级被选中时才能点击
        if (m_bIsSelected || IsPublicCamera)
        {
            Application.ExternalCall("OnSelect", m_URL);
        }
        else
        {
            var station = ItemManager.GetInstance().GetStationItem(m_ID);
            station.Select();
        }
        //Application.OpenURL(m_URL);
        //先直接跳转连接 其他换需求再说
    }


    private bool m_bIsSelected;

    public void SetHUD(bool isShow)
    {
        m_bIsSelected = isShow;
        if (string.IsNullOrEmpty(HUDName))
            return;
        HUD.transform.parent.gameObject.SetActive(isShow);
    }

    public void SetHUD(bool isShow, string name)
    {
        if (null != HUD)
        {
            HUD.text = HUDName;
            if (null == m_str)
            {
                m_str = HUD.transform.parent.gameObject.AddComponent<_LookAtCamera>();
                m_str.CameraTran = CameraController.GetInstance().MainCamera.transform;
            }
        }
        HUDName = name;
        if (null != HUD)
            HUD.text = HUDName;
        SetHUD(isShow);
    }

    /// <summary>
    /// 设置设备上显示器的内容
    /// </summary>
    /// <param name="informationData"></param>
    internal void SetPanelData(InformationData informationData)
    {
        if (null == m_DisplayText || m_DisplayText.Count <= 0)
            return;
        if (null != m_DisplayMaterial)
            m_DisplayMaterial.color = informationData.PanelColor;
        var displayindex = 0;
        foreach (var item in m_DisplayText)
        {
            if (null != item)
                item.gameObject.SetActive(false);
        }

        for (int i = 0; i < informationData.Contentlist.Count; i++)
        {
            if (informationData.Contentlist[i].IsShowPanel && displayindex < m_DisplayText.Count)
            {
                m_DisplayText[displayindex].gameObject.SetActive(true);
                m_DisplayText[displayindex].text = informationData.Contentlist[i].Name + ":" + informationData.Contentlist[i].Value;
                displayindex++;
            }
        }
    }
}
public class _LookAtCamera : MonoBehaviour
{
    public Transform CameraTran;
    void Update()
    {
        if (null == CameraTran)
            return;
        transform.LookAt(CameraTran, CameraTran.up);
    }
}
