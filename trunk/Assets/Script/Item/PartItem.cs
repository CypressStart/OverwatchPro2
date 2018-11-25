using UnityEngine;
using System.Collections;
using System;

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

    protected override void OnMouseUp()
    {
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
