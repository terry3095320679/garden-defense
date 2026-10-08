using UnityEngine;

[RequireComponent(typeof(Collider2D), typeof(Rigidbody2D))]
public class DiamondPickup : MonoBehaviour
{
    [SerializeField] private int diamondAmount = 1;
    [SerializeField] private float lifetime = 10f;
    [SerializeField] private float driftSpeed = 1.5f;

    private float remainingLifetime;
    private Rigidbody2D body;

    private void Awake()
    {
        body = GetComponent<Rigidbody2D>();

        SpriteRenderer diamondRenderer = GetComponent<SpriteRenderer>();
        if (diamondRenderer != null)
        {
            Sprite diamondSprite = Resources.Load<Sprite>("Sprites/DiamondGem");
            if (diamondSprite != null)
            {
                diamondRenderer.sprite = diamondSprite;
                diamondRenderer.color = Color.white;
            }

            // Keep pickups above the ground, walls, enemies, and ordinary projectiles.
            diamondRenderer.sortingOrder = 20;
        }
    }

    private void OnEnable()
    {
        remainingLifetime = lifetime;
        body.linearVelocity = Vector2.left * driftSpeed;
    }

    private void Update()
    {
        remainingLifetime -= Time.deltaTime;

        if (remainingLifetime <= 0f)
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.GetComponent<PlayerHealth>() == null)
        {
            return;
        }

        PlayerProgress.AddDiamonds(diamondAmount);
        Destroy(gameObject);
    }

    private void OnDisable()
    {
        if (body != null)
        {
            body.linearVelocity = Vector2.zero;
        }
    }
}
