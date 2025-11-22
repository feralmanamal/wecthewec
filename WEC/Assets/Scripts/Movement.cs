using UnityEngine;

public class movement : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 5f; // Adjust in Inspector
    private Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void FixedUpdate() // Use FixedUpdate for physics-based movement
    {
        // Get input values for horizontal and vertical movement
        float horizontalInput = Input.GetAxisRaw("Horizontal"); // -1 for left, 1 for right, 0 for none
        float verticalInput = Input.GetAxisRaw("Vertical");   // -1 for down, 1 for up, 0 for none

        // Create a movement vector
        Vector2 movementDirection = new Vector2(horizontalInput, verticalInput).normalized; 
        // Normalize to prevent faster diagonal movement

        // Set the Rigidbody2D's velocity
        rb.linearVelocity = movementDirection * moveSpeed;
    }
}