using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine.SceneManagement;

/// <summary>
/// 单位管理
/// </summary>
public class ItemManager
{
    private static ItemManager _instance;

    public static ItemManager GetInstance()
    {
        if (null == _instance)
            _instance = new ItemManager();
        return _instance;
    }

    const string PATH_CABINET_PREFAB = "Prefab/Cabinet";
    const string PATH_CAMERA_PREFAB = "Prefab/Camera";

    private List<StationItem> m_lStationItemList = new List<StationItem>();
    private List<PartItem> m_lCameraItemList = new List<PartItem>();
    private List<PartItem> m_lCabinetItemList = new List<PartItem>();

    private List<PartItem> m_lCameraCache = new List<PartItem>();
    private List<PartItem> m_lCabinetCache = new List<PartItem>();

    public List<PartItem> m_PublicItemList = new List<PartItem>();//公用添加物体 公共摄像机后边和这里合并

    private bool m_bIsSortedStation = false;

    public StationItem CurSelectStation { get; internal set; }

    public void AddStation(StationItem item)
    {
        if (m_lStationItemList.Contains(item))
        {
            Debug.LogError("contains item!");
            return;
        }
        m_lStationItemList.Add(item);
    }

    public void Release()
    {
        m_lStationItemList.Clear();
        m_lCameraItemList.Clear();
        m_lCabinetItemList.Clear();
        m_lCameraCache.Clear();
        m_lCabinetCache.Clear();
        m_PublicItemList.Clear();
    }

    /// <summary>
    /// 一一对应
    /// </summary>
    internal void RefreshStation(List<StationData> datas)
    {
        for (int i = 0; i < datas.Count; i++)
        {
            var item = GetStationItem(datas[i].ID);
            if (null != item)
            {
                item.Refresh(datas[i]);
            }
        }
    }

    public StationItem GetStationItem(string id)
    {
        StationItem item = null;
        for (int i = 0; i < m_lStationItemList.Count; i++)
        {
            if (m_lStationItemList[i].Index.ToString() == id)
                item = m_lStationItemList[i];
        }
        return item;
    }

    internal void RefreshCabinet(List<PartData> datas)
    {
        m_lCabinetCache.AddRange(m_lCabinetItemList);
        m_lCabinetItemList.Clear();

        for (int i = 0; i < datas.Count; i++)
        {
            var item = GetCabinetItem();
            item.Refresh(datas[i]);
        }
        for (int i = 0; i < m_lCabinetCache.Count; i++)
        {
            m_lCabinetCache[i].SetState(false);
        }
    }

    internal void RefreshCamera(List<PartData> datas)
    {
        m_lCameraCache.AddRange(m_lCameraItemList);
        m_lCameraItemList.Clear();

        for (int i = 0; i < datas.Count; i++)
        {
            var item = GetCameraItem();
            item.Refresh(datas[i]);
        }
        for (int i = 0; i < m_lCameraCache.Count; i++)
        {
            m_lCameraCache[i].SetState(false);
        }
    }

    public List<PartItem> GetCabinetById(string ID)
    {
        var list = new List<PartItem>();

        for (int i = 0; i < m_lCabinetItemList.Count; i++)
        {
            if (m_lCabinetItemList[i].ID == ID)
                list.Add(m_lCabinetItemList[i]);
        }

        return list;
    }

    public List<PartItem> GetCameraById(string ID)
    {
        var list = new List<PartItem>();

        for (int i = 0; i < m_lCameraItemList.Count; i++)
        {
            if (m_lCameraItemList[i].ID == ID)
                list.Add(m_lCameraItemList[i]);
        }

        return list;
    }

    private PartItem GetCabinetItem()
    {
        PartItem item = null;
        if (m_lCabinetCache.Count > 0)
        {
            item = m_lCabinetCache[0];
            m_lCabinetCache.Remove(item);
        }
        else
        {
            var obj = GameObject.Instantiate(DataManager.GetInstance().CabinetObj) as GameObject;
            item = obj.GetComponent<PartItem>();
        }

        m_lCabinetItemList.Add(item);
        item.SetState(true);
        return item;
    }

    private PartItem GetCameraItem()
    {
        PartItem item = null;
        if (m_lCameraCache.Count > 0)
        {
            item = m_lCameraCache[0];
            m_lCameraCache.Remove(item);
        }
        else
        {
            var obj = GameObject.Instantiate(DataManager.GetInstance().CameraObj) as GameObject;
            item = obj.GetComponent<PartItem>();
        }

        m_lCameraItemList.Add(item);
        item.SetState(true);
        return item;
    }

    private void SortStation()
    {
        for (int i = 0; i < m_lStationItemList.Count; i++)
        {
            if (i > 0 && m_lStationItemList[i].Index < m_lStationItemList[i - 1].Index)
            {
                var item = m_lStationItemList[i];
                m_lStationItemList[i] = m_lStationItemList[i - 1];
                m_lStationItemList[i - 1] = item;
                i = 0;
            }

        }
    }

    internal void RefreshPublicCamera(List<PartData> dataList)
    {
        var group = DataManager.GetInstance().PublicCameraList;
        for (int i = 0; i < dataList.Count; i++)
        {
            var data = dataList[i];
            var obj = group.transform.Find(data.ID);
            if (null == obj)
                continue;

            var item = obj.GetComponent<PartItem>();
            if (null == item)
                continue;
            item.IsPublicCamera = true;
            item.Refresh(data);
        }
    }

    /// <summary>
    /// 加载场内动态物体
    /// </summary>
    /// <param name="m_EquipmentDataList"></param>
    internal void LoadEquipment(List<PartData> equipDataList)
    {
        if (null == equipDataList || equipDataList.Count <= 0)
            return;
        for (int i = 0; i < equipDataList.Count; i++)
        {
            var item = GetPublicItem(equipDataList[i].ID);
            if (null == item)//创建新物体
            {
                var res = Resources.Load(DataManager.GetInstance().GetPath(equipDataList[i].ItemType)) as GameObject;
                if (null == res)
                    continue;
                var obj = GameObject.Instantiate(res);
                Debug.LogError("..........." + DataManager.GetInstance().GetPath(equipDataList[i].ItemType));

                if (null == obj)
                    obj = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                item = obj.AddComponent<PartItem>();
                item.Refresh(equipDataList[i]);
                m_PublicItemList.Add(item);
            }
            else if (item.ItemType != equipDataList[i].ItemType)//类型不一致换模型
            {
            }
            item.Refresh(equipDataList[i]);
        }
    }

    private PartItem GetPublicItem(string id)
    {
        for (int i = 0; i < m_PublicItemList.Count; i++)
        {
            if (m_PublicItemList[i].ID == id)
                return m_PublicItemList[i];
        }
        return null;
    }
}
