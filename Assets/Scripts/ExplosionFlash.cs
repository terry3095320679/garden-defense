using UnityEngine;

public class ExplosionFlash : MonoBehaviour
{
    private const float Duration = 0.22f;

    private SpriteRenderer spriteRenderer;
    private float elapsed;
    private Vector3 targetScale;

    public static void Create(Vector3 position, SpriteRenderer sourceRenderer, float radius)
    {
        Sprite explosionSprite = Resources.Load<Sprite>("Sprites/WatermelonExplosion");

        if (sourceRenderer == null || explosionSprite == null)
        {
            return;
        }

        GameObject flashObject = new GameObject("ExplosionFlash");
        flashObject.transform.position = position;
        flashObject.transform.localScale = Vector3.zero;

        SpriteRenderer flashRenderer = flashObject.AddComponent<SpriteRenderer>();
        flashRenderer.sprite = explosionSprite;
        flashRenderer.material = sourceRenderer.material;
        flashRenderer.sortingLayerID = sourceRenderer.sortingLayerID;
        flashRenderer.sortingOrder = sourceRenderer.sortingOrder + 1;
        flashRenderer.color = new Color(1f, 1f, 1f, 0.9f);

        ExplosionFlash flash = flashObject.AddComponent<ExplosionFlash>();
        flash.spriteRenderer = flashRenderer;
        float diameter = radius * 2f;
        Vector2 spriteWorldSize = explosionSprite.bounds.size;
        flash.targetScale = new Vector3(
            diameter / Mathf.Max(0.01f, spriteWorldSize.x),
            diameter / Mathf.Max(0.01f, spriteWorldSize.y),
            1f);
    }

    private void Update()
    {
        elapsed += Time.deltaTime;
        float progress = Mathf.Clamp01(elapsed / Duration);
        float easedProgress = 1f - Mathf.Pow(1f - progress, 3f);

        transform.localScale = targetScale * easedProgress;

        Color color = spriteRenderer.color;
        color.a = Mathf.Lerp(0.8f, 0f, progress);
        spriteRenderer.color = color;

        if (progress >= 1f)
        {
            Destroy(gameObject);
        }
    }
}
