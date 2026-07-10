using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

/**
 * manages all the health bars within the arena (player and enemies)
 */
public class HealthBarsManager : MonoBehaviour
{
    public GameObject healthBarObj;
    public List<HealthBarObj> healthBars;

    public static HealthBarsManager i { get; private set; }

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
    }

    private void LateUpdate()
    {
        foreach(var healthBar in healthBars)
        {
            if (!healthBar) continue;
            if (healthBar.gameObject.activeSelf == false) continue;
            if (!healthBar.target) continue;
            if (healthBar.target.gameObject.activeSelf == false) continue;

            Vector3 screenPos = Camera.main.WorldToScreenPoint(
                healthBar.target.transform.position + 
                healthBar.target.healthBarOffset);

            healthBar.transform.position = screenPos;
            healthBar.HandleUIVisibility();
        }
    }

    /**
     * creates a health bar and returns it for use
     */
    public HealthBarObj CreateHealthBar(Vector3 spawnOffset, EntityBaseClass target)
    {
        //loads in health bar object
        GameObject healthBarObjCopy = ObjectPoolingManager.SpawnObject(healthBarObj,
            target.gameObject.transform.position + spawnOffset, 
            Quaternion.identity, ObjectPoolingManager.PoolType.HealthBar);

        healthBarObjCopy.GetComponent<HealthBarObj>().target = target;
        healthBars.Add(healthBarObjCopy.GetComponent<HealthBarObj>());

        return healthBarObjCopy.GetComponent<HealthBarObj>();
    }
}
