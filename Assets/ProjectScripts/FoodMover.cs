using UnityEngine;

public class FoodMover : MonoBehaviour
{
    [Header("Speed")]
    public static float globalSpeed = 6f;

    [Header("Destroy")]
    public float destroyZ = -10f;

    void Update()
    {
        if (Time.timeScale == 0f) return;

        // Move using shared speed (NO increment here)
        transform.Translate(Vector3.back * globalSpeed * Time.deltaTime, Space.World);

        // Destroy when behind player
        if (transform.position.z <= destroyZ)
        {
            Destroy(gameObject);
        }
    }

    // Optional manual override
    public static void SetGlobalSpeed(float newSpeed)
    {
        globalSpeed = newSpeed;
    }
}