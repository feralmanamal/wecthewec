using UnityEngine;

public class RelativePositioner : MonoBehaviour
{
    public Transform targetGameObject; // Assign the parent GameObject in the Inspector
    public Vector2 offset = new Vector2(1f, 1f); // Adjust these values for your desired top-right offset

    void Update()
    {
        if (targetGameObject != null)
        {
            // Calculate the new position
            // targetGameObject.position gives the world position of the target
            // transform.right and transform.up represent the target's local X and Y axes
            // Multiplying by transform.right * offset.x ensures the offset is applied relative to the target's rotation
            // For 2D, transform.right and transform.up are typically aligned with world X and Y if the target has no rotation.
            // If the target *does* rotate, this correctly applies the offset relative to its orientation.
            transform.position = (Vector2)targetGameObject.position + (Vector2)(targetGameObject.right * offset.x) + (Vector2)(targetGameObject.up * offset.y);
        }
    }
}