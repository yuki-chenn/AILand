using System;
using UnityEngine;


[DisallowMultipleComponent]
public class FloatAndRotate : MonoBehaviour
{
    public bool enable = false;
    
    
    [Header("漂浮参数")]
    [Tooltip("漂浮幅度")]
    public float floatAmplitude = 0.5f;     
    [Tooltip("漂浮频率")]
    public float floatFrequency = 1f;       

    [Header("旋转参数")]
    [Tooltip("自转角速度")]
    public Vector3 rotationSpeed = new Vector3(0f, 30f, 0f);
    
    private Vector3 m_startLocalPos;                 // 初始局部坐标

    
    
    void Start()
    {
        m_startLocalPos = transform.localPosition;
    }

    void Update()
    {
        if (!enable) return;
        DoFloat();
        DoRotate();
    }

    private void OnDisable()
    {
        m_startLocalPos = Vector3.zero;
    }

    private void OnEnable()
    {
        m_startLocalPos = transform.localPosition;
    }

    void DoFloat()
    {
        float offset = Mathf.Sin(Time.time * Mathf.PI * 2f * floatFrequency) * floatAmplitude;
        Vector3 pos = m_startLocalPos;
        pos.y += offset;
        transform.localPosition = pos;
    }

    void DoRotate()
    {
        transform.Rotate(rotationSpeed * Time.deltaTime, Space.Self);
    }
}
