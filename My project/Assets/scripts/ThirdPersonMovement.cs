using UnityEngine;
using System.Collections; // ← important pour les coroutines !

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

    public bool enduranceIs0 = false;
    public float maxEndurance = 5f;

    public bool isSprinting = false;

    Rigidbody rb;
    bool isGrounded;

    private Coroutine staminaDrainCoroutine; // ← nouvelle variable

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Start()
    {
        maxEndurance = 5f;
    }

    void Update()
    {
        print(Endurance);

        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");
        Vector3 inputDir = new Vector3(h, 0, v).normalized;
        if (Endurance == 0)
        {
            enduranceIs0 = true;
            moveSpeed = 1f;
        }

        if (Endurance == maxEndurance)
        {
            moveSpeed = 3f;
        }

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

        if (Input.GetButtonDown("Jump") && IsGrounded())
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }

        if (Input.GetKey(KeyCode.LeftShift) && Endurance > 0f && inputDir.magnitude > 0.1f)
        {
            enduranceIs0 = false;
            if (!isSprinting)
            {
                isSprinting = true;
                moveSpeed = 7f;
                staminaDrainCoroutine = StartCoroutine(DrainEndurance()); // ← on démarre la coroutine
            }
        }
        else
        {
            if (isSprinting)
            {
                isSprinting = false;
                moveSpeed = 3f;
                if (staminaDrainCoroutine != null)
                    StopCoroutine(staminaDrainCoroutine); // ← on l'arrête
            }

            if (Endurance < maxEndurance)
            {
                Endurance += 0.1f * Time.deltaTime * 5f;
                Endurance = Mathf.Min(Endurance, maxEndurance);
            }
        }
    }

    bool IsGrounded()
    {
        return Physics.Raycast(transform.position + Vector3.up * 0.1f,
                               Vector3.down,
                               groundCheckDistance + 0.1f,
                               groundMask);
    }

    IEnumerator DrainEndurance() // ← coroutine correcte ici
    {
        while (isSprinting && Endurance > 0f)
        {
            Endurance -= 0.2f;
            Endurance = Mathf.Max(Endurance, 0f);
            yield return new WaitForSeconds(0.2f);
        }
    }


}
