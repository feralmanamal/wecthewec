using UnityEngine;

public class WeightedSpawner : MonoBehaviour
{
    [System.Serializable]
    public class WeightedRange
    {
        public float minX;
        public float maxX;
        public float weight = 1f;
    }

    [Header("Spawn Settings")]
    public GameObject objectToSpawn;
    public float constantY = 0f; 
    public WeightedRange[] ranges;

    private bool hasSpawned = false;   // <--- prevents multiple spawns

    void Start()
    {
        SpawnOnce();
    }

    void SpawnOnce()
    {
        if (hasSpawned) return;                     // <--- safety lock
        hasSpawned = true;

        // Validate
        if (objectToSpawn == null)
        {
            Debug.LogError("WeightedSpawner: objectToSpawn is NULL.");
            return;
        }

        if (ranges == null || ranges.Length == 0)
        {
            Debug.LogError("WeightedSpawner: No ranges defined.");
            return;
        }

        // Prevent self-replication loop
        if (objectToSpawn.GetComponent<WeightedSpawner>() != null)
        {
            Debug.LogError("WeightedSpawner ERROR: The prefab you're spawning CONTAINS this script. Remove the script from the prefab!");
            return;
        }

        WeightedRange chosen = ChooseWeightedRange();

        float x = Random.Range(chosen.minX, chosen.maxX);
        Vector3 pos = new Vector3(x, constantY, 0f);

        Instantiate(objectToSpawn, pos, Quaternion.identity);
    }

    WeightedRange ChooseWeightedRange()
    {
        float totalWeight = 0f;
        foreach (var r in ranges)
            totalWeight += r.weight;

        float random = Random.Range(0f, totalWeight);

        foreach (var r in ranges)
        {
            if (random < r.weight)
                return r;
            random -= r.weight;
        }

        return ranges[0];
    }
}