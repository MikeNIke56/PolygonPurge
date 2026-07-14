using System.Linq;
using TMPro;
using UnityEngine;
using static GameManager;

/**
 * the option the player can choose upgrade themselves
 */
public class UpgradeObjOption : MonoBehaviour
{
    public UpgradeOptionData data;
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI descriptionText;
    public TextMeshProUGUI levelText;

    public void SetData(UpgradeOptionData data)
    {
        this.data = data;
        nameText.text = data.optionName;
        descriptionText.text = data.description;

        int lvl = 0;

        if (data.abilityObj)
            lvl = data.level + 1;
        else if (data.weaponObj)
        {
            if (data.level >= UpgradesManager.i.GetMaxLevel())
                lvl = PlayerController.i.GetPrimaryWeapon().
                currrentWeaponLevel;
            else
                lvl = PlayerController.i.GetPrimaryWeapon().
                 currrentWeaponLevel + 1;
        }
        else
            lvl = data.level + 1;

        levelText.text = "Lvl " + lvl.ToString();
    }

    public void OptionSelected()
    {
        //grants ability/ability buff
        if (data.abilityObj)
        {
            if (UpgradesManager.i.currentAbilities.Count == 0)
                UpgradesManager.i.PlayerSpawnAbility(data.abilityObj);
            else
            {
                //if we already have the ability, just upgrade it
                foreach (AbilityBaseClass ability in UpgradesManager.i.
                        currentAbilities)
                {
                    if (ability.GetConnectedBossAbility() == data.abilityObj.
                            GetComponent<AbilityBaseClass>().GetConnectedBossAbility())
                    {
                        UpgradesManager.i.UpgradeAbility(ability);
                        break;
                    }
                    //if not, create it
                    UpgradesManager.i.PlayerSpawnAbility(data.abilityObj);
                    break;
                }
            }           
        }         
        //grants weapon/weapon buff
        else if(data.weaponObj)
        {
            //if the weapon is the same as our current one, just upgrade it
            if(data.weaponObj.GetComponent<WeaponBaseClass>().weaponName ==
                PlayerController.i.GetPrimaryWeapon().weaponName)
            {
                UpgradesManager.i.ApplyWeaponBuff(PlayerController.i.
                    GetPrimaryWeapon());
            }
            //if not, swap weapons then upgrade it
            else
            {
                WeaponBaseClass prevWeapon = PlayerController.i.
                    GetPrimaryWeapon();

                //destroy current weapon
                foreach(Transform weapon in PlayerController.i.
                    weaponPivotPoint.transform)
                    Destroy(weapon.gameObject);

                //load in and equip our weapon
                GameObject primaryWeaponGameObjectCopy =
                    Instantiate<GameObject>(data.weaponObj,
                    PlayerController.i.weaponPivotPoint.transform);

                primaryWeaponGameObjectCopy.
                    GetComponent<WeaponBaseClass>().SetWeaponPivotOffset();

                PlayerController.i.SetPrimaryWeapon(primaryWeaponGameObjectCopy.
                    GetComponent<WeaponBaseClass>());

                //upgrade it
                UpgradesManager.i.ApplyWeaponBuff(true, 
                    prevWeapon.currrentWeaponLevel);
            }  
        }
        //grants stat buff
        else
            UpgradesManager.i.ApplyStatBuff(data.statsBuff);

        if (data.weaponObj)
            data.level = PlayerController.i.GetPrimaryWeapon().
                currrentWeaponLevel;
        else
            data.level++;

        LevelSystem.i.levelsGained--;
        UpgradesManager.i.DisableUpgradesMenuMultipleLevelsGained();      
    }
}
