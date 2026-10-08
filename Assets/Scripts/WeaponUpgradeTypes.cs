public enum WeaponType
{
    Basic,
    Laser,
    Freeze,
    Knockback,
    Homing,
    Explosive
}

public enum WeaponStat
{
    Damage,
    FireRate,
    ProjectileCount,
    Area,
    Range
}

public struct WeaponUpgradeOption
{
    public WeaponType Weapon;
    public WeaponStat Stat;
    public bool UnlocksWeapon;

    public WeaponUpgradeOption(WeaponType weapon, WeaponStat stat, bool unlocksWeapon)
    {
        Weapon = weapon;
        Stat = stat;
        UnlocksWeapon = unlocksWeapon;
    }
}
