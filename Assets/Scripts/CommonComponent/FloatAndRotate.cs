using UnityEngine;


[DisallowMultipleComponent]
public class FloatAndRotate : MonoBehaviour
{
    [Header("Ư������")]
    [Tooltip("Ư�����ȣ�����ƫ������")]
    public float floatAmplitude = 0.5f;     
    [Tooltip("Ư��Ƶ�ʣ���/�룩")]
    public float floatFrequency = 1f;       

    [Header("��ת����")]
    [Tooltip("ÿ����ת���ٶȣ��ȣ�")]
    public Vector3 rotationSpeed = new Vector3(0f, 30f, 0f);

    private Vector3 m_startLocalPos;                 // ��ʼ�ֲ�����

    void Start()
    {
        m_startLocalPos = transform.localPosition;
    }

    void Update()
    {
        DoFloat();
        DoRotate();
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
