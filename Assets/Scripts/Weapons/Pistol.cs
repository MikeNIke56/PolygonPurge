using UnityEngine;

public class Pistol : WeaponBaseClass
{
    [SerializeField] private float sizeIncreaseAmnt;

    public override void UpgradeWeapon(int level)
    {
        base.UpgradeWeapon(level);
        projectilePenetration += level;
        projectileSize *= sizeIncreaseAmnt;
    }
}
