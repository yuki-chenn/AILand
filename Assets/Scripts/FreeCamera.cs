// csharp
using UnityEngine;

public class FreeCamera : MonoBehaviour
{
    [Header("移动设置")]
    public float moveSpeed = 10f;
    public float boostMultiplier = 3f;
    [Header("视角设置")]
    public float lookSpeed = 2f;
    public float minPitch = -90f;
    public float maxPitch = 90f;

    private float yaw = 0f;
    private float pitch = 0f;

    void Update()
    {
        HandleLook();
        HandleMove();
    }

    void HandleLook()
    {
        if (Input.GetMouseButton(1))
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;

            yaw += Input.GetAxis("Mouse X") * lookSpeed;
            pitch -= Input.GetAxis("Mouse Y") * lookSpeed;
            pitch = Mathf.Clamp(pitch, minPitch, maxPitch);

            transform.rotation = Quaternion.Euler(pitch, yaw, 0);
        }
        else
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }

    void HandleMove()
    {
        float speed = moveSpeed * (Input.GetKey(KeyCode.LeftShift) ? boostMultiplier : 1f);
        Vector3 dir = Vector3.zero;

        dir += transform.right * Input.GetAxis("Horizontal");   // A/D
        dir += transform.forward * Input.GetAxis("Vertical");   // W/S
        if (Input.GetKey(KeyCode.E)) dir += Vector3.up;        // 上
        if (Input.GetKey(KeyCode.Q)) dir -= Vector3.up;        // 下

        transform.position += dir * speed * Time.deltaTime;
    }
}