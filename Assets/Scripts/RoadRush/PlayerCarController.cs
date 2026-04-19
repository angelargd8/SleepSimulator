using TMPro;
using UnityEngine;

public class PlayerCarController : MonoBehaviour
{
    [Header("Carriles")]
    [SerializeField] private float leftLaneX = -4.3f;
    [SerializeField] private float middleLaneX = -1.29f;
    [SerializeField] private float rightLaneX = 1.4f;

    [Header("Movimiento lateral")]
    [SerializeField] private float laneChangeSpeed = 8f;

    [Header("Boost automatico en Z")]
    [SerializeField] private float boostSpeed = 2f;
    [SerializeField] private float boostDuration = 0.5f;
    [SerializeField] private float boostCooldown = 2f;

    [Header("Boost con Shift")]
    [SerializeField] private float manualBoostSpeed = 8f;
    [SerializeField] private float manualBoostDuration = 1.5f;
    [SerializeField] private float manualBoostCooldown = 0.01f;
    [SerializeField] private int manualBoostCharges = 5;
    [SerializeField] private TMP_Text boostText;

    [Header("Detección enemigo detrás")]
    [SerializeField] private float rearCheckDistance = 0.25f;
    [SerializeField] private Vector3 rearCheckBoxHalfExtents = new Vector3(0.5f, 0.8f, 0.08f);
    [SerializeField] private LayerMask enemyCarLayer;

    [Header("Vida")]
    [SerializeField] private int lives = 3;
    [SerializeField] private TMP_Text livesText;

    [Header("Score")]
    [SerializeField] private int score = 0;
    private float scoreAccumulator = 0f;
    [SerializeField] private TMP_Text scoreText;

    [Header("Bloqueo de carril")]
    [SerializeField] private Vector3 laneBlockCheckHalfExtents = new Vector3(0.8f, 1.0f, 1.0f);
    [SerializeField] private float laneBlockSameZTolerance = 1.2f;

    [Header("Daño")]
    [SerializeField] private float invulnerabilityTime = 1f;
    private bool isInvulnerable = false;

    [Header("Lose Dream Controller")]
    [SerializeField] private LoseDreamController loseDreamController;

    private float targetLaneX;

    // Boost automatico
    private float boostTimer = 0f;
    private float cooldownTimer = 0f;

    // Boost manual
    private float manualBoostTimer = 0f;
    private float manualBoostCooldownTimer = 0f;

    private enum Lane
    {
        Left,
        Middle,
        Right
    }

    private void Start()
    {
        targetLaneX = GetClosestLaneX();
        UpdateLivesUI();
        UpdateScoreUI();
        UpdateBoostUI();
    }

    private void Update()
    {
        HandleInput();
        HandleLaneChange();
        HandleBoost();
        HandleManualBoost();
        UpdateScore();
    }

    private void UpdateScore()
    {
        scoreAccumulator += 10f * Time.deltaTime;

        if (boostTimer > 0f)
        {
            scoreAccumulator += boostSpeed * 5f * Time.deltaTime;
        }

        if (manualBoostTimer > 0f)
        {
            scoreAccumulator += manualBoostSpeed * 6f * Time.deltaTime;
        }

        score = Mathf.FloorToInt(scoreAccumulator);
        UpdateScoreUI();
    }

    private void UpdateScoreUI()
    {
        if (scoreText != null)
        {
            scoreText.text = "Score: " + score;
        }
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

        if (Input.GetKeyDown(KeyCode.LeftShift) || Input.GetKeyDown(KeyCode.RightShift))
        {
            TryUseManualBoost();
        }
    }

    private void TryUseManualBoost()
    {
        if (manualBoostCharges <= 0)
            return;

        if (manualBoostCooldownTimer > 0f)
            return;

        if (manualBoostTimer > 0f)
            return;

        manualBoostCharges--;
        manualBoostTimer = manualBoostDuration;
        manualBoostCooldownTimer = manualBoostCooldown;
        UpdateBoostUI();
    }

