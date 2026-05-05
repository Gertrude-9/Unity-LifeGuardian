using UnityEngine;

public class FoodItem : MonoBehaviour
{
    [Header("Food Info")]
    public string foodName;

    [Header("Food Type")]
    public bool isHealthy = true;

    [Header("Score")]
    public int scoreValue = 10;

    [Header("Body Effects")]
    public int bloatingEffect = 0;

    [Header("Bad Food Behavior")]
    public bool makePlayerFall = false;

    [Header("Education Message")]
    [TextArea(2,3)]
    public string message; // 👈 ADD THIS

    private bool collected = false;

    private void OnTriggerEnter(Collider other)
    {
        if (collected) return;
        if (!other.CompareTag("Player")) return;

        collected = true;

        RewardManager rewardManager = FindObjectOfType<RewardManager>();
        if (rewardManager != null)
            rewardManager.AddScore(scoreValue);

        PlayerStatusManager statusManager = other.GetComponentInParent<PlayerStatusManager>();
        if (statusManager != null)
            statusManager.ApplyFoodEffects(this);

        PlayerRunJumpCollision player = other.GetComponentInParent<PlayerRunJumpCollision>();

        if (player != null)
        {
            // ✅ SHOW MESSAGE ALWAYS (if exists)
            if (!string.IsNullOrEmpty(message))
            {
                player.ShowFoodMessage(message);
            }

            // ✅ FALL ONLY IF MARKED (NOT just negative score)
            if (makePlayerFall)
            {
                player.ReactToBadFood(message);
            }
        }

        Destroy(gameObject);
    }
}