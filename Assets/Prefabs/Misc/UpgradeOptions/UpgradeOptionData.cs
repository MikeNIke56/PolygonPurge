using UnityEngine;
using static UpgradesManager;

/**
 * the scriptable object that holds all relevant data for each upgrade option
 */
[CreateAssetMenu(fileName = "UpgradeOptionData", 
    menuName = "Scriptable Objects/NewUpgradeOptionData")]
public class UpgradeOptionData : ScriptableObject
{
    public GameObject abilityObj;
    public GameObject weaponObj;
    public StatsUpgrades statsBuff;
    public string optionName;
    public string description;
    public int level;
}
