using UnityEngine;

/**
* item that will heal the player by a certain amount if 
* they are not at full hp
*/
public class HealthPickup : ItemBaseClass
{
    public float healAmnt;

    // Update is called once per frame
    private void Update()
    {
        BobUpDown();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        //if the collided object is on a layer we should interact with...
        if (IsInLayerMask(collision.gameObject, targetLayers))
        {
            if(PlayerController.i.GetCurHealth() <
                PlayerController.i.GetMaxHealth())
            {
                PlayerController player = PlayerController.i;

                player.SetCurHealth(Mathf.Clamp(player.GetCurHealth() + healAmnt, 
                    1, player.GetMaxHealth()));


                ItemSpawner.i.DerementItemNum();

                ObjectPoolingManager.ReturnObjectToPool(gameObject,
                    ObjectPoolingManager.PoolType.Item);
            }
        }
    }
}
