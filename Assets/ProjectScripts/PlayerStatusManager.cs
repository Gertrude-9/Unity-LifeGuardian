using UnityEngine;

public class PlayerStatusManager : MonoBehaviour
{
    [Header("References")]
    public Transform bellyTransform;
    public PlayerLaneMovement laneMovement;

    [Header("Bloating")]
    public float bloatingLevel = 0f;
    public float maxBloatingLevel = 100f;
    public float healthyRecoveryAmount = 5f;

    [Header("Belly Scale")]
    public Vector3 normalBellyScale = Vector3.one;
    public Vector3 maxBloatedBellyScale = new Vector3(1.8f, 1.4f, 1.8f);
    public float scaleLerpSpeed = 6f;

    [Header("Speed Effect")]
    public float normalMoveSpeed = 6f;
    public float minMoveSpeed = 2f;
    public float speedReduceAmount = 1.5f;
    public float healthySpeedRecoverAmount = 0.6f;

    [Header("Food Score Rules")]
    public int speedIncreaseStartsAt = 5;

    private Vector3 targetBellyScale;
    private float currentMoveSpeed;

    void Start()
    {
        targetBellyScale = normalBellyScale;
        currentMoveSpeed = normalMoveSpeed;

        if (bellyTransform != null)
            bellyTransform.localScale = normalBellyScale;

        if (laneMovement == null)
            laneMovement = GetComponent<PlayerLaneMovement>();

        ApplySpeed();
    }

    void Update()
    {
        if (bellyTransform != null)
        {
            bellyTransform.localScale = Vector3.Lerp(
                bellyTransform.localScale,
                targetBellyScale,
                Time.deltaTime * scaleLerpSpeed
            );
        }
    }

    public void ApplyFoodEffects(FoodItem food)
    {
        if (food == null) return;

        // FOOD WITH 5 POINTS AND ABOVE INCREASES SPEED
        if (food.scoreValue >= speedIncreaseStartsAt)
        {
            currentMoveSpeed += healthySpeedRecoverAmount;

            if (food.isHealthy)
                bloatingLevel -= healthyRecoveryAmount;
        }
        // FOOD WITH 0 OR NEGATIVE POINTS REDUCES SPEED
        else if (food.scoreValue <= 0)
        {
            currentMoveSpeed -= speedReduceAmount;

            if (food.scoreValue < 0)
                currentMoveSpeed -= Mathf.Abs(food.scoreValue) * 0.02f;

            bloatingLevel += food.bloatingEffect;

            if (food.scoreValue < 0)
                bloatingLevel += Mathf.Abs(food.scoreValue) * 0.3f;
        }

        currentMoveSpeed = Mathf.Clamp(currentMoveSpeed, minMoveSpeed, normalMoveSpeed);
        bloatingLevel = Mathf.Clamp(bloatingLevel, 0f, maxBloatingLevel);

        float bloatingPercent = bloatingLevel / maxBloatingLevel;

        targetBellyScale = Vector3.Lerp(
            normalBellyScale,
            maxBloatedBellyScale,
            bloatingPercent
        );

        ApplySpeed();

        Debug.Log("Food: " + food.foodName +
                  " | Score: " + food.scoreValue +
                  " | Speed: " + currentMoveSpeed +
                  " | Bloating: " + bloatingLevel);
    }

    void ApplySpeed()
    {
        if (laneMovement != null)
            laneMovement.moveSpeed = currentMoveSpeed;
    }
}