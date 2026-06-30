using UnityEngine;

/**
* base class for all abilities
*/
public class AbilityBaseClass : MonoBehaviour
{
    //keeps track of the ability's current level
    protected int currentAbilityLevel = 1;

    //the connected ability to add to the currentAbilities list (Player only)
    [SerializeField] private GameObject connectedBossAbility;

    protected BossEnemy boss;

    protected virtual void Update()
    {

    }

    protected virtual void FixedUpdate()
    {

    }

    public virtual void SetUp()
    {
        if(name.Contains("Boss"))
            boss = FindAnyObjectByType<BossEnemy>();
    }

    /**
     * upgrades ability based on its current level
     */
    public virtual void UpgradeAbility(int level)
    {
        currentAbilityLevel++;
        Debug.Log(name + " upgraded to " + currentAbilityLevel);
    }

    public int GetCurrentLevel()
    {
        return currentAbilityLevel;
    }

    public GameObject GetConnectedBossAbility()
    {
        return connectedBossAbility;
    }
}
