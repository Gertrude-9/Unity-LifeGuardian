using UnityEngine;
using System.Collections;
using TMPro;

public class PlayerRunJumpCollision : MonoBehaviour
{
    [Header("Jump")]
    public Rigidbody rb;
    public float jumpForce = 12f;
    public bool isGrounded = true;

    public TMP_Text messageText;
    public float messageDuration = 2f;

    [Header("References")]
    public Animator animator;
    public PlayerStatusManager statusManager;
    public PlayerLaneMovement laneMovement;
    public RewardManager rewardManager;
    public CameraShake cameraShake;
    public FoodSpawner foodSpawner;

    [Header("Animation State Names")]
    public string runningState = "Running";
    public string jumpingState = "Jumping Up";
    public string fallingState = "Dying";
    public string standingState = "Standing Up";

    [Header("Timing")]
    public float fallAnimationTime = 1.2f;
    public float standUpAnimationTime = 1.5f;
    public float momentumRecoverTime = 2f;

    [Header("Momentum")]
    public float weakMoveSpeed = 2f;
    public float normalMoveSpeed = 6f;

    private bool isFallen = false;
    public EducationManager educationManager;

    void Start()
    {
        if (rb == null)
            rb = GetComponent<Rigidbody>();

        if (educationManager == null)
            educationManager = FindObjectOfType<EducationManager>();

        if (animator == null)
            animator = GetComponentInChildren<Animator>();

        if (statusManager == null)
            statusManager = GetComponent<PlayerStatusManager>();

        if (laneMovement == null)
            laneMovement = GetComponent<PlayerLaneMovement>();

        if (rewardManager == null)
            rewardManager = FindObjectOfType<RewardManager>();

        if (cameraShake == null)
            cameraShake = FindObjectOfType<CameraShake>();

        if (foodSpawner == null)
            foodSpawner = FindObjectOfType<FoodSpawner>();

        if (rb != null)
            rb.freezeRotation = true;

        if (animator != null)
            animator.Play(runningState, 0, 0f);
    }

    void Update()
    {
        if (isFallen) return;

        if (Input.GetKeyDown(KeyCode.UpArrow) && isGrounded)
            Jump();
    }

    void Jump()
    {
        isGrounded = false;

        if (animator != null)
            animator.Play(jumpingState, 0, 0f);

        if (rb != null)
        {
            rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        FoodItem food = other.GetComponentInParent<FoodItem>();

        if (food == null)
            return;

        Debug.Log("Hit food: " + food.gameObject.name);

        if (rewardManager != null)
            rewardManager.AddScore(food.scoreValue);

        if (statusManager != null)
            statusManager.ApplyFoodEffects(food);

        // IMPORTANT:
        // Player falls ONLY if Make Player Fall is ticked on the food prefab.
        // Negative score alone will NOT make the player fall.
        if (food.makePlayerFall)
            StartCoroutine(FallStandRun());

        Destroy(food.gameObject);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;

            if (!isFallen && animator != null)
                animator.Play(runningState, 0, 0f);
        }
    }

    IEnumerator FallStandRun()
    {
        if (isFallen)
            yield break;

        isFallen = true;

        if (foodSpawner != null)
            foodSpawner.PauseSpawning();

        if (laneMovement != null)
            laneMovement.DisableMovement();

        if (rb != null)
        {
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }

        if (animator != null)
            animator.Play(fallingState, 0, 0f);

        if (cameraShake != null)
            cameraShake.Shake(0.3f, 0.25f);
            

        yield return new WaitForSeconds(fallAnimationTime);

        if (animator != null)
            animator.Play(standingState, 0, 0f);

        yield return new WaitForSeconds(standUpAnimationTime);

        if (educationManager != null)
             educationManager.HideBadFoodMessage();

        if (animator != null)
            animator.Play(runningState, 0, 0f);

        if (laneMovement != null)
        {
            laneMovement.EnableMovement();
            StartCoroutine(RegainMomentum());
        }

        if (foodSpawner != null)
            foodSpawner.ResumeSpawning();

        isFallen = false;
    }

    IEnumerator RegainMomentum()
    {
        float timer = 0f;

        if (laneMovement != null)
            laneMovement.moveSpeed = weakMoveSpeed;

        while (timer < momentumRecoverTime)
        {
            timer += Time.deltaTime;
            float t = timer / momentumRecoverTime;

            if (laneMovement != null)
                laneMovement.moveSpeed = Mathf.Lerp(weakMoveSpeed, normalMoveSpeed, t);

            yield return null;
        }

        if (laneMovement != null)
            laneMovement.moveSpeed = normalMoveSpeed;
    }

   public void ReactToBadFood(string message)
{
    if (!isFallen)
    {
        if (educationManager != null)
            educationManager.ShowBadFoodMessage(message);

        StartCoroutine(FallStandRun());
    }
}

    public void ShowFoodMessage(string msg)
{
    if (messageText == null) return;

    StopCoroutine("HideMessage");
    messageText.text = msg;
    messageText.gameObject.SetActive(true);
    StartCoroutine(HideMessage());
}

IEnumerator HideMessage()
{
    yield return new WaitForSeconds(messageDuration);
    messageText.gameObject.SetActive(false);
}
}