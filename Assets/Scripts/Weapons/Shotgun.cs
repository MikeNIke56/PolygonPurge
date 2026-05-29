using UnityEngine;

public class Shotgun : WeaponBaseClass
{
    public int rounds;
    public float bulletSpreadDecreaseAmnt;

    public override void UpgradeWeapon(int level)
    {
        base.UpgradeWeapon(level);
        projectileSpread *= bulletSpreadDecreaseAmnt;
    }
}
