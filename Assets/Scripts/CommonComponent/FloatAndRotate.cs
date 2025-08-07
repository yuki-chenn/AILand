using System;
using UnityEngine;


[DisallowMultipleComponent]
public class FloatAndRotate : MonoBehaviour
{
    public bool enable = false;
    
    
    [Header("Ư������")]
    [Tooltip("Ư������")]
    public float floatAmplitude = 0.5f;     
    [Tooltip("Ư��Ƶ��")]
    public float floatFrequency = 1f;       

    [Header("��ת����")]
    [Tooltip("��ת���ٶ�")]
    public Vector3 rotationSpeed = new Vector3(0f, 30f, 0f);
    
    private Vector3 m_startLocalPos;                 // ��ʼ�ֲ�����

    
    
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
