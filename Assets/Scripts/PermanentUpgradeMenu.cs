using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PermanentUpgradeMenu : MonoBehaviour
{
    private GameObject upgradePanel;
    private TMP_Text currencyText;
    private TMP_Text statusText;

    private void Start()
    {
        Transform canvas = FindFirstObjectByType<Canvas>()?.transform;
        upgradePanel = canvas != null ? canvas.Find("UpgradePanel")?.gameObject : null;

        if (canvas == null || upgradePanel == null)
        {
            Debug.LogError("PermanentUpgradeMenu could not find Canvas/UpgradePanel.", this);
            return;
        }

        currencyText = upgradePanel.transform.Find("CurrencyText")?.GetComponent<TMP_Text>();
        statusText = upgradePanel.transform.Find("StatusText")?.GetComponent<TMP_Text>();

        WireButton(canvas, "UpgradeMenuButton", OpenPanel);
        WireButton(upgradePanel.transform, "BackButton", ClosePanel);
        WireUpgradeButton("AttackButton", PermanentUpgradeType.Attack);
        WireUpgradeButton("HealthButton", PermanentUpgradeType.Health);
        WireUpgradeButton("FireRateButton", PermanentUpgradeType.FireRate);
        WireUpgradeButton("MoveSpeedButton", PermanentUpgradeType.MoveSpeed);
        WireUpgradeButton("ProjectileButton", PermanentUpgradeType.ProjectileCount);

        PlayerProgress.CurrencyChanged += Refresh;
        PermanentUpgrades.Changed += Refresh;
        upgradePanel.SetActive(false);
        Refresh();
    }

    private void OnDestroy()
    {
        PlayerProgress.CurrencyChanged -= Refresh;
        PermanentUpgrades.Changed -= Refresh;
    }

    private void OpenPanel()
    {
        statusText.text = "Choose a permanent upgrade";
        upgradePanel.SetActive(true);
        Refresh();
    }

    private void ClosePanel()
    {
        upgradePanel.SetActive(false);
    }

    private void Purchase(PermanentUpgradeType type)
    {
        if (PermanentUpgrades.TryPurchase(type))
        {
            statusText.text = "Upgrade purchased!";
        }
        else if (PermanentUpgrades.GetLevel(type) >= PermanentUpgrades.GetMaxLevel(type))
        {
            statusText.text = "This upgrade is already MAX";
        }
        else
        {
            statusText.text = PermanentUpgrades.UsesDiamonds(type)
                ? "Not enough diamonds"
                : "Not enough coins";
        }

        Refresh();
    }

    private void Refresh()
    {
        if (upgradePanel == null)
        {
            return;
        }

        if (currencyText != null)
        {
            currencyText.text = $"Coins: {PlayerProgress.Coins}     Diamonds: {PlayerProgress.Diamonds}";
        }

        SetButtonLabel("AttackButton", PermanentUpgradeType.Attack, "ATTACK", "+1 damage");
        SetButtonLabel("HealthButton", PermanentUpgradeType.Health, "HEALTH", "+5 maximum HP");
        SetButtonLabel("FireRateButton", PermanentUpgradeType.FireRate, "FIRE RATE", "+5% attack speed");
        SetButtonLabel("MoveSpeedButton", PermanentUpgradeType.MoveSpeed, "MOVE SPEED", "+5% movement speed");
        SetButtonLabel("ProjectileButton", PermanentUpgradeType.ProjectileCount, "PROJECTILES", "+1 starting projectile");
    }

    private void SetButtonLabel(
        string objectName,
        PermanentUpgradeType type,
        string title,
        string effect)
    {
        Transform buttonTransform = upgradePanel.transform.Find(objectName);
        Button button = buttonTransform?.GetComponent<Button>();
        TMP_Text label = buttonTransform?.GetComponentInChildren<TMP_Text>();
        if (button == null || label == null)
        {
            return;
        }

        int level = PermanentUpgrades.GetLevel(type);
        int maxLevel = PermanentUpgrades.GetMaxLevel(type);
        bool isMax = level >= maxLevel;
        string currency = PermanentUpgrades.UsesDiamonds(type) ? "Diamonds" : "Coins";
        string costLine = isMax ? "MAX" : $"{PermanentUpgrades.GetCost(type)} {currency}";

        label.text = $"{title}  Lv.{level}/{maxLevel}\n{effect}  |  {costLine}";
        button.interactable = !isMax;
    }

    private void WireUpgradeButton(string objectName, PermanentUpgradeType type)
    {
        Transform buttonTransform = upgradePanel.transform.Find(objectName);
        Button button = buttonTransform?.GetComponent<Button>();
        if (button == null)
        {
            Debug.LogError($"PermanentUpgradeMenu could not find {objectName}.", this);
            return;
        }

        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(() => Purchase(type));
    }

    private static void WireButton(Transform root, string objectName, UnityEngine.Events.UnityAction action)
    {
        Button button = root.Find(objectName)?.GetComponent<Button>();
        if (button == null)
        {
            return;
        }

        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(action);
    }
}
