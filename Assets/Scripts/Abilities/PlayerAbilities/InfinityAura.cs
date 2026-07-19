using UnityEngine;

public class InfinityAura : AbilityBaseClass
{
    //the layers of objects this object is allowed to apply physics to
    public LayerMask targetLayers;

    public float slowSpeedPercentage;
    public float slowIncreaseAmnt;
    public float slowRange;

    public GameObject infinityObj;

    public override void SetUp()
    {
        base.SetUp();
        GetComponent<CircleCollider2D>().radius = slowRange;

        //spawns in infinity object
        GameObject infinityObjCopy = Instantiate(infinityObj, transform);

        infinityObjCopy.transform.localScale = 
            new Vector3(slowRange * 2, slowRange * 2);
    }

    public override void UpgradeAbility(int level)
    {
        base.UpgradeAbility(level);
        slowSpeedPercentage -= slowIncreaseAmnt;
        slowRange += slowIncreaseAmnt;

        GetComponent<CircleCollider2D>().radius = slowRange;

        GameObject child = transform.GetChild(0).gameObject;
        child.transform.localScale = new Vector3(slowRange * 2, slowRange * 2);
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        //if the collided object is on a layer we should interact with...
        if (LayerMaskChecker.i.IsInLayerMask(collider.gameObject, targetLayers))
        {
            GameObject entity = collider.gameObject;

            //and if it implements the IDamageable interface
            if (entity.TryGetComponent(out IDamageable myInterface))
            {
                EntityBaseClass enemy = entity.GetComponent<EntityBaseClass>();

                //slow enemy
                if (enemy as SquareEnemy == false && 
                    enemy as TriangleEnemy == false)
                    enemy.GetComponent<Rigidbody2D>().linearVelocity *= slowSpeedPercentage;
                else
                    enemy.SetMoveSpeed(enemy.GetMoveSpeed() * slowSpeedPercentage);
            }
            //enemy projectile
            else
            {
                ProjectileBaseClass enemyBullet =
                    entity.GetComponent<ProjectileBaseClass>();

                //slow projectile
                enemyBullet.GetComponent<Rigidbody2D>().
                    linearVelocity *= slowSpeedPercentage;
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collider)
    {
        //if the collided object is on a layer we should interact with...
        if (LayerMaskChecker.i.IsInLayerMask(collider.gameObject, targetLayers))
        {
            GameObject entity = collider.gameObject;

            //and if it implements the IDamageable interface
            if (entity.TryGetComponent(out IDamageable myInterface))
            {
                EntityBaseClass enemy = entity.GetComponent<EntityBaseClass>();

                //reset enemy speed
                if (enemy as SquareEnemy == false &&
                    enemy as TriangleEnemy == false)
                    enemy.GetComponent<Rigidbody2D>().linearVelocity /= slowSpeedPercentage;
                else
                    enemy.SetMoveSpeed(enemy.GetMoveSpeed() / slowSpeedPercentage);
            }
            //enemy projectile
            else
            {
                ProjectileBaseClass enemyBullet =
                    entity.GetComponent<ProjectileBaseClass>();

                //reset projectile speed
                enemyBullet.GetComponent<Rigidbody2D>().
                    linearVelocity /= slowSpeedPercentage;
            }
        }
    }
}
