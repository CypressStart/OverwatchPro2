using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

/// <summary>
/// Data instance
/// read for txt or xml
/// </summary>
public class DataManager : MonoBehaviour
{
    private static DataManager _instance;

    public static DataManager GetInstance()
    {
        return _instance;
    }

    private void Awake()
    {
        if (null != _instance)
        {
            DestroyImmediate(gameObject);
            return;
        }
        DontDestroyOnLoad(this);
        _instance = this;
    }

    private const string DATA_PATH_KEY = "DATA_PATH";
    private const string DATA_FILENAME = "DATA.txt";

    private const string DATA_FILENAME_STATION = "StationData.txt";
    private const string DATA_FILENAME_EQUIP = "EquipmentData.txt";
    private const string DATA_FILENAME_INFO = "InformationList.txt";
    private const string DATA_FILENAME_CONFIG = "Config.txt";

    private const string DATA_Type_STATION = "STATION";
    private const string DATA_Type_CAMERA = "CAMERA";
    private const string DATA_Type_CABINET = "CABINET";
    private const string DATA_Type_MAN = "NAMELIST";
    private const string DATA_TYPE_PUBLIC_CAMERA = "PUBLIC_CAMERA";

    private const string TEST_Data_Path = "file:///F:/Project/OverwatchPro2/Data/";

    private const string DATA_Type_Scene_Message_Panel = "SCENE_MESSAGE_PANEL";

    private const float UPDATE_TIME = 30;//30S更新一次

    public GameObject CameraObj;
    public GameObject CabinetObj;
    public GameObject PublicCameraList;

    private List<PartData> m_EquipmentDataList = new List<PartData>();
    private List<InformationData> m_InformationDataList = new List<InformationData>();

    public string DataPath { get { return m_strDataPath; } }
    private string m_strDataPath = "file:/F:/Data";
    private List<string> m_DataStrList = new List<string>();
    private List<StationData> m_StationDataList = new List<StationData>();
    private List<PartData> m_CameraDataList = new List<PartData>();
    private List<PartData> m_CabinetDataList = new List<PartData>();
    private List<PartData> m_PublicCameraDataList = new List<PartData>();//公共色相头
    private Dictionary<string, List<string>> m_ManData = new Dictionary<string, List<string>>();

    private List<ModelData> m_ModelConfigList = null;

    private bool m_bIsInit = false;
    private float m_fTimeIndex;

    private string m_strCurSceneDataPath;//每个场景切换不同给的地址

    public void Init(string m_SceneDataPath)
    {
        //取预存路径
        if (PlayerPrefs.HasKey(DATA_PATH_KEY))
            m_strDataPath = PlayerPrefs.GetString(DATA_PATH_KEY);
        m_strCurSceneDataPath = m_SceneDataPath;
        m_bIsInit = true;
    }

    public void Tick(float deltaTime)
    {
        if (m_fTimeIndex < UPDATE_TIME)
        {
            m_fTimeIndex += deltaTime;
        }
        else
        {
            m_fTimeIndex = 0;
            Load();
        }
    }

    public void SaveDataPath(string path)
    {
        m_strDataPath = path;
        PlayerPrefs.SetString(DATA_PATH_KEY, path);
    }

    public void Load()
    {
        if (!m_bIsInit)
        {
            Debug.LogError("DataManager is not init!");
            return;
        }
        Debug.Log("加载数据");
        if (null == m_ModelConfigList)
            StartCoroutine(LoadConfig());

        StartCoroutine(LoadStation());
        StartCoroutine(LoadEquipmentData());
        StartCoroutine(LoadInformationList());
    }

    /// <summary>
    /// 读取文本文件
    /// </summary>
    /// <param name="path">读取文件的路径</param>
    /// <param name="name">读取文件的名称</param>
    /// <returns></returns>
    private List<string> LoadFile(string path, string name)
    {
        //使用流的形式读取
        StreamReader sr = null;
        try
        {
            sr = File.OpenText(path + "//" + name);
        }
        catch (Exception e)
        {
            //路径与名称未找到文件则直接返回空
            return null;
        }
        string line;
        var arrlist = new List<string>();
        while ((line = sr.ReadLine()) != null)
        {
            //一行一行的读取
            //将每一行的内容存入数组链表容器中
            arrlist.Add(line);
        }
        //关闭流
        sr.Close();
        //销毁流
        sr.Dispose();
        //将数组链表容器返回
        return arrlist;
    }

