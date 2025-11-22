using UnityEngine;

public class FishAI : MonoBehaviour
{
    [Header("Waypoints")]
    public Transform pointA;
    public Transform pointB;

    [Header("Settings")]
    public float speed = 2f;
    public float waitTimeMin = 1f;
    public float waitTimeMax = 3f;

    private Transform targetPoint;
    private SpriteRenderer sr;
    private bool isWaiting = false;

    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        targetPoint = pointB; // start moving toward B
    }

    void Update()
    {
        if (!isWaiting)
        {
            MoveFish();
        }
    }

    void MoveFish()
    {
        // Move towards the target
        transform.position = Vector2.MoveTowards(
            transform.position,
            targetPoint.position,
            speed * Time.deltaTime
        );

        // Flip based on direction
        if (sr != null)
        {
            if (targetPoint.position.x < transform.position.x)
                sr.flipX = true;   // Facing left
            else
                sr.flipX = false;  // Facing right
        }
        else
        {
            // If no SpriteRenderer, flip scale instead
            Vector3 scale = transform.localScale;
            scale.x = (targetPoint.position.x < transform.position.x) ? -1 : 1;
            transform.localScale = scale;
        }

        // If reached the waypoint
        if (Vector2.Distance(transform.position, targetPoint.position) < 0.1f)
        {
            StartCoroutine(WaitAtPoint());
        }
    }

    System.Collections.IEnumerator WaitAtPoint()
    {
        isWaiting = true;

        // Pick next destination
        targetPoint = (targetPoint == pointA) ? pointB : pointA;

        // Wait a random time
        float t = Random.Range(waitTimeMin, waitTimeMax);
        yield return new WaitForSeconds(t);

        isWaiting = false;
    }
}