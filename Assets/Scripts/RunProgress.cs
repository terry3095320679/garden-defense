using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RunProgress : MonoBehaviour
{
    public static RunProgress Instance { get; private set; }

    [SerializeField] private Slider experienceSlider;
    [SerializeField] private TMP_Text experienceText;
    [SerializeField] private int firstUpgradeRequirement = 10;
    [SerializeField] private int requirementIncrease = 5;

    public event Action UpgradeEarned;

    public int TotalKills { get; private set; }
    public int KillsTowardNextUpgrade { get; private set; }
    public int NextUpgradeRequirement { get; private set; }
    public int PendingUpgrades { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        NextUpgradeRequirement = Mathf.Max(1, firstUpgradeRequirement);
        UpdateDisplay();
    }

    public void RecordKill()
    {
        TotalKills++;
        KillsTowardNextUpgrade++;

        if (KillsTowardNextUpgrade >= NextUpgradeRequirement)
        {
            KillsTowardNextUpgrade -= NextUpgradeRequirement;
            PendingUpgrades++;
            NextUpgradeRequirement += Mathf.Max(1, requirementIncrease);

            UpgradeEarned?.Invoke();
        }

        UpdateDisplay();
    }

    public bool ConsumePendingUpgrade()
    {
        if (PendingUpgrades <= 0)
        {
            return false;
        }

        PendingUpgrades--;
        return true;
    }

    private void UpdateDisplay()
    {
        if (experienceSlider != null)
        {
            experienceSlider.minValue = 0f;
            experienceSlider.maxValue = NextUpgradeRequirement;
            experienceSlider.value = KillsTowardNextUpgrade;
        }

        if (experienceText != null)
        {
            experienceText.text = $"EXP ({KillsTowardNextUpgrade}/{NextUpgradeRequirement})";
        }
    }
}
