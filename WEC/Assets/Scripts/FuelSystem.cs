using UnityEngine;

public class FuelSystemWorld : MonoBehaviour
{
    [Header("Fuel Settings")]
    public float maxFuel = 100f;
    public float currentFuel;
    public float drainRate = 5f;
    public float refillRate = 20f;

    [Header("Fuel Bar Objects")]
    public Transform fuelBarFill;
    private Vector3 originalScale;

    [Header("Refill Detection")]
    private int fuelSourcesInside = 0;

    void Start()
    {
        currentFuel = maxFuel;
        if (fuelBarFill != null)
        {
            originalScale = fuelBarFill.localScale;
            Debug.Log("[FuelSystem] Fuel bar assigned. Original scale = " + originalScale);
        }
        else
        {
            Debug.LogError("[FuelSystem] ERROR: fuelBarFill is NOT assigned in inspector.");
        }
    }

    void Update()
    {
        bool isNearFuelSource = fuelSourcesInside > 0;

        // Debug the state of refill detection
        Debug.Log("[FuelSystem] Near fuel source? " + isNearFuelSource + " | Fuel sources inside: " + fuelSourcesInside);

        // Drain or refill
        if (!isNearFuelSource)
        {
            currentFuel -= drainRate * Time.deltaTime;
        }
        else
        {
            currentFuel += refillRate * Time.deltaTime;
        }

        currentFuel = Mathf.Clamp(currentFuel, 0, maxFuel);

        Debug.Log("[FuelSystem] Current fuel: " + currentFuel + "/" + maxFuel);

        // Update the fuel bar
        if (fuelBarFill != null)
        {
            float fuelPercent = currentFuel / maxFuel;
            fuelBarFill.localScale = new Vector3(
                originalScale.x,
                originalScale.y * fuelPercent,
                originalScale.z
            );

            Debug.Log("[FuelSystem] Updated bar scale to: " + fuelBarFill.localScale);
        }
        else
        {
            Debug.LogWarning("[FuelSystem] Fuel bar object missing — cannot update graphics.");
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
            Debug.Log("ENTERED TRIGGER with: " + collision.name + 
                      " | Tag: " + collision.tag +
                      " | Collider2D: " + collision.GetType());

        
        Debug.Log("[FuelSystem] Trigger ENTER with: " + collision.name);

        if (collision.CompareTag("FuelSource"))
        {
            fuelSourcesInside++;
            Debug.Log("[FuelSystem] ENTERED Fuel Source. Count now = " + fuelSourcesInside);
        }
        else
        {
            Debug.Log("[FuelSystem] Entered NON fuel source.");
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        Debug.Log("[FuelSystem] Trigger EXIT with: " + collision.name);

        if (collision.CompareTag("FuelSource"))
        {
            fuelSourcesInside--;
            fuelSourcesInside = Mathf.Max(fuelSourcesInside, 0);

            Debug.Log("[FuelSystem] EXITED Fuel Source. Count now = " + fuelSourcesInside);
        }
        else
        {
            Debug.Log("[FuelSystem] Exited NON fuel source.");
        }
    }
}
