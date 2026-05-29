using UnityEngine;

public class Sniper : WeaponBaseClass
{
    [SerializeField] private float newBulletSpeed;

    public override void SetUp()
    {
        base.SetUp();
        newBulletSpeed = projectileSpeed;
    }

    public override void UpgradeWeapon(int level)
    {
        base.UpgradeWeapon(level);
        projectilePenetration += level;
        newBulletSpeed += level;
    }
}
