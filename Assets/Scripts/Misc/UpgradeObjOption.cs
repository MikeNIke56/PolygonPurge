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

    public void SetData(UpgradeOptionData data)
    {
        this.data = data;
        nameText.text = data.optionName;
        descriptionText.text = data.description;
    }

    public void OptionSelected()
    {
        //grants ability/ability buff
        if (data.abilityObj)
            UpgradesManager.i.PlayerSpawnAbility(data.abilityObj);
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

                //load in and equip our weapon
                foreach(Transform weapon in PlayerController.i.
                    weaponPivotPoint.transform)
                    Destroy(weapon.gameObject);

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
        {
            UpgradesManager.i.ApplyStatBuff(data.statsBuff);
        }

        LevelSystem.i.levelsGained--;
        UpgradesManager.i.DisableUpgradesMenuMultipleLevelsGained();      
    }
}
