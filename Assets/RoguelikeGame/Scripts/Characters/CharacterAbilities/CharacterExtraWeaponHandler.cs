using JadePhoenix.Tools;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterExtraWeaponHandler : CharacterWeaponHandler
{
    // This will be something like 'SecondaryShoot' or 'Melee'.
    public string ButtonID = "";

    protected JP_Input.Button assignedButton;

    protected override void Initialization()
    {
        base.Initialization();
        assignedButton = InputManager.Instance.GetButtonFromID(ButtonID);
    }

    protected override void HandleInput()
    {
        if (_condition.CurrentState != CharacterStates.CharacterConditions.Normal)
        {
            return;
        }

        if (assignedButton.State.CurrentState == JP_Input.ButtonStates.ButtonDown)
        {
            //Debug.Log($"{this.GetType()}.HandleInput: Got button down, ShootStart called.", gameObject);
            ShootStart();
        }

        if (assignedButton.State.CurrentState == JP_Input.ButtonStates.ButtonUp)
        {
            //Debug.Log($"{this.GetType()}.HandleInput: Got button up, ShootStop called.", gameObject);
            ShootStop();
        }

        if (CurrentWeapon != null)
        {
            if (CurrentWeapon.WeaponState.CurrentState == Weapon.WeaponStates.WeaponDelayBetweenUses
            && assignedButton.State.CurrentState == JP_Input.ButtonStates.Off)
            {
                CurrentWeapon.WeaponInputStop();
            }
        }
    }
}
