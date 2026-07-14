using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

/**
* item that will damage all enemies within range 
*/
public class Bomb : ItemBaseClass
{
    public float damage;
    public float range;
    public CircleCollider2D damageCollider;
    public List<Collider2D> colliders;

    private void Start()
    {
        damageCollider.radius = range;
    }

    // Update is called once per frame
    private void Update()
    {
        BobUpDown();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        //if the collided object is the player
        if (LayerMaskChecker.i.IsInLayerMask(collision.gameObject, targetLayers))
        {
            //apply damage to all enemies within the bomb's collider
            foreach (Collider2D col in colliders)
            {
                EnemyBaseClass enemy = col.gameObject.
                    GetComponent<EnemyBaseClass>();

                enemy.TakeDamage(damage);
            }

            ItemSpawner.i.DerementItemNum();

            ObjectPoolingManager.ReturnObjectToPool(gameObject,
                ObjectPoolingManager.PoolType.Item);
        }
        else
        {
            //add enemy to list
            if (!colliders.Contains(collision))
                colliders.Add(collision);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        //remove enemy from list when out of range
        if (colliders.Contains(collision))
            colliders.Remove(collision);
    }
}
