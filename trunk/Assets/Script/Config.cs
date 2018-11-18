using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 放在每个场景用来配置参数
/// </summary>
public class Config : MonoBehaviour
{
    [SerializeField]
    private TextAsset m_EquipmentDataFile;//设备信息
    [SerializeField]
    private TextAsset m_InformationListFile;//数据 用ID对应

    public TextAsset EquipmentDataFile { get { return m_EquipmentDataFile; } }
    public TextAsset InformationListFile { get { return m_InformationListFile; } }
    void Start()
    {

    }

    void Update()
    {

    }
}
