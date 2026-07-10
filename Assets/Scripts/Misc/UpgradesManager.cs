using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using UnityEngine.UIElements;
using static GameManager;

/**
 * manages all upgrades related to the player (stats and abilities)
 */
public class UpgradesManager : MonoBehaviour
{
    public enum StatsUpgrades
    {
        None,
        Attack,
        MaxHealth,
        HealthRegen,
        MoveSpeed,
        AttackSpeed
    }

    [Header("UpgradesManager Variables")]
    //keeps track of the player's current stats buffs
    public Dictionary<StatsUpgrades, int> statsBuffRecord;

    //keeps track of the player's current abilities
    public List<AbilityBaseClass> currentAbilities;

    public float[] statIncreases = new float[5];
    private const int maxLevel = 5;

    public GameObject background;
    public GameObject upgradesMenu;
    public GameObject upgradesContainer;
    public GameObject upgradeOptionObj;
    public int maxUpgrageOptions;

    public List<UpgradeOptionData> availableUpgrades;
    public List<UpgradeOptionData> allUpgrades;

    public static UpgradesManager i { get; private set; }

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

    private void Start()
    {
        statsBuffRecord = new Dictionary<StatsUpgrades, int>();
        currentAbilities = new List<AbilityBaseClass>();
        availableUpgrades = new List<UpgradeOptionData>();

        foreach (UpgradeOptionData data in allUpgrades)
            data.level = 1;

        foreach (UpgradeOptionData data in allUpgrades)
            availableUpgrades.Add(data);

        InitializeStartingStats();
    }

    /**
     * initializes all player stats to lvl1
     */
    private void InitializeStartingStats()
    {
        statsBuffRecord.Add(StatsUpgrades.Attack, 1);
        statsBuffRecord.Add(StatsUpgrades.MaxHealth, 1);
        statsBuffRecord.Add(StatsUpgrades.HealthRegen, 1);
        statsBuffRecord.Add(StatsUpgrades.MoveSpeed, 1);
        statsBuffRecord.Add(StatsUpgrades.AttackSpeed, 1);
    }

    /**
     * applies the appropriate buff to the player based on what upgrade they
     * chose
     */
    public void ApplyStatBuff(StatsUpgrades statType, bool carryOverLvls=false,
        int lvlsToAdd=1)
    {
        if (statsBuffRecord.TryGetValue(statType, out int value))
        {
            if (carryOverLvls == true)
            {
                switch (statType)
                {
                    //sets the previous weapon's buff lvl to the new one
                    case StatsUpgrades.Attack:
                        WeaponBaseClass weaponAtkCopy = PlayerController.i.
                            GetPrimaryWeapon();

                        for(int i = 0; i < lvlsToAdd; i++)
                        {
                            weaponAtkCopy.SetWeaponAttack(weaponAtkCopy.
                                GetWeaponAttack() * statIncreases[i]);
                        }
                        //Debug.Log("atk: " + value);
                        break;

                    //sets the previous weapon's buff lvl to the new one
                    case StatsUpgrades.AttackSpeed:
                        WeaponBaseClass weaponCDCopy = PlayerController.i.
                            GetPrimaryWeapon();

                        for (int i = 0; i < lvlsToAdd; i++)
                        {
                            weaponCDCopy.SetWeaponCooldown(weaponCDCopy.
                                GetWeaponFireCooldown() / statIncreases[i]);
                        }
                        //Debug.Log("atk spd: " + value);
                        break;
                    default:
                        break;
                }
            }
            else
            {
                if (value < maxLevel)
                {
                    switch (statType)
                    {
                        //applies the buffs to the player's current weapon
                        case StatsUpgrades.Attack:
                            float curWeaponAttack = PlayerController.i.
                                GetPrimaryWeapon().GetWeaponAttack();

                            WeaponBaseClass weaponAtkCopy = PlayerController.i.
                                GetPrimaryWeapon();

                            weaponAtkCopy.SetWeaponAttack(curWeaponAttack *
                                statIncreases[value]);
                            Debug.Log("atk: " + value);
                            break;

                        //applies the buffs to the player's current weapon
                        case StatsUpgrades.AttackSpeed:
                            float curWeaponCooldown = PlayerController.i.
                                GetPrimaryWeapon().fireCooldown;

                            WeaponBaseClass weaponCDCopy = PlayerController.i.
                                GetPrimaryWeapon();

                            weaponCDCopy.SetWeaponCooldown(weaponCDCopy.GetWeaponFireCooldown() /
                                statIncreases[value]);
                            //Debug.Log("atk spd: " + value);
                            break;

                        case StatsUpgrades.MoveSpeed:
                            float curPlayerSpeed = PlayerController.i.GetMoveSpeed();
                            PlayerController.i.SetMoveSpeed(curPlayerSpeed *
                                statIncreases[value]);
                            //Debug.Log("move: " + value);
                            break;

                        case StatsUpgrades.HealthRegen:
                            float curPlayerHealthRegen =
                                PlayerController.i.GetHealthRegenSpeed();

                            float curPlayerMaxHealthRegenTime =
                                PlayerController.i.GetMaxHealthRegenTime();

                            PlayerController.i.SetHealthRegenSpeed(
                                curPlayerHealthRegen * statIncreases[value]);
                            PlayerController.i.SetMaxHealthRegenTime(
                                curPlayerMaxHealthRegenTime / statIncreases[value]);
                            //Debug.Log("regen spd: " + value);
                            break;

                        case StatsUpgrades.MaxHealth:
                            float curPlayerMaxHealth = PlayerController.i.GetMaxHealth();
                            PlayerController.i.SetMaxHealth(curPlayerMaxHealth *
                                statIncreases[value]);
                            //Debug.Log("max hlth: " + value);
                            break;

                        default:
                            Debug.Log("no existing stat");
                            break;
                    }

                    statsBuffRecord[statType]++;
                }
                else
                    Debug.Log(statType.ToString() + " is maxxed out");
            }      
        }
    }

