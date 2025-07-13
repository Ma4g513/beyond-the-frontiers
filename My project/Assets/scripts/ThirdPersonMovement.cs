using UnityEngine;
using System.Collections; // ← nécessaire pour IEnumerator

[RequireComponent(typeof(Rigidbody))]
public class ThirdPersonMovement : MonoBehaviour
{
    [Header("Références")]
    public Transform cameraTransform;
    public Transform model;

    [Header("Déplacements")]
    public float moveSpeed = 3f;
    public float jumpForce = 5f;
    public float groundCheckDistance = 0.2f;
    public LayerMask groundMask;

    public float Endurance = 5f;
    public float maxEndurance = 5f;
    public bool isSprinting = false;

    Rigidbody rb;
    private Coroutine staminaDrainCoroutine;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Start()
    {
        Endurance = maxEndurance;
    }

    void Update()
    {
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");
        Vector3 inputDir = new Vector3(h, 0, v).normalized;

        // Mouvement
        if (inputDir.magnitude >= 0.1f)
        {
            float targetAngle = Mathf.Atan2(inputDir.x, inputDir.z) * Mathf.Rad2Deg + cameraTransform.eulerAngles.y;
            Vector3 moveDir = Quaternion.Euler(0, targetAngle, 0) * Vector3.forward;

            Vector3 velocity = moveDir.normalized * moveSpeed;
            velocity.y = rb.velocity.y;
            rb.velocity = velocity;

            model.rotation = Quaternion.Euler(0, targetAngle, 0);
        }
        else
        {
            rb.velocity = new Vector3(0, rb.velocity.y, 0);
        }

        // Saut
        if (Input.GetButtonDown("Jump") && IsGrounded())
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }

        // Sprint
        bool wantsToSprint = Input.GetKey(KeyCode.LeftShift) && Endurance > 0f && inputDir.magnitude > 0.1f;

        if (wantsToSprint && !isSprinting)
        {
            isSprinting = true;
            moveSpeed = 7f;
            staminaDrainCoroutine = StartCoroutine(DrainEndurance());
        }
        else if (!wantsToSprint && isSprinting)
        {
            isSprinting = false;
            moveSpeed = 3f;
            if (staminaDrainCoroutine != null)
                StopCoroutine(staminaDrainCoroutine);
        }

        // Recharge d'endurance
        if (!isSprinting && Endurance < maxEndurance)
        {
            Endurance += 0.1f * Time.deltaTime * 5f;
            Endurance = Mathf.Min(Endurance, maxEndurance);
        }

        // DEBUG
        Debug.Log("Endurance: " + Endurance.ToString("F2"));
    }

    bool IsGrounded()
    {
        return Physics.Raycast(transform.position + Vector3.up * 0.1f,
                               Vector3.down,
                               groundCheckDistance + 0.1f,
                               groundMask);
    }

    IEnumerator DrainEndurance()
    {
        while (isSprinting && Endurance > 0f)
        {
            Endurance -= 0.1f;
            Endurance = Mathf.Max(Endurance, 0f);
            yield return new WaitForSeconds(0.1f);
        }
    }
}
