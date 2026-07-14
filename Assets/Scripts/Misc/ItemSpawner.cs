using UnityEngine;
using UnityEngine.InputSystem;

/**
 * manager class to handle spawning items
 */
public class ItemSpawner : MonoBehaviour
{
    [Header("Item Spawn Variables")]
    public GameObject[] possibleItems;
    public float minSpawnTime;
    public float maxSpawnTime;
    public float selectedSpawnTime;
    public int maxAllowedItems;
    private int curActiveItems = 0;

    public static ItemSpawner i { get; private set; }

    private void Awake()
    {
        if (i != null)
            Destroy(gameObject);
        else
            i = this;
    }

    private void Start()
    {
        selectedSpawnTime = SelectRandomSpawnTime();
    }

    private void Update()
    {
        selectedSpawnTime -= Time.deltaTime;

        if(selectedSpawnTime <= 0.0f && curActiveItems < maxAllowedItems)
        {
            SpawnItem();
            selectedSpawnTime = SelectRandomSpawnTime();
        }
    }

    void SpawnItem()
    {
        //spawn in randomItem
        GameObject itemGameObjectCopy = ObjectPoolingManager.SpawnObject(
            possibleItems[Random.Range(0, possibleItems.Length)], 
            FindRandomSpawnPoint(), Quaternion.identity, 
            ObjectPoolingManager.PoolType.Item);

        curActiveItems++;
    }

    private float SelectRandomSpawnTime()
    {
        return Random.Range(minSpawnTime, maxSpawnTime);
    }


    /**
     * finds random point within spawn area to spawn the item
     */
    private Vector3 FindRandomSpawnPoint()
    {
        //grab a random point within the box spawn area
        Bounds bounds = ArenaArea.i.GetSpawnArea().bounds;

        Vector2 randSpawnPoint = new Vector2(
            Random.Range(bounds.min.x, bounds.max.x),
            Random.Range(bounds.min.y, bounds.max.y)
        );
        return randSpawnPoint;
    }

    public void DerementItemNum()
    {
        curActiveItems--;
    }
}
