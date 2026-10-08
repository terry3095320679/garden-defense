using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] private GameObject enemyPrefab;
    [SerializeField] private float spawnInterval = 1.25f;
    [SerializeField] private float timeToMaximumSpawnRate = 120f;
    [SerializeField] private float maximumSpawnRateMultiplier = 3f;
    [SerializeField] private float entranceCenterY = 0f;
    [SerializeField] private float verticalWallMargin = 1.5f;
    [SerializeField] private float outsidePadding = 1f;

    private float nextSpawnTime;
    private Camera mainCamera;

    private void Awake()
    {
        mainCamera = Camera.main;
    }

    private void Update()
    {
        if (enemyPrefab == null || Time.time < nextSpawnTime)
        {
            return;
        }

        float verticalExtent = mainCamera != null ? mainCamera.orthographicSize : 8f;
        float horizontalExtent = mainCamera != null
            ? verticalExtent * mainCamera.aspect
            : 4.5f;

        float centerX = mainCamera != null ? mainCamera.transform.position.x : 0f;
        float centerY = mainCamera != null ? mainCamera.transform.position.y : 0f;
        float spawnX = centerX + horizontalExtent + outsidePadding;
        float verticalSpawnExtent = Mathf.Max(0.5f, verticalExtent - verticalWallMargin);
        float minimumY = centerY + entranceCenterY - verticalSpawnExtent;
        float maximumY = centerY + entranceCenterY + verticalSpawnExtent;

        Vector3 spawnPosition = new Vector3(
            spawnX,
            Random.Range(minimumY, maximumY),
            0f);

        Instantiate(enemyPrefab, spawnPosition, Quaternion.identity);
        float progress = Mathf.Clamp01(Time.timeSinceLevelLoad / timeToMaximumSpawnRate);
        float spawnRateMultiplier = Mathf.Lerp(1f, maximumSpawnRateMultiplier, progress);
        nextSpawnTime = Time.time + spawnInterval / spawnRateMultiplier;
    }
}