    private IEnumerator LoadStation()
    {
        var path = TEST_Data_Path + m_strCurSceneDataPath + "/" + DATA_FILENAME_STATION;
        //var www = new WWW(path);

        var www = new WWW(TEST_Data_Path + m_strCurSceneDataPath + "/" + DATA_FILENAME_STATION);
        yield return www;
        string strAll = System.Text.Encoding.UTF8.GetString(www.bytes);
        if (string.IsNullOrEmpty(strAll))
        {
            //如果无法从网络读取  从本地读测试数据
            Debug.LogError("无法从" + path + " 获取数据,将加载测试数据." + "WWWerror:" + www.error);
        }
        string[] sr = strAll.Split(new string[] { "\r\n" }, System.StringSplitOptions.None);

        m_fTimeIndex = 0;
        //m_DataStrList = LoadFile(m_strDataPath, DATA_FILENAME);
        m_DataStrList.Clear();
        m_DataStrList.AddRange(sr);
        if (null == m_DataStrList || m_DataStrList.Count <= 0)
            yield return 0;
        //解析数据
        m_StationDataList.Clear();
        m_CameraDataList.Clear();
        m_CabinetDataList.Clear();
        m_PublicCameraDataList.Clear();
        for (int i = 0; i < m_DataStrList.Count; i++)
        {
            AdaptiveStation(m_DataStrList[i].Split(';'));
        }

        ItemManager.GetInstance().RefreshCamera(m_CameraDataList);
        ItemManager.GetInstance().RefreshPublicCamera(m_PublicCameraDataList);
        ItemManager.GetInstance().RefreshCabinet(m_CabinetDataList);

        ItemManager.GetInstance().RefreshStation(m_StationDataList);
    }

    /// <summary>
    /// 加载物体配置
    /// </summary>
    /// <returns></returns>
    private IEnumerator LoadEquipmentData()
    {
        var path = TEST_Data_Path + m_strCurSceneDataPath + "/" + DATA_FILENAME_EQUIP;
        //var www = new WWW(path);
        var www = new WWW(TEST_Data_Path + m_strCurSceneDataPath + "/" + DATA_FILENAME_EQUIP);

        yield return www;
        string strAll = System.Text.Encoding.UTF8.GetString(www.bytes);
        if (string.IsNullOrEmpty(strAll))
        {
            //如果无法从网络读取  从本地读测试数据
            Debug.LogError("无法从" + path + " 获取数据,将加载测试数据." + "WWWerror:" + www.error);
        }
        string[] sr = strAll.Split(new string[] { "\r\n" }, System.StringSplitOptions.None);

        m_fTimeIndex = 0;
        if (null == sr || sr.Length <= 0)
            yield return 0;
        //解析数据
        m_EquipmentDataList.Clear();
        for (int i = 0; i < sr.Length; i++)
        {
            AdaptiveEquipmentData(sr[i].Split(';'));
        }
        //加载物体
        if (m_EquipmentDataList.Count > 0)
            ItemManager.GetInstance().LoadEquipment(m_EquipmentDataList);
    }

    /// <summary>
    /// 加载信息
    /// </summary>
    /// <returns></returns>
    private IEnumerator LoadInformationList()
    {
        var path = TEST_Data_Path + m_strCurSceneDataPath + "/" + DATA_FILENAME_INFO;
        //var www = new WWW(path);
        var www = new WWW(path);
        yield return www;
        string strAll = System.Text.Encoding.UTF8.GetString(www.bytes);
        if (string.IsNullOrEmpty(strAll))
        {
            //如果无法从网络读取  从本地读测试数据
            Debug.LogError("无法从" + path + " 获取数据,将加载测试数据." + "WWWerror:" + www.error);
        }
        string[] sr = strAll.Split(new string[] { "\r\n" }, System.StringSplitOptions.None);

        m_fTimeIndex = 0;
        if (null == sr || sr.Length <= 0)
            yield return 0;
        //解析数据
        m_InformationDataList.Clear();
        for (int i = 0; i < sr.Length; i++)
        {
            AdaptiveInfomationData(sr[i].Split(';'));
        }
        //显示UI
        //TODO:根据m_EquipmentDataList 找ID添加UI信息
        if (m_InformationDataList.Count > 0)
            ItemManager.GetInstance().LoadInformation(m_InformationDataList);
    }

