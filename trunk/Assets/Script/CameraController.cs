using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CameraController : MonoBehaviour
{

    private static CameraController _instance;

    public static CameraController GetInstance()
    {
        return _instance;
    }

    [SerializeField]
    private float m_MoveSpeed = 5;

    private float m_MovementUpper = 29.5f;//摄像机位移限制
    private float m_MovementFloor = .5f;
    [SerializeField]
    private float m_MovementL = -30f;//西
    [SerializeField]
    private float m_MovementR = 30f;//东面
    [SerializeField]
    private float m_MovementF = 16f;//北面
    [SerializeField]
    private float m_MovementB = -19f;//南

    //方向灵敏度  
    public float sensitivityX = 10F;
    public float sensitivityY = 10F;

    //上下最大视角(Y视角)  
    public float minimumY = -60F;
    public float maximumY = 60F;

    float rotationY = 0F;

    public Vector3 Forward { get { return transform.forward; } }

    private Vector3 ORIGINAL_POS;
    private Quaternion ORIGINAL_ROT;

    private Vector3 m_vOriginalPos;
    private Quaternion m_vOriginalRot;

    private float m_fTargetAngle;
    private bool m_bIsRotating = false;

    private Camera m_MainCamera;
    public Camera MainCamera { get { return m_MainCamera; } }
    public Vector3 RotPoint = Vector3.zero;
    public bool LockMovtion = false;
    private Hashtable m_MoveArg = new Hashtable();
    private Hashtable m_RotaArg = new Hashtable();
    private bool m_bIsTopView;
    private Vector3 m_vMoveDirection = Vector3.zero;

    private Vector3 m_vNewPos;

    private void Awake()
    {
        if (null == _instance)
            _instance = this;
        m_MainCamera = transform.GetComponent<Camera>();

        ORIGINAL_POS = transform.position;
        ORIGINAL_ROT = transform.rotation;
    }

    internal void Record()
    {
        m_vOriginalPos = transform.position;
        m_vOriginalRot = transform.rotation;
    }

    protected virtual void Start()
    {
        Record();
        m_MoveArg.Add("islocal", false);
        m_MoveArg.Add("time", 1f);
        m_MoveArg.Add("easetype", iTween.EaseType.easeOutQuart);
        m_MoveArg.Add("position", Vector3.zero);

        m_RotaArg.Add("islocal", false);
        m_RotaArg.Add("time", 1f);
        m_RotaArg.Add("easetype", iTween.EaseType.easeOutQuart);
        m_RotaArg.Add("rotation", Vector3.zero);

        rotationY = transform.localEulerAngles.x;
    }

    void Update()
    {
        if (!m_bIsRotating)
        {
            if (m_fIdleWaitTimeIndex < IDLE_WAIT)
                m_fIdleWaitTimeIndex += Time.deltaTime;
            else
            {
                m_bIsRotating = true;
                m_fIdleWaitTimeIndex = 0;
            }
        }
        else
        {
            transform.RotateAround(RotPoint, Vector3.up, Time.deltaTime * -1);
        }
        MovingUpdate();
    }

    public void MoveTo(Vector3 pos)
    {
        m_MoveArg["position"] = pos;
        iTween.MoveTo(gameObject, m_MoveArg);
    }

    public void RotateTo(Quaternion rot)
    {
        //m_bIsRotating = true;
        m_RotaArg["rotation"] = rot.eulerAngles;
        iTween.RotateTo(gameObject, m_RotaArg);
        iTween.RotateBy(gameObject, m_RotaArg);
    }

    public void GetBack()
    {
        LockMovtion = false;
        RotPoint = Vector3.zero;
        MoveTo(m_vOriginalPos);
        RotateTo(m_vOriginalRot);
        m_bIsTopView = false;
    }

    const float IDLE_WAIT = 300;//无操作5分钟
    private float m_fIdleWaitTimeIndex = 0;//等待计时

    private void RefrshIdle()
    {
        m_bIsRotating = false;
        m_fIdleWaitTimeIndex = 0;
    }

    private void MovingUpdate()
    {
        if (!LockMovtion || m_bIsTopView)
        {
            m_vMoveDirection = Vector3.zero;
            m_vNewPos = transform.position;
            if (Input.GetKey(KeyCode.W))
                m_vMoveDirection.z = 1;
            if (Input.GetKey(KeyCode.A))
                m_vMoveDirection.x = -1;
            if (Input.GetKey(KeyCode.S))
                m_vMoveDirection.z = -1;
            if (Input.GetKey(KeyCode.D))
                m_vMoveDirection.x = 1;
            if (Input.GetKey(KeyCode.E))
                m_vMoveDirection.y = 1;
            if (Input.GetKey(KeyCode.Q))
                m_vMoveDirection.y = -1;

            if (m_vMoveDirection != Vector3.zero)
            {
                RefrshIdle();
                m_vMoveDirection.Normalize();
                m_vMoveDirection *= m_MoveSpeed * (Input.GetKey(KeyCode.LeftShift) ? 3 : 1);
                m_vNewPos = m_vNewPos + transform.rotation * m_vMoveDirection;
            }
            //if (Input.GetKey(KeyCode.E))
            //    m_vNewPos = m_vNewPos + Vector3.up * m_MoveSpeed;
            //if (Input.GetKey(KeyCode.Q))
            //    m_vNewPos = m_vNewPos + Vector3.down * m_MoveSpeed;
            if (m_vNewPos.y > m_MovementUpper)
                m_vNewPos.y = m_MovementUpper;
            if (m_vNewPos.y < m_MovementFloor)
                m_vNewPos.y = m_MovementFloor;

            if (m_vNewPos.z > m_MovementF)
                m_vNewPos.z = m_MovementF;
            if (m_vNewPos.z < m_MovementB)
                m_vNewPos.z = m_MovementB;

            if (m_vNewPos.x < m_MovementL)
                m_vNewPos.x = m_MovementL;
            if (m_vNewPos.x > m_MovementR)
                m_vNewPos.x = m_MovementR;
            transform.position = m_vNewPos;
        }

        if (Input.GetMouseButtonDown(1)) { Cursor.visible = false; }
        if (Input.GetMouseButtonUp(1)) { Cursor.visible = true; }



        if (Input.GetMouseButtonUp(0)) { RefrshIdle(); }
        if (Input.GetMouseButton(1))
        {
            RefrshIdle();
            //rotationY = -transform.localEulerAngles.x;

            //根据鼠标移动的快慢(增量), 获得相机左右旋转的角度(处理X)  
            float rotationX = transform.localEulerAngles.y + Input.GetAxis("Mouse X") * sensitivityX;

            //根据鼠标移动的快慢(增量), 获得相机上下旋转的角度(处理Y)  
            rotationY -= Input.GetAxis("Mouse Y") * sensitivityY;
            //角度限制. rotationY小于min,返回min. 大于max,返回max. 否则返回value   
            rotationY = Mathf.Clamp(rotationY, minimumY, maximumY);
            //总体设置一下相机角度  
            transform.localEulerAngles = new Vector3(rotationY, rotationX, 0);

        }
    }

    public void RotRight()
    {
        if (m_bIsTopView)
            return;
        m_fTargetAngle = -20;
        m_bIsRotating = true;
    }

    public void Rotleft()
    {
        if (m_bIsTopView)
            return;
        m_fTargetAngle = 20;
        m_bIsRotating = true;
    }

    public void StopRot()
    {

        m_bIsRotating = false;
    }

    public void TopView(Text label)
    {
        if (null != ItemManager.GetInstance().CurSelectStation)
        {
            ItemManager.GetInstance().CurSelectStation.CancelSelect();
            ItemManager.GetInstance().CurSelectStation = null;
            UIManager.GetInstance().ShowSimpleInfo();
            UIManager.GetInstance().HideDetalInfo();
        }

        if (m_bIsTopView)
        {
            m_bIsTopView = false;
            GetBack();
        }
        else
        {
            m_bIsTopView = true;
            m_bIsTopView = true;

            RotateTo(Quaternion.Euler(new Vector3(90, 0, 0)));
            MoveTo(new Vector3(0, m_MovementUpper, -1));
        }
        if (null != label)
            label.text = m_bIsTopView ? "俯视" : "顶视";
    }

    public void Restoration()
    {
        LockMovtion = false;
        m_bIsTopView = false;
        RotPoint = Vector3.zero;
        RotateTo(ORIGINAL_ROT);
        MoveTo(ORIGINAL_POS);
        m_vOriginalPos = ORIGINAL_POS;
        m_vOriginalRot = ORIGINAL_ROT;
        if (null != ItemManager.GetInstance().CurSelectStation)
        {
            ItemManager.GetInstance().CurSelectStation.CancelSelect();
            ItemManager.GetInstance().CurSelectStation = null;
            UIManager.GetInstance().ShowSimpleInfo();
            UIManager.GetInstance().HideDetalInfo();
        }
    }
}
