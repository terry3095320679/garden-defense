using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(RunProgress))]
public class UpgradeChoicePanel : MonoBehaviour
{
    private readonly WeaponType[] weaponChoices =
    {
        WeaponType.Laser,
        WeaponType.Freeze,
        WeaponType.Knockback,
        WeaponType.Homing,
        WeaponType.Explosive
    };

    private readonly WeaponUpgradeOption[] displayedChoices = new WeaponUpgradeOption[3];

    private RunProgress runProgress;
    private PlayerShooter playerShooter;
    private GameObject panel;
    private Button[] buttons;
    private TMP_Text[] buttonTexts;

    private void Start()
    {
        runProgress = GetComponent<RunProgress>();
        playerShooter = FindFirstObjectByType<PlayerShooter>();
        if (playerShooter == null)
        {
            Debug.LogError("UpgradeChoicePanel could not find PlayerShooter.", this);
            enabled = false;
            return;
        }

        Canvas canvas = FindFirstObjectByType<Canvas>();
        Transform panelTransform = canvas != null ? canvas.transform.Find("UpgradePanel") : null;
        if (panelTransform == null)
        {
            Debug.LogError("UpgradeChoicePanel could not find Canvas/UpgradePanel.", this);
            enabled = false;
            return;
        }

        panel = panelTransform.gameObject;
        buttons = new Button[3];
        buttonTexts = new TMP_Text[3];

        for (int i = 0; i < buttons.Length; i++)
        {
            Transform buttonTransform = panelTransform.Find($"ChoiceButton{i + 1}");
            if (buttonTransform == null)
            {
                Debug.LogError($"UpgradeChoicePanel could not find ChoiceButton{i + 1}.", this);
                enabled = false;
                return;
            }

            buttons[i] = buttonTransform.GetComponent<Button>();
            buttonTexts[i] = buttonTransform.GetComponentInChildren<TMP_Text>(true);

            int choiceIndex = i;
            buttons[i].onClick.AddListener(() => SelectChoice(choiceIndex));
        }

        panel.SetActive(false);
        runProgress.UpgradeEarned += ShowChoices;
    }

    private void OnDestroy()
    {
        if (runProgress != null)
        {
            runProgress.UpgradeEarned -= ShowChoices;
        }
    }

    private void ShowChoices()
    {
        List<WeaponType> availableChoices = new List<WeaponType>(weaponChoices);

        for (int i = 0; i < displayedChoices.Length; i++)
        {
            int randomIndex = Random.Range(0, availableChoices.Count);
            WeaponType weaponType = availableChoices[randomIndex];
            availableChoices.RemoveAt(randomIndex);

            bool unlocksWeapon = !playerShooter.IsWeaponUnlocked(weaponType);
            WeaponStat stat = unlocksWeapon ? WeaponStat.Damage : GetRandomStat(weaponType);
            displayedChoices[i] = new WeaponUpgradeOption(weaponType, stat, unlocksWeapon);
            buttonTexts[i].text = FormatChoice(displayedChoices[i]);
        }

        panel.SetActive(true);
        Time.timeScale = 0f;
    }

    private void SelectChoice(int choiceIndex)
    {
        WeaponUpgradeOption selectedOption = displayedChoices[choiceIndex];
        playerShooter.ApplyUpgrade(selectedOption);
        runProgress.ConsumePendingUpgrade();
        panel.SetActive(false);
        Time.timeScale = 1f;
    }

    private WeaponStat GetRandomStat(WeaponType weaponType)
    {
        WeaponStat[] possibleStats;

        if (weaponType == WeaponType.Freeze)
        {
            possibleStats = new[]
            {
                WeaponStat.Damage,
                WeaponStat.FireRate,
                WeaponStat.Range,
                WeaponStat.Area
            };
        }
        else if (weaponType == WeaponType.Laser || weaponType == WeaponType.Explosive)
        {
            possibleStats = new[] { WeaponStat.Damage, WeaponStat.FireRate, WeaponStat.Area };
        }
        else
        {
            possibleStats = new[] { WeaponStat.Damage, WeaponStat.FireRate, WeaponStat.ProjectileCount };
        }

        return possibleStats[Random.Range(0, possibleStats.Length)];
    }

    private string FormatChoice(WeaponUpgradeOption option)
    {
        string weaponName = option.Weapon == WeaponType.Freeze
            ? "COLD SLASH"
            : option.Weapon.ToString().ToUpperInvariant() + " BULLET";
        if (option.UnlocksWeapon)
        {
            return weaponName + "\nUNLOCK";
        }

        string statName = option.Stat switch
        {
            WeaponStat.Damage => "DAMAGE +50%",
            WeaponStat.FireRate => "FIRE RATE +50%",
            WeaponStat.ProjectileCount => "PROJECTILE +1",
            WeaponStat.Range when option.Weapon == WeaponType.Freeze => "SLASH RANGE +25%",
            WeaponStat.Area when option.Weapon == WeaponType.Freeze => "SLASH WIDTH +25%",
            WeaponStat.Area when option.Weapon == WeaponType.Laser => "LASER WIDTH +50%",
            WeaponStat.Area => "EXPLOSION RANGE +50%",
            _ => "UPGRADE"
        };

        return weaponName + "\n" + statName;
    }
}
