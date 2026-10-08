using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Bullet : MonoBehaviour
{
    [SerializeField] private float lifetime = 3f;
    [SerializeField] private float homingTurnSpeed = 8f;
    [SerializeField] private float freezeDuration = 2f;
    [SerializeField, Range(0.1f, 1f)] private float freezeSpeedMultiplier = 0.45f;
    [SerializeField] private float knockbackDistance = 0.8f;
    [SerializeField] private float baseExplosionRadius = 1.25f;
    [SerializeField] private float baseColdSlashRange = 3f;
    [SerializeField] private float baseColdSlashWidth = 2f;
    [SerializeField] private float coldSlashVisualLength = 0.45f;

    private Rigidbody2D body;
    private SpriteRenderer spriteRenderer;
    private BoxCollider2D boxCollider;
    private SimplePool owningPool;
    private float remainingLifetime;
    private float damage;
    private float splashDamage;
    private float speed;
    private float areaMultiplier;
    private float rangeMultiplier;
    private float sizeMultiplier;
    private WeaponType weaponType;
    private Vector3 originalScale;
    private Color originalColor;
    private Sprite originalSprite;
    private Vector2 originalColliderSize;
    private Sprite basicSprite;
    private Sprite laserSprite;
    private Sprite coldSlashSprite;
    private Sprite knockbackSprite;
    private Sprite homingSprite;
    private Sprite explosiveSprite;
    private readonly HashSet<Enemy> laserHits = new();

    private void Awake()
    {
        body = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        boxCollider = GetComponent<BoxCollider2D>();
        originalScale = transform.localScale;
        originalColor = spriteRenderer != null ? spriteRenderer.color : Color.white;
        originalSprite = spriteRenderer != null ? spriteRenderer.sprite : null;
        originalColliderSize = boxCollider != null ? boxCollider.size : Vector2.one;
        basicSprite = Resources.Load<Sprite>("Sprites/PeaProjectile");
        laserSprite = Resources.Load<Sprite>("Sprites/CactusPiercer");
        coldSlashSprite = Resources.Load<Sprite>("Sprites/ColdLeafSlash");
        knockbackSprite = Resources.Load<Sprite>("Sprites/AcornKnockback");
        homingSprite = Resources.Load<Sprite>("Sprites/WingedHomingSeed");
        explosiveSprite = Resources.Load<Sprite>("Sprites/WatermelonProjectile");
    }

    private void OnEnable()
    {
        remainingLifetime = lifetime;
        laserHits.Clear();
    }

    public void Launch(
        SimplePool pool,
        Vector2 direction,
        float launchSpeed,
        WeaponType launchedWeaponType,
        float launchedDamage,
        float launchedSplashDamage,
        float launchedAreaMultiplier,
        float launchedRangeMultiplier,
        float launchedSizeMultiplier)
    {
        owningPool = pool;
        speed = launchSpeed;
        weaponType = launchedWeaponType;
        damage = launchedDamage;
        splashDamage = launchedSplashDamage;
        areaMultiplier = launchedAreaMultiplier;
        rangeMultiplier = launchedRangeMultiplier;
        sizeMultiplier = launchedSizeMultiplier;
        ConfigureAppearance();
        body.linearVelocity = direction.normalized * speed;

        if (weaponType == WeaponType.Freeze)
        {
            remainingLifetime = baseColdSlashRange * rangeMultiplier / Mathf.Max(0.01f, speed);
        }
    }

    private void Update()
    {
        remainingLifetime -= Time.deltaTime;

        if (remainingLifetime <= 0f)
        {
            ReturnToPool();
        }
    }

    private void FixedUpdate()
    {
        if (weaponType != WeaponType.Homing)
        {
            return;
        }

        Enemy nearestEnemy = FindNearestEnemy();
        if (nearestEnemy == null)
        {
            return;
        }

        Vector2 desiredDirection = ((Vector2)nearestEnemy.transform.position - body.position).normalized;
        Vector2 currentDirection = body.linearVelocity.normalized;
        Vector2 newDirection = Vector2.Lerp(currentDirection, desiredDirection, homingTurnSpeed * Time.fixedDeltaTime).normalized;
        body.linearVelocity = newDirection * speed;
        transform.right = newDirection;
    }

    private void OnDisable()
    {
        if (body != null)
        {
            body.linearVelocity = Vector2.zero;
        }

        transform.localScale = originalScale;
        if (spriteRenderer != null)
        {
            spriteRenderer.color = originalColor;
            spriteRenderer.sprite = originalSprite;
        }

        if (boxCollider != null)
        {
            boxCollider.size = originalColliderSize;
        }
    }

    public void ReturnToPool()
    {
        if (owningPool != null)
        {
            owningPool.ReturnToPool(gameObject);
        }
        else
        {
            gameObject.SetActive(false);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.TryGetComponent(out Enemy enemy))
        {
            if (weaponType == WeaponType.Laser)
            {
                if (laserHits.Add(enemy))
                {
                    enemy.TakeDamage(damage);
                }
                return;
            }

            if (weaponType == WeaponType.Freeze)
            {
                if (laserHits.Add(enemy))
                {
                    enemy.TakeDamage(damage);
                    enemy.ApplySlow(freezeDuration, freezeSpeedMultiplier);
                }
                return;
            }

            if (weaponType == WeaponType.Explosive)
            {
                enemy.TakeDamage(damage);
                Explode();
                ReturnToPool();
                return;
            }

            enemy.TakeDamage(damage);

            if (weaponType == WeaponType.Knockback)
            {
                enemy.ApplyKnockback(knockbackDistance);
            }

            ReturnToPool();
        }
    }

    private void ConfigureAppearance()
    {
        if (spriteRenderer == null)
        {
            return;
        }

        transform.rotation = Quaternion.identity;

        switch (weaponType)
        {
            case WeaponType.Laser:
                ConfigureSprite(laserSprite, 1.4f, 0.28f * areaMultiplier);
                break;
            case WeaponType.Freeze:
                ConfigureColdSlash();
                break;
            case WeaponType.Knockback:
                ConfigureSprite(knockbackSprite, 0.65f * sizeMultiplier, 0.48f * sizeMultiplier);
                break;
            case WeaponType.Homing:
                ConfigureSprite(homingSprite, 0.52f * sizeMultiplier, 0.42f * sizeMultiplier);
                break;
            case WeaponType.Explosive:
                ConfigureSprite(explosiveSprite, 0.36f * sizeMultiplier, 0.26f * sizeMultiplier);
                break;
            default:
                ConfigureSprite(basicSprite, 0.48f * sizeMultiplier, 0.34f * sizeMultiplier);
                break;
        }
    }

    private void ConfigureSprite(Sprite sprite, float targetWidth, float targetHeight)
    {
        spriteRenderer.color = Color.white;

        if (sprite == null)
        {
            spriteRenderer.sprite = originalSprite;
            transform.localScale = originalScale * sizeMultiplier;
            return;
        }

        spriteRenderer.sprite = sprite;
        Vector2 spriteWorldSize = sprite.bounds.size;
        transform.localScale = new Vector3(
            targetWidth / Mathf.Max(0.01f, spriteWorldSize.x),
            targetHeight / Mathf.Max(0.01f, spriteWorldSize.y),
            1f);

        ConfigureColliderForWorldSize(targetWidth * 0.75f, targetHeight * 0.75f);
    }

    private void ConfigureColliderForWorldSize(float worldWidth, float worldHeight)
    {
        if (boxCollider == null)
        {
            return;
        }

        boxCollider.size = new Vector2(
            worldWidth / Mathf.Max(0.01f, Mathf.Abs(transform.localScale.x)),
            worldHeight / Mathf.Max(0.01f, Mathf.Abs(transform.localScale.y)));
    }

    private void ConfigureColdSlash()
    {
        spriteRenderer.color = Color.white;

        float targetWidth = baseColdSlashWidth * areaMultiplier;
        ConfigureSprite(coldSlashSprite, coldSlashVisualLength, targetWidth);
        ConfigureColliderForWorldSize(coldSlashVisualLength, targetWidth);
    }

    private Enemy FindNearestEnemy()
    {
        Enemy[] enemies = FindObjectsByType<Enemy>(FindObjectsSortMode.None);
        Enemy nearestEnemy = null;
        float nearestDistanceSquared = float.MaxValue;

        foreach (Enemy enemy in enemies)
        {
            float distanceSquared = ((Vector2)enemy.transform.position - body.position).sqrMagnitude;
            if (distanceSquared < nearestDistanceSquared)
            {
                nearestDistanceSquared = distanceSquared;
                nearestEnemy = enemy;
            }
        }

        return nearestEnemy;
    }

    private void Explode()
    {
        float radius = baseExplosionRadius * areaMultiplier;
        ExplosionFlash.Create(transform.position, spriteRenderer, radius);
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, radius);
        HashSet<Enemy> damagedEnemies = new HashSet<Enemy>();

        foreach (Collider2D hit in hits)
        {
            if (hit.TryGetComponent(out Enemy enemy) && damagedEnemies.Add(enemy))
            {
                enemy.TakeDamage(splashDamage);
            }
        }
    }
}
