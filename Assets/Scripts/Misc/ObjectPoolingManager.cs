using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

/**
 * Object pooler for our bullets and enemies
 */
public class ObjectPoolingManager : MonoBehaviour
{
    //used to keep things from destroying on scene loads
    [SerializeField] private bool addToDontDestroyOnLoad = false;

    //holds the lists of pools
    private static GameObject emptyHolder;

    //the game objects of the types of pools we're handling once spawned
    private static GameObject bulletsEmpty;
    private static GameObject enemiesEmpty;
    private static GameObject abilitiesEmpty;

    //list of pools for each type of object we're handling
    private static Dictionary<GameObject, ObjectPool<GameObject>> objectPools;

    //list of game objects instanced 
    private static Dictionary<GameObject, GameObject> cloneToPrefabMap;

    //types of object we're handling
    public enum PoolType
    {
        Bullet,
        Enemy,
        Ability
    }
    public static PoolType poolType;

    public static ObjectPoolingManager i { get; private set; }

    private void Awake()
    {
        if (i != null)
        {
            Destroy(gameObject);
        }
        else
        {
            i = this;
            DontDestroyOnLoad(gameObject);
        }

        objectPools = new Dictionary<GameObject, ObjectPool<GameObject>>();
        cloneToPrefabMap = new Dictionary<GameObject, GameObject>();

        SetupEmpties();
    }

    /**
     * sets up new game objects in the scene to hold out lists of pools
     */
    private void SetupEmpties()
    {
        //create the empty parent holder
        emptyHolder = new GameObject("Object Pools");

        //parent the child holders
        bulletsEmpty = new GameObject("Bullets");
        bulletsEmpty.transform.SetParent(emptyHolder.transform);

        enemiesEmpty = new GameObject("Enemies");
        enemiesEmpty.transform.SetParent(emptyHolder.transform);

        abilitiesEmpty = new GameObject("Abilities");
        abilitiesEmpty.transform.SetParent(emptyHolder.transform);

        if (addToDontDestroyOnLoad)
            DontDestroyOnLoad(bulletsEmpty.transform.root);
    }

    /**
     * creates pool of the type of game object being passed
     */
    private static void CreatePool(GameObject prefab, Vector3 pos, Quaternion rot, 
        PoolType poolType = PoolType.Bullet)
    {
        ObjectPool<GameObject> pool = new ObjectPool<GameObject>(
            createFunc: () => CreateObject(prefab, pos, rot, poolType),
            actionOnGet: OnGetObject,
            actionOnRelease: OnReleaseObject,
            actionOnDestroy: OnDestroyObject);

        objectPools.Add(prefab, pool);
    }

    /**
     * spawns in the passed in game object and parents it to the appropriate 
     * pool holder in scene
     */
    private static GameObject CreateObject(GameObject prefab, Vector3 pos, Quaternion rot,
        PoolType poolType = PoolType.Bullet)
    {
        //we deactive the object first so its OnEnable is called when we reactivate it after spawning
        prefab.SetActive(false);

        GameObject obj = Instantiate(prefab, pos, rot);

        prefab.SetActive(true);

        GameObject parentObj = SetParentObject(poolType);
        obj.transform.SetParent(parentObj.transform);

        return obj;
    }

    /**
     * whatever logic we want to call when getting an object
     */
    private static void OnGetObject(GameObject obj)
    {
        //optional logic to call when getting object
    }

    /**
     * when the object is put back into its pool,
     * we'll deactive it in game
     */
    private static void OnReleaseObject(GameObject obj)
    {
        obj.SetActive(false);
    }

    /**
     * whenever we destroy the object, remove it from the list
     */
    private static void OnDestroyObject(GameObject obj)
    {
        if (cloneToPrefabMap.ContainsKey(obj))
        {
            cloneToPrefabMap.Remove(obj);
        }
    }

    /**
     * determines which game object to use as the parent
     * when spawning objects
     */
    private static GameObject SetParentObject(PoolType poolType)
    {
       switch (poolType)
       {
            case PoolType.Bullet:
                return bulletsEmpty;

            case PoolType.Enemy:
                return enemiesEmpty;

            case PoolType.Ability:
                return abilitiesEmpty;

            default:
                return null;
       }
    }

    /**
     * spawns in passed in object
     */
    private static T SpawnObject<T>(GameObject objectToSpawn, Vector3 pos, Quaternion rot,
        PoolType poolType = PoolType.Bullet) where T : UnityEngine.Object
    {
        //if a pool that should hold the passed in object doesnt exist, create it
        if(!objectPools.ContainsKey(objectToSpawn))
        {
            CreatePool(objectToSpawn, pos, rot, poolType);
        }

        /*this will try to retrieve the obj from the pool if it exists
        *if it does, it'll get it and reactivate it
        *if it doesnt, it'll instantiate a new one
        */
        GameObject obj = objectPools[objectToSpawn].Get();

        if(obj != null)
        {
            //if the object being passed doesnt exist in the dictionary, add it
            if (!cloneToPrefabMap.ContainsKey(obj))
            {
                cloneToPrefabMap.Add(obj, objectToSpawn);
            }

            //setting its spawn parameters
            obj.transform.position = pos;
            obj.transform.rotation = rot;
            obj.SetActive(true);

            //return object types are matching
            if(typeof(T) == typeof(GameObject))
            {
                return obj as T;
            }

            /*
             * if types dont match, get the T component in the game object and return that
             * if it exists
             */
            T component = obj.GetComponent<T>();
            if(component == null)
            {
                Debug.Log(objectToSpawn.name + " doesnt have a component of type " + typeof(T));
                return null;
            }

            return component;
        }

        return null;
    }

    /**
     * spawns in passed in object
     */
    public static T SpawnObject<T>(T typePrefab, Vector3 pos, Quaternion rot,
       PoolType poolType = PoolType.Bullet) where T : UnityEngine.Component
    {
        return SpawnObject<T>(typePrefab, pos, rot, poolType);
    }

    /**
     * spawns in passed in object
     */
    public static GameObject SpawnObject(GameObject objectToSpawn, Vector3 pos, Quaternion rot,
       PoolType poolType = PoolType.Bullet)
    {
        return SpawnObject<GameObject>(objectToSpawn, pos, rot, poolType);
    }

    /**
     * when an object is no longer needed, return it to its respective pool
     */
    public static void ReturnObjectToPool(GameObject obj, PoolType poolType = PoolType.Bullet)
    {
        if(cloneToPrefabMap.TryGetValue(obj, out GameObject prefab))
        {
            GameObject parentObject = SetParentObject(poolType);

            if(obj.transform.parent != parentObject.transform)
            {
                obj.transform.SetParent(parentObject.transform);
            }

            if(objectPools.TryGetValue(prefab, out ObjectPool<GameObject> pool))
            {
                pool.Release(obj);
            }
        }
        else
            Debug.Log("object not pooled " +  obj.name);
    }
}
