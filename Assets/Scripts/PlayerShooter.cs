using System.Collections.Generic;
using UnityEngine;

public class PlayerShooter : MonoBehaviour
{
    [SerializeField] private SimplePool bulletPool;
    [SerializeField] private Transform firePoint;
    [SerializeField] private float bulletSpeed = 12f;
    [SerializeField] private float fireInterval = 0.25f;
    [SerializeField] private float baseAttackPower = 10f;
    [SerializeField] private float projectileSpacing = 0.45f;

    private class WeaponState
    {
        public bool Unlocked;
        public float DamageUpgradeMultiplier = 1f;
        public float FireRateUpgradeMultiplier = 1f;
        public int ProjectileCount = 1;
        public float AreaMultiplier = 1f;
        public float RangeMultiplier = 1f;
        public float NextFireTime;
    }

    private readonly Dictionary<WeaponType, WeaponState> weaponStates = new();
    private float permanentFireRateMultiplier = 1f;

    private void Awake()
    {
        baseAttackPower = PermanentUpgrades.ApplyAttack(baseAttackPower);
        permanentFireRateMultiplier = PermanentUpgrades.ApplyFireRate();

        foreach (WeaponType weaponType in System.Enum.GetValues(typeof(WeaponType)))
        {
            weaponStates[weaponType] = new WeaponState();
        }

        weaponStates[WeaponType.Basic].Unlocked = true;
        weaponStates[WeaponType.Basic].ProjectileCount = PermanentUpgrades.ProjectileCount;

        if (bulletPool == null)
        {
            bulletPool = FindFirstObjectByType<SimplePool>();
        }

        Transform playerFirePoint = transform.Find("FirePoint");
        if (playerFirePoint != null)
        {
            firePoint = playerFirePoint;
        }

        if (bulletPool == null)
        {
            Debug.LogError("PlayerShooter could not find a SimplePool in the scene.", this);
        }

        if (firePoint == null)
        {
            Debug.LogError("PlayerShooter could not find the Player/FirePoint child.", this);
        }
    }

    private void Update()
    {
        foreach (KeyValuePair<WeaponType, WeaponState> pair in weaponStates)
        {
            if (pair.Value.Unlocked && Time.time >= pair.Value.NextFireTime)
            {
                FireWeapon(pair.Key, pair.Value);
            }
        }
    }

    public bool IsWeaponUnlocked(WeaponType weaponType)
    {
        return weaponStates.TryGetValue(weaponType, out WeaponState state) && state.Unlocked;
    }

    public void ApplyUpgrade(WeaponUpgradeOption option)
    {
        WeaponState state = weaponStates[option.Weapon];

        if (!state.Unlocked)
        {
            state.Unlocked = true;
            state.NextFireTime = Time.time;
            return;
        }

        switch (option.Stat)
        {
            case WeaponStat.Damage:
                state.DamageUpgradeMultiplier += 0.5f;
                break;
            case WeaponStat.FireRate:
                state.FireRateUpgradeMultiplier += 0.5f;
                break;
            case WeaponStat.ProjectileCount:
                state.ProjectileCount++;
                break;
            case WeaponStat.Area:
                state.AreaMultiplier += option.Weapon == WeaponType.Freeze ? 0.25f : 0.5f;
                break;
            case WeaponStat.Range:
                state.RangeMultiplier += 0.25f;
                break;
        }
    }

    private void FireWeapon(WeaponType weaponType, WeaponState state)
    {
        if (bulletPool == null || firePoint == null)
        {
            return;
        }

        int projectileCount = weaponType == WeaponType.Laser || weaponType == WeaponType.Explosive
            ? 1
            : state.ProjectileCount;

        for (int i = 0; i < projectileCount; i++)
        {
            float centeredIndex = i - (projectileCount - 1) * 0.5f;
            Vector3 spawnPosition = firePoint.position + Vector3.up * centeredIndex * projectileSpacing;
            Vector2 shotDirection = firePoint.right;
            Quaternion shotRotation = Quaternion.FromToRotation(Vector3.right, shotDirection);

            GameObject bulletObject = bulletPool.GetFromPool();
            bulletObject.transform.SetPositionAndRotation(spawnPosition, shotRotation);

            Bullet bullet = bulletObject.GetComponent<Bullet>();
            if (bullet != null)
            {
                bullet.Launch(
                    bulletPool,
                    shotDirection,
                    bulletSpeed * GetProjectileSpeedMultiplier(weaponType),
                    weaponType,
                    baseAttackPower * GetDamageMultiplier(weaponType) * state.DamageUpgradeMultiplier,
                    weaponType == WeaponType.Explosive
                        ? baseAttackPower * 3f * state.DamageUpgradeMultiplier
                        : 0f,
                    state.AreaMultiplier,
                    state.RangeMultiplier,
                    GetProjectileSizeMultiplier(weaponType));
            }
        }

        float totalFireRateMultiplier = GetFireRateMultiplier(weaponType)
            * state.FireRateUpgradeMultiplier
            * permanentFireRateMultiplier;
        state.NextFireTime = Time.time + fireInterval / totalFireRateMultiplier;
    }

    private float GetDamageMultiplier(WeaponType weaponType)
    {
        return weaponType switch
        {
            WeaponType.Laser => 1.5f,
            WeaponType.Homing => 0.8f,
            WeaponType.Explosive => 2f,
            _ => 1f
        };
    }

    private float GetFireRateMultiplier(WeaponType weaponType)
    {
        return weaponType switch
        {
            WeaponType.Laser => 0.7f,
            WeaponType.Freeze => 0.5f,
            WeaponType.Knockback => 0.5f,
            WeaponType.Explosive => 0.3f,
            _ => 1f
        };
    }

    private float GetProjectileSpeedMultiplier(WeaponType weaponType)
    {
        return weaponType switch
        {
            WeaponType.Laser => 0.7f,
            WeaponType.Freeze => 0.8f,
            WeaponType.Knockback => 0.8f,
            WeaponType.Explosive => 0.5f,
            _ => 1f
        };
    }

    private float GetProjectileSizeMultiplier(WeaponType weaponType)
    {
        return weaponType switch
        {
            WeaponType.Homing => 0.8f,
            WeaponType.Explosive => 2f,
            _ => 1f
        };
    }
}
