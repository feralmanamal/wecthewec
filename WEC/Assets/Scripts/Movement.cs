using UnityEngine;
using UnityEngine.SceneManagement;

public class Movement : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 5f;
    private Rigidbody2D rb;
    private bool facingRight = true;

    // Reference to fuel system (assign in Inspector)
    public FuelSystemWorld fuelSystem;

    // Game over UI (assign a Canvas or panel)
    public GameObject gameOverScreen;

    private bool isGameOver = false;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        gameOverScreen.SetActive(false); // Hide at start
    }

    void Update()
    {
        // If game over, listen for restart
        if (isGameOver)
        {
            if (Input.GetKeyDown(KeyCode.R))
            {
                SceneManager.LoadScene(SceneManager.GetActiveScene().name);
            }
            return;
        }
    }

    void FixedUpdate()
    {
        // If out of fuel, trigger game over
        if (fuelSystem != null && fuelSystem.currentFuel <= 0f && !isGameOver)
        {
            TriggerGameOver();
            return; // stop movement
        }

        // If game over, freeze movement
        if (isGameOver)
        {
            rb.linearVelocity = Vector2.zero;
            return;
        }

        // Movement
        float horizontalInput = Input.GetAxisRaw("Horizontal");
        float verticalInput = Input.GetAxisRaw("Vertical");

        Vector2 movementDirection = new Vector2(horizontalInput, verticalInput).normalized;
        rb.linearVelocity = movementDirection * moveSpeed;

        HandleFlip(horizontalInput);
    }

    void HandleFlip(float horizontalInput)
    {
        if (horizontalInput > 0 && !facingRight)
            Flip();
        else if (horizontalInput < 0 && facingRight)
            Flip();
    }

    void Flip()
    {
        facingRight = !facingRight;
        Vector3 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;
    }

    void TriggerGameOver()
    {
        isGameOver = true;
        rb.linearVelocity = Vector2.zero; // freeze movement
        gameOverScreen.SetActive(true);
    }
}
