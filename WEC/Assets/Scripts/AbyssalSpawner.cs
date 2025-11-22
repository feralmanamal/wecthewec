using UnityEngine;
using System.Collections.Generic;
using System.IO;
using System.Linq;

public class AbyssalSpawner : MonoBehaviour
{
    [Header("CSV File Name (inside StreamingAssets)")]
    public string csvFileName = "life.csv";

    [Header("Fish Settings")]
    public GameObject fishPrefab;

    [Tooltip("How many Abyssal_Ray fish to spawn.")]
    public int numberOfFishToSpawn = 3;

    [Tooltip("Fish spawn X will be between these values.")]
    public float minSpawnX = -20f;
    public float maxSpawnX = 20f;

    [Tooltip("Waypoint range relative to spawn X.")]
    public float movementRangeX = 5f; // ±5 movement

    [Header("Depth Mapping")]
    public float yMappedMin = 0f;     // bottom of screen
    public float yMappedMax = 208f;   // top of screen
    public float csvMinDepth = 0f;    // lowest depth in your dataset
    public float csvMaxDepth = 6000f; // highest depth in your dataset

    void Start()
    {
        string path = Path.Combine(Application.streamingAssetsPath, csvFileName);

        if (!File.Exists(path))
        {
            Debug.LogError("CSV file not found at: " + path);
            return;
        }

        // Read CSV data
        List<OceanSpecies> abyssalRays = ReadAbyssalRays(path);

        if (abyssalRays.Count == 0)
        {
            Debug.LogError("No Abyssal_Ray entries in the CSV file.");
            return;
        }

        // Clamp fish number
        int amount = Mathf.Min(numberOfFishToSpawn, abyssalRays.Count);

        System.Random random = new System.Random();

        // Pick N random entries
        List<OceanSpecies> selected =
            abyssalRays.OrderBy(x => random.Next()).Take(amount).ToList();

        // Spawn them
        for (int i = 0; i < selected.Count; i++)
        {
            SpawnFishFromDepth(selected[i], i);
        }
    }

    // -------------------------------
    // CSV READING
    // -------------------------------
    List<OceanSpecies> ReadAbyssalRays(string path)
    {
        List<OceanSpecies> rays = new List<OceanSpecies>();

        using (var fs = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
        using (var reader = new StreamReader(fs))
        {
            string header = reader.ReadLine(); // skip header

            string line;
            while ((line = reader.ReadLine()) != null)
            {
                string[] fields = line.Split(',');

                // CSV: Row,Col,Species,Depth
                if (fields.Length >= 4 && fields[2] == "Abyssal_Ray")
                {
                    if (int.TryParse(fields[3], out int depth))
                    {
                        rays.Add(new OceanSpecies
                        {
                            Row = fields[0],
                            Col = fields[1],
                            Depth = depth
                        });
                    }
                }
            }
        }

        return rays;
    }

    // -------------------------------
    // FISH SPAWNING
    // -------------------------------
    void SpawnFishFromDepth(OceanSpecies fishData, int index)
    {
        float depth = fishData.Depth;

        // Map CSV depth → Unity Y
        float yPos = MapDepthToUnityY(depth);

        // Random X within range
        float startX = Random.Range(minSpawnX, maxSpawnX);

        // Create fish
        GameObject fish = Instantiate(fishPrefab, new Vector3(startX, yPos, 0f), Quaternion.identity);

        // Create waypoints
        GameObject pointA = new GameObject($"Fish{index}_PointA");
        GameObject pointB = new GameObject($"Fish{index}_PointB");

        float offset = Random.Range(-movementRangeX, movementRangeX);

        pointA.transform.position = new Vector3(startX + offset, yPos, 0f);
        pointB.transform.position = new Vector3(startX - offset, yPos, 0f);

        // Assign to FishAI
        FishAI ai = fish.GetComponent<FishAI>();
        ai.pointA = pointA.transform;
        ai.pointB = pointB.transform;
    }

    float MapDepthToUnityY(float depth)
    {
        float t = Mathf.InverseLerp(csvMinDepth, csvMaxDepth, depth);
        return Mathf.Lerp(yMappedMin, yMappedMax, t);
    }
}

// Helper data class
public class OceanSpecie
{
    public string Row;
    public string Col;
    public int Depth;
}
