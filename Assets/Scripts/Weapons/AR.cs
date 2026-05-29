using UnityEngine;

public class AR : WeaponBaseClass
{
    public override void UpgradeWeapon(int level)
    {
        base.UpgradeWeapon(level);
        projectilePenetration += level;
    }
}