    private void MoveLeft()
    {
        Lane currentLane = GetCurrentLane();
        float desiredLaneX = transform.position.x;

        switch (currentLane)
        {
            case Lane.Middle:
                desiredLaneX = leftLaneX;
                break;
            case Lane.Right:
                desiredLaneX = middleLaneX;
                break;
            default:
                return;
        }

        if (IsLaneBlocked(desiredLaneX))
        {
            TakeDamage();
            return;
        }

        targetLaneX = desiredLaneX;
    }

    private void MoveRight()
    {
        Lane currentLane = GetCurrentLane();
        float desiredLaneX = transform.position.x;

        switch (currentLane)
        {
            case Lane.Middle:
                desiredLaneX = rightLaneX;
                break;
            case Lane.Left:
                desiredLaneX = middleLaneX;
                break;
            default:
                return;
        }

        if (IsLaneBlocked(desiredLaneX))
        {
            TakeDamage();
            return;
        }

        targetLaneX = desiredLaneX;
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

    private void HandleManualBoost()
    {
        if (manualBoostCooldownTimer > 0f)
            manualBoostCooldownTimer -= Time.deltaTime;

        if (manualBoostTimer > 0f)
        {
            manualBoostTimer -= Time.deltaTime;
            transform.position += Vector3.forward * manualBoostSpeed * Time.deltaTime;
        }
    }

    private bool IsEnemyBehind()
    {
        Vector3 checkCenter = transform.position + Vector3.back * (rearCheckDistance * 0.35f);

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

            bool behindPlayer = hit.transform.position.z < transform.position.z;
            bool sameLane = Mathf.Abs(hit.transform.position.x - transform.position.x) < 0.35f;

            if (behindPlayer && sameLane)
                return true;
        }

        return false;
    }

    private bool IsLaneBlocked(float desiredLaneX)
    {
        Vector3 checkCenter = new Vector3(
            desiredLaneX,
            transform.position.y,
            transform.position.z
        );

        Collider[] hits = Physics.OverlapBox(
            checkCenter,
            laneBlockCheckHalfExtents,
            Quaternion.identity,
            enemyCarLayer
        );

        foreach (Collider hit in hits)
        {
            if (hit.gameObject == gameObject)
                continue;

            bool closeInZ = Mathf.Abs(hit.transform.position.z - transform.position.z) <= laneBlockSameZTolerance;

            if (closeInZ)
                return true;
        }

        return false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            TakeDamage();
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            TakeDamage();
        }
    }

    private void TakeDamage()
    {
        if (isInvulnerable)
            return;

        lives--;
        isInvulnerable = true;

        Debug.Log("Golpe! Vidas restantes: " + lives);
        UpdateLivesUI();

        Invoke(nameof(ResetInvulnerability), invulnerabilityTime);

        if (lives <= 0)
        {
            GameOver();
        }
    }

    private void ResetInvulnerability()
    {
        isInvulnerable = false;
    }

    private void UpdateLivesUI()
    {
        if (livesText != null)
        {
            livesText.text = "Choques restantes: " + lives;
        }
    }

    private void UpdateBoostUI()
    {
        if (boostText != null)
        {
            boostText.text = "Boost: " + manualBoostCharges;
        }
    }

    private void GameOver()
    {
        //Debug.Log("GAME OVER");
        //Time.timeScale = 0f;
        loseDreamController.PlayerLost();
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
        Vector3 rearCheckCenter = transform.position + Vector3.back * (rearCheckDistance * 0.35f);
        Gizmos.DrawWireCube(rearCheckCenter, rearCheckBoxHalfExtents * 2f);

        Gizmos.color = Color.yellow;
        Vector3 laneCheckLeft = new Vector3(leftLaneX, transform.position.y, transform.position.z);
        Vector3 laneCheckMiddle = new Vector3(middleLaneX, transform.position.y, transform.position.z);
        Vector3 laneCheckRight = new Vector3(rightLaneX, transform.position.y, transform.position.z);

        Gizmos.DrawWireCube(laneCheckLeft, laneBlockCheckHalfExtents * 2f);
        Gizmos.DrawWireCube(laneCheckMiddle, laneBlockCheckHalfExtents * 2f);
        Gizmos.DrawWireCube(laneCheckRight, laneBlockCheckHalfExtents * 2f);
    }
}