    /**
     * applies the appropriate buff to the player's currrent weapon
     */
    public void ApplyWeaponBuff(bool carryOverLvls = false, int lvlsToAdd=1)
    {
        WeaponBaseClass weaponCopy = PlayerController.i.GetPrimaryWeapon();

        if (carryOverLvls == true)
        {
            for (int i = 0; i < lvlsToAdd; i++)
            {
                if (weaponCopy.currrentWeaponLevel < maxLevel-1)
                    weaponCopy.UpgradeWeapon(1);
                else if (weaponCopy.currrentWeaponLevel == maxLevel-1)
                    weaponCopy.UpgradeWeapon(2);
            }
        }
        else
        {
            if (weaponCopy.currrentWeaponLevel < maxLevel-1)
                weaponCopy.UpgradeWeapon(lvlsToAdd);
            else if(weaponCopy.currrentWeaponLevel == maxLevel - 1)
                weaponCopy.UpgradeWeapon(lvlsToAdd + 1);
        }
    }

    /**
     * spawns in ability for player
     */
    public void PlayerSpawnAbility(GameObject ability)
    {
        /*
         * spawns in ability object
         * have to spawn and parent seperately because we are attaching
         * to a persistent (Dont Destroy On Load) object
         */
        GameObject abilityObjCopy = Instantiate(ability);

        Transform playerParentTrans;
        if (ability.GetComponent<AbilityBaseClass>() is LittleBuddy == false)
        {
            playerParentTrans = PlayerController.i.
                abilityPivotPoint.transform;
            abilityObjCopy.transform.SetParent(playerParentTrans);
            abilityObjCopy.transform.localPosition = Vector3.zero;
        }
        else
        {
            abilityObjCopy.transform.SetParent(PlayerController.i.littleBuddyPivotPoint.transform);
            abilityObjCopy.transform.localPosition = Vector3.zero;
        }

        abilityObjCopy.GetComponent<AbilityBaseClass>().SetUp();
        currentAbilities.Add(abilityObjCopy.GetComponent<AbilityBaseClass>());

        if (ability.GetComponent<AbilityBaseClass>() is SpectreRounds)
            PlayerController.i.SetSpectreRounds(
                abilityObjCopy.GetComponent<SpectreRounds>());
    }

