using UnityEngine;

public class ThirdPersonCamera : MonoBehaviour
{
    [Header("Références")]
    public Transform target;                 // joueur à suivre

    [Header("Réglages caméra")]
    public Vector3 offset = new Vector3(0, 2, -4);
    public float rotateSpeed = 5f;
    public float minPitch = -35f;            // angle le plus bas (regarde vers le haut)
    public float maxPitch = 60f;             // angle le plus haut (regarde vers le bas)

    float yaw;   // rotation horizontale (Y)
    float pitch; // rotation verticale   (X)

    void LateUpdate()
    {
        // -- Entrées souris --
        float mouseX = Input.GetAxis("Mouse X") * rotateSpeed;
        float mouseY = Input.GetAxis("Mouse Y") * rotateSpeed;

        yaw   += mouseX;
        pitch -= mouseY;                     // inversé pour un feeling FPS
        pitch  = Mathf.Clamp(pitch, minPitch, maxPitch);

        // -- Calcul position/rotation caméra --
        Quaternion rotation = Quaternion.Euler(pitch, yaw, 0);
        Vector3 desiredPos = target.position + rotation * offset;

        transform.position = desiredPos;
        transform.LookAt(target.position + Vector3.up * 1.5f);
    }
}