    /// <summary>
    /// 加载配置
    /// </summary>
    /// <returns></returns>
    private IEnumerator LoadConfig()
    {
        var path = TEST_Data_Path + DATA_FILENAME_CONFIG;
        //var www = new WWW(path);
        var www = new WWW(path);
        yield return www;
        string strAll = System.Text.Encoding.UTF8.GetString(www.bytes);
        if (string.IsNullOrEmpty(strAll))
        {
            //如果无法从网络读取  从本地读测试数据
            Debug.LogError("无法从" + path + " 获取数据,将加载测试数据." + "WWWerror:" + www.error);
        }
        string[] sr = strAll.Split(new string[] { "\r\n" }, System.StringSplitOptions.None);

        m_fTimeIndex = 0;
        if (null == sr || sr.Length <= 0)
            yield return 0;
        m_ModelConfigList = new List<ModelData>();
        //解析数据
        for (int i = 0; i < sr.Length; i++)
        {
            if (string.IsNullOrEmpty(sr[i]))
                continue;
            var _sr = sr[i].Split('@');
            if (_sr.Length <= 1)
                continue;
            m_ModelConfigList.Add(new ModelData() { Name = _sr[0], Path = _sr[1] });
        }
    }

    private void AdaptiveStation(string[] strArr)
    {
        if (null == strArr || strArr.Length <= 0)
            return;
        switch (strArr[0])
        {
            case DATA_Type_STATION:
                {
                    if (strArr.Length < 12)
                    {
                        Debug.LogError("工位数据长度不够");
                        return;
                    }
                    var data = new StationData
                    {
                        ID = strArr[1],
                        Version = strArr[2],
                        Node = strArr[3],
                        Rate = strArr[4],
                        StateType = int.Parse(strArr[5]),
                        Leader = strArr[6],
                        Date = strArr[7],
                        MissionType = strArr[8],
                        Style = strArr[9],
                        RotateIndex = int.Parse(strArr[10]),
                        Link = strArr[11]
                    };
                    m_StationDataList.Add(data);
                }
                break;

            case DATA_Type_CAMERA:
                {
                    if (strArr.Length < 7)
                    {
                        Debug.LogError("摄像机数据长度不够");
                        return;
                    }
                    var data = new PartData();
                    data.ID = strArr[1];
                    float x, y, z, r = 0;
                    float.TryParse(strArr[2], out x);
                    float.TryParse(strArr[3], out y);
                    float.TryParse(strArr[4], out z);
                    float.TryParse(strArr[5], out r);
                    data.Pos = new Vector3(x, y, z);
                    data.Rot = Quaternion.Euler(0, r, 0);
                    data.Link = strArr[6];
                    m_CameraDataList.Add(data);
                }
                break;

            case DATA_Type_CABINET:
                {
                    if (strArr.Length < 7)
                    {
                        Debug.LogError("机柜数据长度不够");
                        return;
                    }
                    var data = new PartData();
                    data.ID = strArr[1];
                    float x, y, z, r = 0;
                    float.TryParse(strArr[2], out x);
                    float.TryParse(strArr[3], out y);
                    float.TryParse(strArr[4], out z);
                    float.TryParse(strArr[5], out r);
                    data.Pos = new Vector3(x, 0, z);
                    data.Rot = Quaternion.Euler(0, r, 0);
                    data.Name = strArr[6];
                    data.Link = strArr[7];
                    m_CabinetDataList.Add(data);
                }
                break;

            case DATA_TYPE_PUBLIC_CAMERA:
                {
                    if (strArr.Length < 2)
                    {
                        Debug.LogError("公共 摄像机数据长度不够");
                        return;
                    }
                    var data = new PartData();
                    data.ID = strArr[1];
                    data.Link = strArr[2];
                    m_PublicCameraDataList.Add(data);
                }
                break;

            case DATA_Type_MAN:
                {
                    string id = strArr[1];
                    var nameArr = strArr[2].Split(',');
                    if (!m_ManData.ContainsKey(id))
                        m_ManData.Add(id, new List<string>(nameArr));
                }
                break;

            case DATA_Type_Scene_Message_Panel:
                {
                    string content = strArr[1];
                    content = content.Replace('^', '\n');
                    if (null != UIManager.GetInstance())
                        UIManager.GetInstance().SetSceneMessagePanel(content);
                }
                break;
        }
    }

