using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using System;

public class UIManager : MonoBehaviour
{
    private static UIManager _instance;

    public static UIManager GetInstance()
    {
        return _instance;
    }
    public Camera UICamera { get { return m_UICamera; } }

    [SerializeField]
    private Camera m_UICamera;
    [SerializeField]
    private GameObject m_SimpleInfoItem;
    [SerializeField]
    private RectTransform m_DetailInfo;
    [SerializeField]
    private Text m_Detail_Version;
    [SerializeField]
    private Text m_Detail_Node;
    [SerializeField]
    private Text m_Detail_Rate;
    [SerializeField]
    private Text m_Detail_State;
    [SerializeField]
    private Text m_Detail_Leader;
    [SerializeField]
    private Text m_Detail_Date;
    [SerializeField]
    private Text m_Detail_Type;
    [SerializeField]
    private Button m_Detail_Goto;
    [SerializeField]
    private Button m_Detail_Back;
    [SerializeField]
    private Transform m_SimpleInfoGroup;
    [SerializeField]
    private GameObject m_EditWindows;
    [SerializeField]
    private InputField m_EditInput;
    [SerializeField]
    private GameObject m_TopViewBtn;
    [SerializeField]
    private Transform m_StateIcon;
    [SerializeField]
    private Text m_Text_Message;
    [SerializeField]
    private Text m_Text_Time;
    [SerializeField]
    private TweenPlayer m_ImgAddOn;

    private const string STR_TIME = "{0}年{1}月{2}日 {3}:{4}:{5}";

    public void DebugText(string str)
    {

    }

    private List<SimpleInfoUIItem> m_SimpleInfoUIItemList = new List<SimpleInfoUIItem>();
    private float m_fDetailInfoWidth = 195;
    private float m_fDetailInfoHight = 175;
    private Vector2 m_vDetailPanelSize = new Vector2(214, 246);
    private bool m_bIsPlayPanelAnim;
    private bool m_bInfoToShow = false;
    private bool m_bEditWindowsShow = false;

    private void Awake()
    {
        if (null == _instance)
            _instance = this;
    }

    void Start()
    {
        if (null != m_EditWindows)
            m_EditWindows.SetActive(false);
        m_DetailInfo.sizeDelta = Vector2.zero;
        m_Detail_Back.onClick.AddListener(() =>
        {
            if (null != ItemManager.GetInstance().CurSelectStation)
                ItemManager.GetInstance().CurSelectStation.CancelSelect();
            ItemManager.GetInstance().CurSelectStation = null;
            CameraController.GetInstance().GetBack();
            UIManager.GetInstance().ShowSimpleInfo();
            UIManager.GetInstance().HideDetalInfo();
        });
    }

    void Update()
    {
        m_Text_Time.text = string.Format(STR_TIME, System.DateTime.Now.Year
            , System.DateTime.Now.Month
            , System.DateTime.Now.Day
            , System.DateTime.Now.Hour
            , System.DateTime.Now.Minute
            , System.DateTime.Now.Second);

        if (m_bIsPlayPanelAnim && null != m_DetailInfo)
        {
            if (m_bInfoToShow)
            {
                m_DetailInfo.sizeDelta = Vector2.MoveTowards(m_DetailInfo.sizeDelta, m_vDetailPanelSize, 20);
                if (Vector2.Distance(m_DetailInfo.sizeDelta, m_vDetailPanelSize) <= Vector2.kEpsilon)
                    m_bIsPlayPanelAnim = false;
            }
            else
            {
                m_DetailInfo.sizeDelta = Vector2.MoveTowards(m_DetailInfo.sizeDelta, Vector2.zero, 20);
                if (Vector2.Distance(m_DetailInfo.sizeDelta, Vector2.zero) <= Vector2.kEpsilon)
                    m_bIsPlayPanelAnim = false;
            }
        }

        if (Input.GetKeyDown(KeyCode.Home))
        {
            m_bEditWindowsShow = !m_bEditWindowsShow;
            m_EditWindows.SetActive(m_bEditWindowsShow);
            if (m_bEditWindowsShow)
            {
                m_EditInput.text = DataManager.GetInstance().DataPath;
            }
        }
    }

    public void CloseEditWindows()
    {
        m_bEditWindowsShow = false;
        m_EditWindows.SetActive(false);
    }

    public void SaveDataPath()
    {
        DataManager.GetInstance().SaveDataPath(m_EditInput.text);
    }

    public SimpleInfoUIItem AddSimpleInfo()
    {
        if (null == m_SimpleInfoItem)
            return null;
        var obj = GameObject.Instantiate(m_SimpleInfoItem) as GameObject;
        obj.transform.SetParent(m_SimpleInfoGroup, false);
        var item = obj.GetComponent<SimpleInfoUIItem>();
        m_SimpleInfoUIItemList.Add(item);
        return item;
    }

    public void ShowSimpleInfo()
    {
        for (int i = 0; i < m_SimpleInfoUIItemList.Count; i++)
        {
            m_SimpleInfoUIItemList[i].Show();
        }
        m_TopViewBtn.gameObject.SetActive(true);
    }

    public void HideSimpleInfo()
    {
        for (int i = 0; i < m_SimpleInfoUIItemList.Count; i++)
        {
            m_SimpleInfoUIItemList[i].Hide();
        }
        m_TopViewBtn.gameObject.SetActive(false);
    }

    public void ShowDetalInfo(StationData stationData)
    {
        if (null == m_DetailInfo)
            return;

        m_Detail_Version.text = stationData.Version;
        m_Detail_Node.text = stationData.Node;
        m_Detail_Rate.text = stationData.Rate + "%";
        m_Detail_State.text = stationData.State;
        m_Detail_Leader.text = stationData.Leader;
        m_Detail_Date.text = stationData.Date;
        m_Detail_Type.text = stationData.MissionTypeName;
        if (null != m_StateIcon)
        {
            for (int i = 0; i < m_StateIcon.childCount; i++)
            {
                var child = m_StateIcon.GetChild(i).gameObject;
                child.SetActive(child.name == stationData.StateType.ToString());
            }
        }
        m_bInfoToShow = true;
        m_bIsPlayPanelAnim = true;
    }

    public void HideDetalInfo()
    {
        if (null == m_DetailInfo)
            return;
        m_bInfoToShow = false;
        m_bIsPlayPanelAnim = true;
    }

    internal void SetSceneMessagePanel(string content)
    {
        m_Text_Message.text = content;
    }

    public void ShowImgAddOn()
    {
        if (Vector3.zero == m_ImgAddOn.transform.localScale)
            TweenPlayer.PlayTween(m_ImgAddOn.gameObject);
    }

    public void OnImgAddOnPlayComplete()
    {
        m_ImgAddOn.Stop();
        if (m_ImgAddOn.Amount == Vector3.zero)
        {
            m_ImgAddOn.Amount = Vector3.one;
            m_ImgAddOn.Delay = 0;
        }
        else
        {
            m_ImgAddOn.Amount = Vector3.zero;
            m_ImgAddOn.Delay = 3;
            TweenPlayer.PlayTween(m_ImgAddOn.gameObject);
        }
    }
}
