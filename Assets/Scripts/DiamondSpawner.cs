using UnityEngine;

public class DiamondSpawner : MonoBehaviour
{
    [SerializeField] private GameObject diamondPrefab;
    [SerializeField] private float minimumCheckInterval = 5f;
    [SerializeField] private float maximumCheckInterval = 10f;
    [SerializeField, Range(0f, 1f)] private float minimumSpawnChance = 0.05f;
    [SerializeField, Range(0f, 1f)] private float maximumSpawnChance = 0.10f;
    [SerializeField] private float outsidePadding = 1f;

    private Camera mainCamera;
    private float nextCheckTime;

    private void Awake()
    {
        mainCamera = Camera.main;
    }

    private void Start()
    {
        ScheduleNextCheck();
    }

    private void Update()
    {
        if (diamondPrefab == null || Time.time < nextCheckTime)
        {
            return;
        }

        float spawnChance = Random.Range(minimumSpawnChance, maximumSpawnChance);
        if (Random.value <= spawnChance)
        {
            SpawnDiamond();
        }

        ScheduleNextCheck();
    }

    private void SpawnDiamond()
    {
        float verticalExtent = mainCamera != null ? mainCamera.orthographicSize : 5f;
        float horizontalExtent = mainCamera != null
            ? verticalExtent * mainCamera.aspect
            : 8.9f;

        float centerX = mainCamera != null ? mainCamera.transform.position.x : 0f;
        float centerY = mainCamera != null ? mainCamera.transform.position.y : 0f;
        float verticalLimit = Mathf.Max(0.5f, verticalExtent - 1.25f);

        Vector3 spawnPosition = new Vector3(
            centerX + horizontalExtent + outsidePadding,
            Random.Range(centerY - verticalLimit, centerY + verticalLimit),
            0f);

        Instantiate(diamondPrefab, spawnPosition, diamondPrefab.transform.rotation);
    }

    private void ScheduleNextCheck()
    {
        nextCheckTime = Time.time + Random.Range(minimumCheckInterval, maximumCheckInterval);
    }
}