    private void AdaptiveEquipmentData(string[] strArr)
    {
        if (null == strArr || strArr.Length <= 0)
            return;
        var data = new PartData();
        for (int i = 0; i < strArr.Length; i++)
        {
            if (string.IsNullOrEmpty(strArr[i]))
                continue;
            var optionArr = strArr[i].Split('@');//长度必定为2
            if (null == optionArr || optionArr.Length < 2)
                continue;
            switch (optionArr[0])
            {
                case "ID":
                    data.ID = optionArr[1];
                    //Debug.LogError(optionArr[0] + "/" + optionArr[1]);
                    break;

                case "ROT": data.Rot = Quaternion.Euler(0, float.Parse(optionArr[1]), 0); break;
                case "TYPE": data.ItemType = optionArr[1]; break;
                case "X": data.Pos.x = float.Parse(optionArr[1]); break;
                case "Y": data.Pos.y = float.Parse(optionArr[1]); break;
                case "Z": data.Pos.z = float.Parse(optionArr[1]); break;
            }
            //Debug.LogError(data.ID + "/" + data.Pos + "/" + data.ItemType);
        }
        m_EquipmentDataList.Add(data);
    }

    private void AdaptiveInfomationData(string[] strArr)
    {
        if (null == strArr || strArr.Length <= 0)
            return;
        var data = new InformationData();
        data.Contentlist = new List<InfoContent>();
        for (int i = 0; i < strArr.Length; i++)
        {
            if (string.IsNullOrEmpty(strArr[i]))
                continue;
            var optionArr = strArr[i].Split('@');//长度至少为2
            if (null == optionArr || optionArr.Length < 2)
                continue;
            switch (optionArr[0])
            {
                case "ID": data.ID = optionArr[1]; break;
                case "Link": data.Link = optionArr[1]; break;
                default:
                    {
                        if (optionArr.Length > 2)
                            data.Contentlist.Add(new InfoContent
                            {
                                Name = optionArr[0],
                                Value = optionArr[1],
                                ViewType = (EContentViewType)int.Parse(optionArr[2])
                            });
                    }
                    break;
            }
        }
        m_InformationDataList.Add(data);
    }

    public string GetPath(string name)
    {
        for (int i = 0; i < m_ModelConfigList.Count; i++)
        {
            if (m_ModelConfigList[i].Name == name)
                return m_ModelConfigList[i].Path;
        }
        return string.Empty;
    }
}

public class InformationData
{
    public string ID;
    public string Link;
    public List<InfoContent> Contentlist;
}

public class InfoContent
{
    public string Name;
    public string Value;
    public EContentViewType ViewType;
}

public enum EContentViewType
{
    E_Hide = 0,//默认显示
    E_Show = 1,//默认隐藏
    E_Other,
}

//工位 机构 type/id/version/node/rate/state/leader/date/link
public struct StationData
{
    public string ID;
    public string Version;
    public string Node;
    public string Rate;
    private int m_nStateType;

    public int StateType
    {
        get { return m_nStateType; }
        set
        {
            m_nStateType = value;
            switch (m_nStateType)
            {
                case 0: State = "正常"; break;
                case 1: State = "异常"; break;
                case 2: State = "暂停"; break;
                case 3: State = "空闲"; break;
            }
        }
    } //0正常 1异常 2暂停 3空闲

    public string State;//
    public string Leader;
    public string Date;
    public string Link;
    public string MissionType;//1中型任务 2大型任务

    public string MissionTypeName
    {
        get
        {
            switch (MissionType)
            {
                case "1": return "中型任务";
                case "2": return "大型任务";
            }
            return "无任务";
        }
    }

    public string Style;//1双排2半包围3单排
    public int RotateIndex;
}

//摄像机/机柜 结构 type/id/x/y/z/r/name/link
public struct PartData
{
    public string ID;//ID去找各自工位
    public Vector3 Pos;
    public Quaternion Rot;//角度
    public string Link;
    public string Name;//机柜有
    public string ItemType;
}

public struct ModelData
{
    public string Name;
    public string Path;
}
