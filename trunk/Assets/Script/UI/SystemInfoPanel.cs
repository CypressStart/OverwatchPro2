using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SystemInfoPanel : MonoBehaviour {

    private const string STR_TIME = "{0}年{1}月{2}日 {3}:{4}:{5}";

    [SerializeField]
    private Text m_Text_Time;

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        if (null != m_Text_Time)
            m_Text_Time.text = string.Format(STR_TIME, System.DateTime.Now.Year
                , System.DateTime.Now.Month
                , System.DateTime.Now.Day
                , System.DateTime.Now.Hour
                , System.DateTime.Now.Minute
                , System.DateTime.Now.Second);
    }
}
