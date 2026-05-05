using UnityEngine;

public class PlayerLaneMovement : MonoBehaviour
{
    [Header("Lane Settings")]
    public Transform roadPlane;
    public float[] laneOffsets = { -15f, 0f, 15f };
    public float moveSpeed = 4f;

    [Header("State Control")]
    public bool canMove = true;

    [Header("Optional Visual Turn")]
    public Transform remyVisual;
    public float turnAngle = 12f;
    public float turnSpeed = 8f;

    private int currentLane = 1;
    private Vector3 targetPosition;
    private Quaternion remyStartRotation;

    void Start()
    {
        FindClosestLane();
        SetTargetToCurrentLane();

        if (remyVisual != null)
            remyStartRotation = remyVisual.localRotation;
    }

    void Update()
    {
        if (Time.timeScale == 0f || !canMove) return;

        if (Input.GetKeyDown(KeyCode.LeftArrow))
            ChangeLane(-1);

        if (Input.GetKeyDown(KeyCode.RightArrow))
            ChangeLane(1);

        transform.position = Vector3.MoveTowards(
            transform.position,
            targetPosition,
            moveSpeed * Time.deltaTime
        );

        if (remyVisual != null)
        {
            float deltaX = targetPosition.x - transform.position.x;
            float zTilt = Mathf.Clamp(-deltaX * turnAngle, -turnAngle, turnAngle);

            Quaternion targetRot = remyStartRotation * Quaternion.Euler(0f, 0f, zTilt);

            remyVisual.localRotation = Quaternion.Lerp(
                remyVisual.localRotation,
                targetRot,
                Time.deltaTime * turnSpeed
            );
        }
    }

    void ChangeLane(int direction)
    {
        currentLane = Mathf.Clamp(currentLane + direction, 0, laneOffsets.Length - 1);
        SetTargetToCurrentLane();
    }

    void SetTargetToCurrentLane()
    {
        float centerX = roadPlane != null ? roadPlane.position.x : 0f;
        float targetX = centerX + laneOffsets[currentLane];

        targetPosition = new Vector3(targetX, transform.position.y, transform.position.z);
    }

    void FindClosestLane()
    {
        float centerX = roadPlane != null ? roadPlane.position.x : 0f;
        float closestDistance = Mathf.Infinity;

        for (int i = 0; i < laneOffsets.Length; i++)
        {
            float laneX = centerX + laneOffsets[i];
            float distance = Mathf.Abs(transform.position.x - laneX);

            if (distance < closestDistance)
            {
                closestDistance = distance;
                currentLane = i;
            }
        }
    }

    public void DisableMovement()
    {
        canMove = false;
        targetPosition = transform.position;
    }

    public void EnableMovement()
    {
        FindClosestLane();
        SetTargetToCurrentLane();
        canMove = true;
    }
}