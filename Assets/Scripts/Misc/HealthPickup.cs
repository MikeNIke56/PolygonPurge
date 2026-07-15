using UnityEngine;

/**
* item that will heal the player by a certain amount if 
* they are not at full hp
*/
public class HealthPickup : ItemBaseClass
{
    public float healAmnt;
    public GameObject healVFXObj;

    // Update is called once per frame
    private void Update()
    {
        BobUpDown();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        //if the collided object is on a layer we should interact with...
        if (LayerMaskChecker.i.IsInLayerMask(collision.gameObject, targetLayers))
        {
            if(PlayerController.i.GetCurHealth() <
                PlayerController.i.GetMaxHealth())
            {
                PlayerController player = PlayerController.i;

                player.SetCurHealth(Mathf.Clamp(player.GetCurHealth() + healAmnt, 
                    1, player.GetMaxHealth()));
                player.GetHealthBar().UpdateHealth(player.GetCurHealth());

                ItemSpawner.i.DerementItemNum();

                //loads in vfx
                GameObject healthVFXCopy = ObjectPoolingManager.SpawnObject(
                    healVFXObj, PlayerController.i.transform.position,
                    Quaternion.identity, ObjectPoolingManager.PoolType.VFX);

                ObjectPoolingManager.ReturnObjectToPool(gameObject,
                    ObjectPoolingManager.PoolType.Item);
            }
        }
    }
}
