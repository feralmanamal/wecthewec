using UnityEngine;
using System.Collections.Generic;
using System.IO;
using System.Linq;

public class GlassSquidSpawner : MonoBehaviour
{
    [Header("CSV File Name (inside StreamingAssets)")]
    public string csvFileName = "life.csv";

    [Header("Fish Settings")]
    public GameObject fishPrefab;

    [Tooltip("How many Glass Squid to spawn.")]
    public int numberOfFishToSpawn = 3;

    [Header("X Position Mapping (Row → Unity X)")]
    public float xMappedMin = 104f;
    public float xMappedMax = 487f;

    [Tooltip("CSV Row values min/max for mapping")]
    public float csvRowMin = 0f;
    public float csvRowMax = 100f;

    [Header("Vertical Movement Settings")]
    public float yMovementRange = 5f;

    public float minYSpawn = 0f;
    public float maxYSpawn = 200f;

    void Start()
    {
        string path = Path.Combine(Application.streamingAssetsPath, csvFileName);

        if (!File.Exists(path))
        {
            Debug.LogError("CSV not found: " + path);
            return;
        }

        // Read CSV
        List<OceanSpecies> glassSquid = ReadGlassSquid(path);

        if (glassSquid.Count == 0)
        {
            Debug.LogError("No Glass_Squid entries found in CSV.");
            return;
        }

        // Clamp amount
        int amount = Mathf.Min(numberOfFishToSpawn, glassSquid.Count);

        // Pick random entries
        System.Random rand = new System.Random();
        List<OceanSpecies> selected =
            glassSquid.OrderBy(x => rand.Next()).Take(amount).ToList();

        // Spawn fish
        for (int i = 0; i < selected.Count; i++)
        {
            SpawnFish(selected[i], i);
        }
    }

    // -------------------------------
    // CSV Reader
    // -------------------------------
    List<OceanSpecies> ReadGlassSquid(string path)
    {
        List<OceanSpecies> list = new List<OceanSpecies>();

        using (var fs = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
        using (var reader = new StreamReader(fs))
        {
            string header = reader.ReadLine();

            string line;
            while ((line = reader.ReadLine()) != null)
            {
                string[] f = line.Split(',');

                if (f.Length >= 4 && f[2] == "Glass_Squid")
                {
                    list.Add(new OceanSpecies
                    {
                        Row = f[0],
                        Col = f[1],
                        Depth = int.Parse(f[3])
                    });
                }
            }
        }

        return list;
    }

    // -------------------------------
    // Spawning
    // -------------------------------
    void SpawnFish(OceanSpecies squid, int index)
    {
        // Convert CSV row (string) to float
        if (!float.TryParse(squid.Row, out float rowValue))
            rowValue = csvRowMin;

        // Map CSV row → Unity X (104 → 487)
        float xPos = MapValue(rowValue, csvRowMin, csvRowMax, xMappedMin, xMappedMax);

        // Random Y spawn point
        float yPos = Random.Range(minYSpawn, maxYSpawn);

        GameObject fish = Instantiate(fishPrefab, new Vector3(xPos, yPos, 0), Quaternion.identity);

        // Create waypoints for vertical motion
        GameObject pA = new GameObject($"Squid{index}_PointA");
        GameObject pB = new GameObject($"Squid{index}_PointB");

        // ±5 Y range
        float yOffset = Random.Range(-yMovementRange, yMovementRange);

        pA.transform.position = new Vector3(xPos, yPos + yOffset, 0);
        pB.transform.position = new Vector3(xPos, yPos - yOffset, 0);

        // Assign to FishAI
        FishAI ai = fish.GetComponent<FishAI>();
        ai.pointA = pA.transform;
        ai.pointB = pB.transform;
    }

    float MapValue(float value, float inMin, float inMax, float outMin, float outMax)
    {
        float t = Mathf.InverseLerp(inMin, inMax, value);
        return Mathf.Lerp(outMin, outMax, t);
    }
}

public class OceanSpecies
{
    public string Row;
    public string Col;
    public int Depth;
}
