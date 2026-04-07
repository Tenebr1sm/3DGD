using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class cubeMovement : MonoBehaviour
{
    public GameObject winScreen;

    [Header("Movement")]
    public float moveSpeed;
    public float jumpForce;

    private Rigidbody rb;
    private bool isGrounded;
    private bool isRotating = false;
    private bool isLevelFinished = false;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        // rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY;

        if (winScreen != null)
            winScreen.SetActive(false);
    }

    void Update()
    {

        if (Keyboard.current.spaceKey.isPressed && isGrounded)
        {
            Jump();
        }

        if (Mouse.current.leftButton.isPressed && isGrounded)
        {
            Jump();
        }
    }

    void FixedUpdate()
    {
        if (isLevelFinished) return;

        rb.linearVelocity = new Vector3(0, rb.linearVelocity.y, moveSpeed);
        // transform.Translate(Vector3.forward * moveSpeed * Time.deltaTime);
    }

    void Jump()
    {
        isGrounded = false;
        rb.linearVelocity = new Vector3(0, 0, rb.linearVelocity.z);
        rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);

        // Calculate air time: (2 * velocity) / gravity
        // We use jumpForce directly since it's an Impulse on a Rigidbody of mass 1
        float airTime = (2f * jumpForce) / Mathf.Abs(Physics.gravity.y);

        if (!isRotating) StartCoroutine(SyncFlip(airTime));
    }

    //weird math that makes the flip smooth
    System.Collections.IEnumerator SyncFlip(float duration)
    {
        isRotating = true;

        Quaternion startRotation = transform.rotation;
        // Rotate 90 degrees around the X axis (the "rolling" axis for Z movement)
        Quaternion endRotation = transform.rotation * Quaternion.Euler(90, 0, 0);

        float elapsed = 0;
        while (elapsed < duration)
        {
            // Slerp (Spherical Linear Interpolation) makes the rotation smooth
            transform.rotation = Quaternion.Slerp(startRotation, endRotation, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.rotation = endRotation;
        isRotating = false;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
        }

        if (collision.gameObject.CompareTag("Spike"))
        {
            Die();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Goal"))
        {
            Win();
        }
    }

    void Die()
    {
        Debug.Log("cube is die");

        transform.position = new Vector3(0, 1, 0);

    }

    void Win()
    {
        isLevelFinished = true; // Switch the movement off
        rb.isKinematic = true;  // Stop physics interactions

        if (winScreen != null) winScreen.SetActive(true);

        Time.timeScale = 0f;

        // Unlock the mouse so you can click the button
        //Cursor.lockState = CursorLockMode.None;
        //Cursor.visible = true;
    }

    public void RestartGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
