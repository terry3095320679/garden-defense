using System;
using UnityEngine;

public enum PermanentUpgradeType
{
    Attack,
    Health,
    FireRate,
    MoveSpeed,
    ProjectileCount
}

public static class PermanentUpgrades
{
    private const string KeyPrefix = "biubiubiu_Upgrade_";
    private const int AttackAndHealthMaxLevel = 100;
    private const int SpeedMaxLevel = 10;
    private const int ProjectileMaxLevel = 2;

    public static event Action Changed;

    public static int ProjectileCount => 1 + GetLevel(PermanentUpgradeType.ProjectileCount);

    public static int GetLevel(PermanentUpgradeType type)
    {
        return PlayerPrefs.GetInt(KeyPrefix + type, 0);
    }

    public static int GetMaxLevel(PermanentUpgradeType type)
    {
        return type switch
        {
            PermanentUpgradeType.Attack => AttackAndHealthMaxLevel,
            PermanentUpgradeType.Health => AttackAndHealthMaxLevel,
            PermanentUpgradeType.FireRate => SpeedMaxLevel,
            PermanentUpgradeType.MoveSpeed => SpeedMaxLevel,
            PermanentUpgradeType.ProjectileCount => ProjectileMaxLevel,
            _ => 0
        };
    }

    public static int GetCost(PermanentUpgradeType type)
    {
        int level = GetLevel(type);

        return type switch
        {
            PermanentUpgradeType.Attack => 20 * (level + 1),
            PermanentUpgradeType.Health => 20 * (level + 1),
            PermanentUpgradeType.FireRate => 3,
            PermanentUpgradeType.MoveSpeed => 3,
            PermanentUpgradeType.ProjectileCount => level == 0 ? 10 : 30,
            _ => 0
        };
    }

    public static bool UsesDiamonds(PermanentUpgradeType type)
    {
        return type == PermanentUpgradeType.FireRate
            || type == PermanentUpgradeType.MoveSpeed
            || type == PermanentUpgradeType.ProjectileCount;
    }

    public static bool TryPurchase(PermanentUpgradeType type)
    {
        int level = GetLevel(type);
        if (level >= GetMaxLevel(type))
        {
            return false;
        }

        int cost = GetCost(type);
        bool spent = UsesDiamonds(type)
            ? PlayerProgress.TrySpendDiamonds(cost)
            : PlayerProgress.TrySpendCoins(cost);

        if (!spent)
        {
            return false;
        }

        PlayerPrefs.SetInt(KeyPrefix + type, level + 1);
        PlayerPrefs.Save();
        Changed?.Invoke();
        return true;
    }

    public static float ApplyAttack(float baseAttack)
    {
        return baseAttack + GetLevel(PermanentUpgradeType.Attack);
    }

    public static int ApplyHealth(int baseHealth)
    {
        return baseHealth + GetLevel(PermanentUpgradeType.Health) * 5;
    }

    public static float ApplyFireRate(float baseMultiplier = 1f)
    {
        return baseMultiplier * (1f + GetLevel(PermanentUpgradeType.FireRate) * 0.05f);
    }

    public static float ApplyMoveSpeed(float baseMoveSpeed)
    {
        return baseMoveSpeed * (1f + GetLevel(PermanentUpgradeType.MoveSpeed) * 0.05f);
    }
}