    /**
     * spawns in ability for boss
     */
    public void SpawnBossAbilities(Transform parent, BossEnemy boss)
    {
        /*
         * spawns in all abilities from the currentAbilities list
         * have to spawn and parent seperately because we are attaching
         * to a persistent (Dont Destroy On Load) object
         */

        foreach(AbilityBaseClass ability in currentAbilities)
        {
            if (boss.currentAbilities.Contains(ability.
                GetConnectedBossAbility().GetComponent<AbilityBaseClass>())) return;

            GameObject abilityObjCopy = Instantiate(ability.
                GetConnectedBossAbility());

            abilityObjCopy.GetComponent<AbilityBaseClass>().SetUp();

            boss.currentAbilities.Add(ability.GetConnectedBossAbility().
                GetComponent<AbilityBaseClass>());

            if (ability.GetConnectedBossAbility().
                GetComponent<AbilityBaseClass>() is BossLittleBuddy == false)
            {
                abilityObjCopy.transform.SetParent(parent);
                abilityObjCopy.transform.localPosition = Vector3.zero;
            }
            else
            {
                abilityObjCopy.transform.SetParent(boss.littleBuddyPivotPoint.transform);
                abilityObjCopy.transform.localPosition = Vector3.zero;
            }

            if (ability.GetConnectedBossAbility().
                GetComponent<AbilityBaseClass>() is BossSpectreRounds)
                boss.SetSpectreRounds(abilityObjCopy.
                    GetComponent<BossSpectreRounds>());
        }
    }


    /**
     * upgrades selected ability
     */
    public void UpgradeAbility(AbilityBaseClass ability)
        {
            if (ability.GetCurrentLevel() < maxLevel)
                ability.UpgradeAbility(1);
        }

    /**
     * brings up the upgrades menu and pauses the active game
     */
    public void InitializeUpgradesMenu()
    {
        background.SetActive(true);
        upgradesMenu.SetActive(true);
        Time.timeScale = 0f;

        //first clear all previous upgrades if there are any
        foreach (Transform child in upgradesContainer.transform)
            Destroy(child.gameObject);

        availableUpgrades.Clear();
        foreach (UpgradeOptionData data in allUpgrades)
        {
            if (data.level < maxLevel)
                availableUpgrades.Add(data);
        }

        //repopulate children
        int optionsToSpawn;
        if (availableUpgrades.Count < maxUpgrageOptions)
            optionsToSpawn = availableUpgrades.Count;
        else
            optionsToSpawn = maxUpgrageOptions;

        for (int i = 0; i < optionsToSpawn; i++)
        {
            GameObject upgradeOptionCopy = Instantiate(upgradeOptionObj, 
                upgradesContainer.transform);

            //set the option's data a randomly picked one
            UpgradeOptionData randomUpgradeOption = GetRandomUpgradeData();
            
            upgradeOptionCopy.GetComponent<UpgradeObjOption>().SetData(
                randomUpgradeOption);

            availableUpgrades.Remove(randomUpgradeOption);
        }

        HorizontalLayoutGroup horizontalGroup = upgradesContainer.
            GetComponent<HorizontalLayoutGroup>();

        switch (optionsToSpawn)
        {
            case 1:
                horizontalGroup.padding.left = -35;
                return;
            case 2:
                horizontalGroup.padding.left = -250;
                return;
            case 3:
                horizontalGroup.padding.left = -400;
                return;
            case 4:
                horizontalGroup.padding.left = -550;
                return;
            default:
                Debug.Log("no options to load");
                break;
        }
    }

    /**
     * randomly selects upgrade from list
     */
    private UpgradeOptionData GetRandomUpgradeData()
    {
        return availableUpgrades[Random.Range(0, availableUpgrades.Count)];
    }

    public void DisableUpgradesMenuMultipleLevelsGained()
    {
        background.SetActive(false);
        upgradesMenu.SetActive(false);
    }

    public void DisableUpgradesMenuSingleLevelGained()
    {
        background.SetActive(false);
        upgradesMenu.SetActive(false);
        Time.timeScale = 1f;
    }

    public int GetMaxLevel()
    {
        return maxLevel;
    }
}
