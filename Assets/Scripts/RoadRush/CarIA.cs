using Unity.VisualScripting;
using UnityEngine;

public class CarIA : MonoBehaviour
{
    [Header("Carriles")]
    [SerializeField] private float leftLaneX = -4.3f;
    [SerializeField] private float middleLaneX = -1.29f;
    [SerializeField] private float rightLaneX = 1.4f;

    [Header("Movimiento")]
    [SerializeField] private float forwardSpeed = 8f;
    [SerializeField] private float minSpeed = 1f;
    [SerializeField] private float maxSpeed = 10f;
    [SerializeField] private float laneChangeSpeed = 4f;
    [SerializeField] private float zMovementFactor = 0.15f;

    [Header("Jugador")]
    [SerializeField] private Transform player;

    [Header("Rango relativo al jugador")]
    [SerializeField] private float tooFarAheadZ = 20f;
    [SerializeField] private float tooFarBehindZ = -25f;
    [SerializeField] private float respawnBehindMin = -20f;
    [SerializeField] private float respawnBehindMax = -12f;

    [Header("Decisiones")]
    [SerializeField] private float decisionInterval = 2f;
    [SerializeField] private float laneChangeChance = 0.45f;
    [SerializeField] private float speedChangeChance = 0.65f;

    [Header("Detección")]
    [SerializeField] private float sideCheckRadius = 1.2f;
    [SerializeField] private float forwardBackCheckDistance = 4f;
    [SerializeField] private LayerMask carLayer;
    [SerializeField] private float playerLaneBlockDistanceZ = 6f;

    [Header("Reacción al jugador")]
    [SerializeField] private float sameLaneTolerance = 0.6f;
    [SerializeField] private float slowDownAmount = 2f;

    [Header("Comportamiento evasivo")]
    [SerializeField] private float preferredBehindDistance = 10f;
    [SerializeField] private float avoidPlayerSideDistance = 2f;
    [SerializeField] private float overtakeBoost = 3f;
    [SerializeField] private float evadeBoost = 2f;
    [SerializeField] private float speedRecoverRate = 1.5f;

    [Header("Salto obstaculos")]
    [SerializeField] private LayerMask obstacleLayer;
    [SerializeField] private float obstacleCheckDistance = 3f;
    [SerializeField] private Vector3 obstacleCheckHalfExtents = new Vector3(0.45f, 0.6f, 0.8f);
    [SerializeField] private float jumpHeight = 1.6f;
    [SerializeField] private float jumpSpeed = 6f;
    [SerializeField] private float fallSpeed = 7f;

    private float groundY;
    private bool isJumping = false;
    private bool isFalling = false;

    private float targetLaneX;
    private bool isChangingLane = false;
    private float decisionTimer;

    private enum Lane
    {
        Left,
        Middle,
        Right
    }

    private void Start()
    {
        targetLaneX = GetClosestLaneX();
        groundY = transform.position.y;
        decisionTimer = decisionInterval;
        forwardSpeed = Random.Range(minSpeed, maxSpeed);
        
    }

    private void Update()
    {
        ReactToPlayer();
        MoveForward();
        HandleLaneChange();
        HandleDecisionTimer();
        RecycleIfNeeded();
        CheckObstacleJump();
        HandleJump();
    }
    private void CheckObstacleJump()
    {
        if (isJumping || isFalling)
            return;

        Vector3 checkCenter = transform.position + Vector3.forward * obstacleCheckDistance;

        Collider[] hits = Physics.OverlapBox(
            checkCenter,
            obstacleCheckHalfExtents,
            Quaternion.identity,
            obstacleLayer
        );

        if (hits.Length > 0)
        {
            isJumping = true;
        }
    }

    private void HandleJump()
    {
        Vector3 pos = transform.position;

        if (isJumping)
        {
            pos.y = Mathf.MoveTowards(pos.y, groundY + jumpHeight, jumpSpeed * Time.deltaTime);
            transform.position = pos;

            if (Mathf.Abs(transform.position.y - (groundY + jumpHeight)) < 0.05f)
            {
                isJumping = false;
                isFalling = true;
            }

            return;
        }

        if (isFalling)
        {
            pos.y = Mathf.MoveTowards(pos.y, groundY, fallSpeed * Time.deltaTime);
            transform.position = pos;

            if (Mathf.Abs(transform.position.y - groundY) < 0.05f)
            {
                transform.position = new Vector3(transform.position.x, groundY, transform.position.z);
                isFalling = false;
            }
        }
    }

