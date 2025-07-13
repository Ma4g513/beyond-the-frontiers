using UnityEngine;

public class ThirdPersonCamera : MonoBehaviour
{
    [Header("Références")]
    public Transform target;

    [Header("Réglages caméra")]
    public Vector3 offset = new Vector3(0f, 2f, -4f);
    public float rotateSpeed = 5f;
    public float minPitch = -35f;
    public float maxPitch = 60f;

    float yaw;
    float pitch;

    void LateUpdate()
    {
        float mouseX = Input.GetAxis("Mouse X") * rotateSpeed;
        float mouseY = Input.GetAxis("Mouse Y") * rotateSpeed;

        yaw += mouseX;
        pitch -= mouseY;
        pitch = Mathf.Clamp(pitch, minPitch, maxPitch);

        Quaternion rotation = Quaternion.Euler(pitch, yaw, 0);
        Vector3 desiredPosition = target.position + rotation * offset;

        transform.position = desiredPosition;
        transform.LookAt(target.position + Vector3.up * 1.5f);
    }
}
