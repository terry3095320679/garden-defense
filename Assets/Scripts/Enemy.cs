using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(Collider2D))]
public class Enemy : MonoBehaviour
{
    [SerializeField] private float baseHealth = 50f;
    [SerializeField] private float moveSpeed = 2f;
    [SerializeField] private float healthGrowthMultiplier = 1.10f;
    [SerializeField] private float visualWorldSize = 1.25f;

    private Rigidbody2D body;
    private float currentHealth;
    private Camera mainCamera;
    private bool defeated;
    private float slowUntil;
    private float slowMultiplier = 1f;

    private void Awake()
    {
        body = GetComponent<Rigidbody2D>();
        mainCamera = Camera.main;
        ApplyEnemySprite();
    }

#if UNITY_EDITOR
    private void OnEnable()
    {
        if (!Application.isPlaying)
        {
            ApplyEnemySprite();
        }
    }

    private void OnValidate()
    {
        if (!Application.isPlaying)
        {
            ApplyEnemySprite();
        }
    }
#endif

    private void ApplyEnemySprite()
    {
        Sprite enemySprite = Resources.Load<Sprite>("Sprites/WiltedFlowerEnemy");
        SpriteRenderer visualRenderer = transform.Find("Visual")?.GetComponent<SpriteRenderer>();

        if (enemySprite == null || visualRenderer == null)
        {
            return;
        }

        visualRenderer.sprite = enemySprite;
        visualRenderer.color = Color.white;
        visualRenderer.transform.localRotation = Quaternion.identity;

        float largestDimension = Mathf.Max(enemySprite.bounds.size.x, enemySprite.bounds.size.y);
        float uniformScale = visualWorldSize / Mathf.Max(0.01f, largestDimension);
        visualRenderer.transform.localScale = Vector3.one * uniformScale;
    }

    private void Start()
    {
        int healthStages = Mathf.FloorToInt(Time.timeSinceLevelLoad / 10f);
        currentHealth = baseHealth * Mathf.Pow(healthGrowthMultiplier, healthStages);
        body.linearVelocity = Vector2.left * moveSpeed;
    }

    private void FixedUpdate()
    {
        float currentSpeedMultiplier = Time.time < slowUntil ? slowMultiplier : 1f;
        body.linearVelocity = Vector2.left * moveSpeed * currentSpeedMultiplier;
    }

    private void Update()
    {
        float leftEdge = mainCamera != null
            ? mainCamera.transform.position.x - mainCamera.orthographicSize * mainCamera.aspect - 1f
            : -10f;

        if (transform.position.x < leftEdge)
        {
            defeated = true;
            Destroy(gameObject);
        }
    }

    public void TakeDamage(float damage)
    {
        if (defeated)
        {
            return;
        }

        currentHealth -= damage;

        if (currentHealth <= 0f)
        {
            defeated = true;
            PlayerProgress.AddCoins(1);
            RunProgress.Instance?.RecordKill();
            Destroy(gameObject);
        }
    }

    public void ApplySlow(float duration, float speedMultiplier)
    {
        if (Time.time >= slowUntil)
        {
            slowMultiplier = Mathf.Clamp(speedMultiplier, 0.1f, 1f);
        }
        else
        {
            slowMultiplier = Mathf.Min(slowMultiplier, Mathf.Clamp(speedMultiplier, 0.1f, 1f));
        }

        slowUntil = Mathf.Max(slowUntil, Time.time + duration);
    }

    public void ApplyKnockback(float distance)
    {
        body.position += Vector2.right * Mathf.Max(0f, distance);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.TryGetComponent(out PlayerHealth playerHealth))
        {
            playerHealth.TakeDamage(10);
            defeated = true;
            Destroy(gameObject);
        }
    }

}