    private void ReactToPlayer()
    {
        if (player == null)
            return;

        float zDiff = player.position.z - transform.position.z;
        float absZDiff = Mathf.Abs(zDiff);

        bool isBehindPlayer = zDiff > 0f && zDiff < preferredBehindDistance;
        bool isSideBySide = absZDiff <= avoidPlayerSideDistance;
        bool sameLane = Mathf.Abs(transform.position.x - player.position.x) < sameLaneTolerance;

        if (isBehindPlayer && sameLane)
        {
            Lane currentLane = GetCurrentLane();

            if (currentLane == Lane.Middle)
            {
                Lane firstOption = Random.value < 0.5f ? Lane.Left : Lane.Right;
                Lane secondOption = firstOption == Lane.Left ? Lane.Right : Lane.Left;

                if (TryForceLaneChange(firstOption, true))
                {
                    forwardSpeed = Mathf.Min(maxSpeed, forwardSpeed + evadeBoost * Time.deltaTime);
                    return;
                }

                if (TryForceLaneChange(secondOption, true))
                {
                    forwardSpeed = Mathf.Min(maxSpeed, forwardSpeed + evadeBoost * Time.deltaTime);
                    return;
                }
            }
            else
            {
                if (TryForceLaneChange(Lane.Middle, true))
                {
                    forwardSpeed = Mathf.Min(maxSpeed, forwardSpeed + evadeBoost * Time.deltaTime);
                    return;
                }
            }

            forwardSpeed = Mathf.Max(minSpeed, forwardSpeed - slowDownAmount * Time.deltaTime);
            return;
        }

        if (isSideBySide)
        {
            forwardSpeed = Mathf.Min(maxSpeed, forwardSpeed + overtakeBoost * Time.deltaTime);
            return;
        }

        float cruisingSpeed = Mathf.Lerp(minSpeed, maxSpeed, 0.5f);
        forwardSpeed = Mathf.MoveTowards(forwardSpeed, cruisingSpeed, speedRecoverRate * Time.deltaTime);
    }

    private bool TryForceLaneChange(Lane desiredLane, bool avoidPlayerLane)
    {
        float desiredX = GetLaneX(desiredLane);

        if (avoidPlayerLane && IsPlayerLane(desiredX))
            return false;

        if (CanChangeToLane(desiredX))
        {
            targetLaneX = desiredX;
            isChangingLane = true;
            return true;
        }

        return false;
    }

    private void MoveForward()
    {
        Vector3 pos = transform.position;
        pos.z += forwardSpeed * zMovementFactor * Time.deltaTime;
        transform.position = pos;
    }

    private void RecycleIfNeeded()
    {
        if (player == null)
            return;

        float relativeZ = transform.position.z - player.position.z;

        if (relativeZ > tooFarAheadZ || relativeZ < tooFarBehindZ)
        {
            RespawnBehindPlayer();
        }
    }

    private void RespawnBehindPlayer()
    {
        if (player == null)
            return;

        // Si ya hay demasiados carros detras del jugador, no reaparece atras
        int carsBehind = CountCarsBehindPlayer(12f);

        float[] preferredLanes = { leftLaneX, middleLaneX, rightLaneX };


        for (int i = 0; i < 10; i++)
        {
            float randomLaneX = preferredLanes[Random.Range(0, preferredLanes.Length)];

            if (IsPlayerLane(randomLaneX))
                continue;

            float randomZ;
            if (carsBehind >= 2)
                randomZ = player.position.z + Random.Range(-35f, -25f);
            else
                randomZ = player.position.z + Random.Range(respawnBehindMin, respawnBehindMax);

            Vector3 spawnPos = new Vector3(randomLaneX, groundY, randomZ);

            if (IsSpawnPositionFree(spawnPos))
            {
                transform.position = spawnPos;
                targetLaneX = randomLaneX;
                isChangingLane = false;
                forwardSpeed = Random.Range(minSpeed, maxSpeed);
                return;
            }
        }

        // fallback
        float fallbackLaneX = leftLaneX;
        if (IsPlayerLane(fallbackLaneX))
            fallbackLaneX = rightLaneX;

        float fallbackZ = player.position.z - 30f;
        transform.position = new Vector3(fallbackLaneX, transform.position.y, fallbackZ);
        targetLaneX = fallbackLaneX;
        isChangingLane = false;
    }


    private bool IsSpawnPositionFree(Vector3 spawnPos)
    {
        Collider[] nearbyCars = Physics.OverlapBox(
            spawnPos,
            new Vector3(sideCheckRadius, 2.5f, forwardBackCheckDistance * 2f), // <-- más margen
            Quaternion.identity,
            carLayer
        );

        foreach (Collider col in nearbyCars)
        {
            if (col.gameObject == gameObject)
                continue;
            return false;
        }

        return true;
    }

    private void HandleLaneChange()
    {
        Vector3 pos = transform.position;
        float newX = Mathf.MoveTowards(pos.x, targetLaneX, laneChangeSpeed * Time.deltaTime);
        transform.position = new Vector3(newX, pos.y, pos.z);

        if (Mathf.Abs(transform.position.x - targetLaneX) < 0.05f)
        {
            transform.position = new Vector3(targetLaneX, transform.position.y, transform.position.z);
            isChangingLane = false;
        }
    }

    private void HandleDecisionTimer()
    {
        decisionTimer -= Time.deltaTime;

        if (decisionTimer > 0f || isChangingLane)
            return;

        decisionTimer = decisionInterval + Random.Range(-0.5f, 0.8f);

        if (Random.value < speedChangeChance)
        {
            ChangeSpeedRandomly();
        }

        if (Random.value < laneChangeChance)
        {
            TryLaneChange();
        }
    }

