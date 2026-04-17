using UnityEngine;

public class PlayerCarController : MonoBehaviour
{
    [Header("Carriles")]
    [SerializeField] private float leftLaneX = -4.3f;
    [SerializeField] private float middleLaneX = -1.29f;
    [SerializeField] private float rightLaneX = 1.4f;

    [Header("Movimiento lateral")]
    [SerializeField] private float laneChangeSpeed = 8f;

    [Header("Boost en Z")]
    [SerializeField] private float boostSpeed = 2f;
    [SerializeField] private float boostDuration = 0.3f;
    [SerializeField] private float boostCooldown = 1.5f;

    [Header("Detección enemigo detrás")]
    [SerializeField] private float rearCheckDistance = 0.25f;
    //[SerializeField] private Vector3 rearCheckBoxHalfExtents = new Vector3(1.3f, 1.2f, 3.5f);
    [SerializeField] private Vector3 rearCheckBoxHalfExtents = new Vector3(1.3f, 1.2f, 0.6f);
    [SerializeField] private LayerMask enemyCarLayer;

    private float targetLaneX;
    private float boostTimer = 0f;
    private float cooldownTimer = 0f;

    private enum Lane
    {
        Left,
        Middle,
        Right
    }

    private void Start()
    {
        targetLaneX = GetClosestLaneX();
    }

    private void Update()
    {
        HandleInput();
        HandleLaneChange();
        HandleBoost();
    }

    private void HandleInput()
    {
        if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow))
        {
            MoveLeft();
        }

        if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow))
        {
            MoveRight();
        }
    }

    private void MoveLeft()
    {
        Lane currentLane = GetCurrentLane();

        switch (currentLane)
        {
            case Lane.Middle:
                targetLaneX = leftLaneX;
                break;
            case Lane.Right:
                targetLaneX = middleLaneX;
                break;
        }
    }

    private void MoveRight()
    {
        Lane currentLane = GetCurrentLane();

        switch (currentLane)
        {
            case Lane.Middle:
                targetLaneX = rightLaneX;
                break;
            case Lane.Left:
                targetLaneX = middleLaneX;
                break;
        }
    }

    private void HandleLaneChange()
    {
        Vector3 pos = transform.position;
        float newX = Mathf.MoveTowards(pos.x, targetLaneX, laneChangeSpeed * Time.deltaTime);
        transform.position = new Vector3(newX, pos.y, pos.z);
    }

    private void HandleBoost()
    {
        if (cooldownTimer > 0f)
            cooldownTimer -= Time.deltaTime;

        if (boostTimer > 0f)
        {
            boostTimer -= Time.deltaTime;
            transform.position += Vector3.forward * boostSpeed * Time.deltaTime;
            return;
        }

        if (cooldownTimer <= 0f && IsEnemyBehind())
        {
            boostTimer = boostDuration;
            cooldownTimer = boostCooldown + boostDuration;
        }
    }

    private bool IsEnemyBehind()
    {
        Vector3 checkCenter = transform.position + Vector3.back * (rearCheckDistance * 0.5f);

        Collider[] hits = Physics.OverlapBox(
            checkCenter,
            rearCheckBoxHalfExtents,
            Quaternion.identity,
            enemyCarLayer
        );

        foreach (Collider hit in hits)
        {
            if (hit.gameObject == gameObject)
                continue;

            if (hit.transform.position.z < transform.position.z)
                return true;
        }

        return false;
    }

    private Lane GetCurrentLane()
    {
        float x = GetClosestLaneX();

        if (Mathf.Abs(x - leftLaneX) < 0.1f)
            return Lane.Left;

        if (Mathf.Abs(x - middleLaneX) < 0.1f)
            return Lane.Middle;

        return Lane.Right;
    }

    private float GetClosestLaneX()
    {
        float x = transform.position.x;

        float distLeft = Mathf.Abs(x - leftLaneX);
        float distMiddle = Mathf.Abs(x - middleLaneX);
        float distRight = Mathf.Abs(x - rightLaneX);

        if (distLeft < distMiddle && distLeft < distRight)
            return leftLaneX;

        if (distMiddle < distLeft && distMiddle < distRight)
            return middleLaneX;

        return rightLaneX;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Vector3 checkCenter = transform.position + Vector3.back * (rearCheckDistance * 0.5f);
        Gizmos.DrawWireCube(checkCenter, rearCheckBoxHalfExtents * 2f);
    }
}