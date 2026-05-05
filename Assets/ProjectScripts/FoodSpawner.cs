using UnityEngine;

public class FoodSpawner : MonoBehaviour
{
    public GameObject[] foodPrefabs;
    public Transform roadPlane;

    [Header("Spawn Timing")]
    public float spawnInterval = 3.5f;
    private float timer = 0f;

    [Header("Spawn Position")]
    public float spawnHeight = 2f;
    public float spawnForwardOffset = 50f;

    public float[] laneOffsets = { -15f, 0f, 15f };

    [Header("Spawner Control")]
    public bool canSpawn = true;

    void Start()
    {
        foodPrefabs = Resources.LoadAll<GameObject>("Foods2");

        if (foodPrefabs == null || foodPrefabs.Length == 0)
        {
            Debug.LogWarning("No food prefabs found in Assets/Resources/Foods2.");
        }
    }

    void Update()
    {
        if (Time.timeScale == 0f || !canSpawn)
            return;

        timer += Time.deltaTime;

        if (timer >= spawnInterval)
        {
            SpawnFood();
            timer = 0f;
        }
    }

    void SpawnFood()
    {
        if (foodPrefabs == null || foodPrefabs.Length == 0)
        {
            Debug.LogWarning("No food prefabs loaded from Foods2.");
            return;
        }

        if (roadPlane == null)
        {
            Debug.LogWarning("Road Plane is not assigned in FoodSpawner.");
            return;
        }

        int randomFood = Random.Range(0, foodPrefabs.Length);
        int randomLane = Random.Range(0, laneOffsets.Length);

        GameObject selectedPrefab = foodPrefabs[randomFood];

        float spawnX = roadPlane.position.x + laneOffsets[randomLane];
        float spawnY = roadPlane.position.y + spawnHeight;
        float spawnZ = roadPlane.position.z + spawnForwardOffset;

        Vector3 spawnPosition = new Vector3(spawnX, spawnY, spawnZ);

        GameObject spawnedFood = Instantiate(
            selectedPrefab,
            spawnPosition,
            selectedPrefab.transform.rotation
        );

        Debug.Log("Spawned food: " + spawnedFood.name + " at " + spawnPosition);
    }

    public void PauseSpawning()
    {
        canSpawn = false;
        timer = 0f;
    }

    public void ResumeSpawning()
    {
        canSpawn = true;
    }
}