    private void ChangeSpeedRandomly()
    {
        float speedOffset = Random.Range(-2f, 2f);
        forwardSpeed = Mathf.Clamp(forwardSpeed + speedOffset, minSpeed, maxSpeed);
    }

    private void TryLaneChange()
    {
        Lane currentLane = GetCurrentLane();
        Lane targetLane = currentLane;

        bool isBehindPlayer = false;
        if (player != null)
        {
            float zDiff = player.position.z - transform.position.z;
            isBehindPlayer = zDiff > 0f && zDiff < preferredBehindDistance;
        }

        switch (currentLane)
        {
            case Lane.Left:
                targetLane = Lane.Middle;
                break;

            case Lane.Right:
                targetLane = Lane.Middle;
                break;

            case Lane.Middle:
                if (isBehindPlayer)
                {
                    bool playerInLeft = IsPlayerLane(leftLaneX);
                    bool playerInRight = IsPlayerLane(rightLaneX);

                    if (playerInLeft && !playerInRight)
                        targetLane = Lane.Right;
                    else if (playerInRight && !playerInLeft)
                        targetLane = Lane.Left;
                    else
                        targetLane = Random.value < 0.5f ? Lane.Left : Lane.Right;
                }
                else
                {
                    targetLane = Random.value < 0.5f ? Lane.Left : Lane.Right;
                }
                break;
        }

        float desiredX = GetLaneX(targetLane);

        if (isBehindPlayer && IsPlayerLane(desiredX))
            return;

        if (CanChangeToLane(desiredX))
        {
            targetLaneX = desiredX;
            isChangingLane = true;
        }
    }

    private bool CanChangeToLane(float desiredLaneX)
    {
        // Bloquea cambios peligrosos relacionados con el jugador y otros enemies
        if (ShouldBlockPlayerLaneChange(desiredLaneX))
            return false;

        // Bloqueo general: no cambiarse si el carril destino tiene otro carro
        if (IsLaneOccupiedByOtherCar(desiredLaneX))
            return false;

        return true;
    }

    private bool ShouldBlockPlayerLaneChange(float desiredLaneX)
    {
        if (player == null)
            return false;

        Lane currentLane = GetCurrentLane();

        //cuando el enemy esta en el carril del medio
        if (currentLane != Lane.Middle)
            return false;

        // si el jugador esta cerca del enemy en Z
        float zDistanceToPlayer = Mathf.Abs(player.position.z - transform.position.z);

        if (zDistanceToPlayer > playerLaneBlockDistanceZ)
            return false;

        // Si intenta irse al carril donde está el jugador, bloquear
        if (IsPlayerLane(desiredLaneX))
            return true;

        // Si intenta irse al carril libre, pero ese carril está ocupado por otro enemy, bloquear tambien
        if (IsLaneOccupiedByOtherCar(desiredLaneX))
            return true;

        return false;
    }

    private bool IsLaneOccupiedByOtherCar(float laneX)
    {
        Vector3 checkCenter = new Vector3(laneX, transform.position.y, transform.position.z);

        Collider[] nearbyCars = Physics.OverlapBox(
            checkCenter,
            new Vector3(sideCheckRadius, 2.5f, forwardBackCheckDistance * 1.5f),
            Quaternion.identity,
            carLayer
        );

        foreach (Collider col in nearbyCars)
        {
            if (col.gameObject == gameObject)
                continue;
            return true;
        }

        return false;
    }



    private bool IsPlayerLane(float laneX)
    {
        if (player == null)
            return false;

        return Mathf.Abs(player.position.x - laneX) < sameLaneTolerance;
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

    private float GetLaneX(Lane lane)
    {
        switch (lane)
        {
            case Lane.Left: return leftLaneX;
            case Lane.Middle: return middleLaneX;
            case Lane.Right: return rightLaneX;
        }

        return middleLaneX;
    }

    private int CountCarsBehindPlayer(float distance)
    {
        if (player == null)
            return 0;

        Collider[] hits = Physics.OverlapBox(
            new Vector3(player.position.x, transform.position.y, player.position.z - distance * 0.5f),
            new Vector3(10f, 2f, distance * 0.5f),
            Quaternion.identity,
            carLayer
        );

        int count = 0;

        foreach (Collider hit in hits)
        {
            if (hit.gameObject == gameObject)
                continue;

            if (hit.transform.position.z < player.position.z)
                count++;
        }

        return count;
    }

    private void OnDrawGizmosSelected()
    {
        if (player == null)
            return;

        Gizmos.color = Color.red;
        Vector3 behindCenter = new Vector3(transform.position.x, transform.position.y, transform.position.z + preferredBehindDistance * 0.5f);
        Gizmos.DrawWireCube(behindCenter, new Vector3(1f, 1f, preferredBehindDistance));

        Gizmos.color = Color.blue;
        Gizmos.DrawWireCube(transform.position, new Vector3(1f, 1f, avoidPlayerSideDistance * 2f));
        Gizmos.color = Color.green;
        Vector3 obstacleCheckCenter = transform.position + Vector3.forward * obstacleCheckDistance;
        Gizmos.DrawWireCube(obstacleCheckCenter, obstacleCheckHalfExtents * 2f);
    }
}