using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 6f;
    [SerializeField] private float visualWorldSize = 1.35f;

    private Rigidbody2D body;
    private Vector2 moveInput;

    private void Awake()
    {
        body = GetComponent<Rigidbody2D>();
        moveSpeed = PermanentUpgrades.ApplyMoveSpeed(moveSpeed);
        ApplyPlayerSprite();
    }

#if UNITY_EDITOR
    private void OnEnable()
    {
        if (!Application.isPlaying)
        {
            ApplyPlayerSprite();
        }
    }

    private void OnValidate()
    {
        if (!Application.isPlaying)
        {
            ApplyPlayerSprite();
        }
    }
#endif

    private void ApplyPlayerSprite()
    {
        Sprite playerSprite = Resources.Load<Sprite>("Sprites/PlayerSproutHero");
        SpriteRenderer visualRenderer = transform.Find("Visual")?.GetComponent<SpriteRenderer>();

        if (playerSprite == null || visualRenderer == null)
        {
            return;
        }

        visualRenderer.sprite = playerSprite;
        visualRenderer.color = Color.white;
        visualRenderer.transform.localRotation = Quaternion.identity;

        float largestDimension = Mathf.Max(playerSprite.bounds.size.x, playerSprite.bounds.size.y);
        float uniformScale = visualWorldSize / Mathf.Max(0.01f, largestDimension);
        visualRenderer.transform.localScale = Vector3.one * uniformScale;
    }

    public void OnMove(InputValue value)
    {
        moveInput = value.Get<Vector2>();
    }

    private void FixedUpdate()
    {
        body.linearVelocity = moveInput * moveSpeed;
    }

    private void OnDisable()
    {
        moveInput = Vector2.zero;

        if (body != null)
        {
            body.linearVelocity = Vector2.zero;
        }
    }
}
