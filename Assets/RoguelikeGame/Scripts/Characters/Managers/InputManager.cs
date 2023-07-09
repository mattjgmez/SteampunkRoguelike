using JadePhoenix.Tools;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : Singleton<InputManager>
{
    [Header("Status")]
    // Set this to false to prevent input from being detected.
    public bool InputDetectionActive = true;

    [Header("Player Binding")]
    // A string used to identify the target player. Must match the ID given to the player object exactly.
    public string PlayerID = "Player";
    // The possible kinds of control used for movement.
    public enum ControlProfiles { Gamepad, Keyboard }

    [Header("Movement Settings")]
    // If set to true, acceleration/deceleration will be active for player movement.
    public bool SmoothMovement = true;
    /// the minimum horizontal and vertical value you need to reach to trigger movement on a gamepad
    public Vector2 Threshold = new Vector2(0.1f, 0.4f);

    public JP_Input.Button ShootButton { get; protected set; }
    public JP_Input.Button SecondaryShootButton { get; protected set; }
    public JP_Input.Button InteractButton { get; protected set; }
    public JP_Input.Button MeleeButton { get; protected set; }
    public JP_Input.Button DodgeButton { get; protected set; }
    public JP_Input.Button PauseButton { get; protected set; }
    public Vector2 PrimaryMovement { get { return _primaryMovement; } }

    protected List<JP_Input.Button> ButtonList;
    protected Vector2 _primaryMovement = Vector2.zero;
    protected string _axisHorizontal;
    protected string _axisVertical;

    protected virtual void Start()
    {
        InitializeButtons();
        InitializeAxis();
    }

    protected virtual void InitializeButtons()
    {
        ButtonList = new List<JP_Input.Button>
        {
            (ShootButton = new JP_Input.Button(PlayerID, "Shoot", ShootButtonDown, ShootButtonPressed, ShootButtonUp)),
            (SecondaryShootButton = new JP_Input.Button(PlayerID, "SecondaryShoot", SecondaryShootButtonDown, SecondaryShootButtonPressed, SecondaryShootButtonUp)),
            (InteractButton = new JP_Input.Button(PlayerID, "Interact", InteractButtonDown, InteractButtonPressed, InteractButtonUp)),
            (MeleeButton = new JP_Input.Button(PlayerID, "Melee", MeleeButtonDown, MeleeButtonPressed, MeleeButtonUp)),
            (DodgeButton = new JP_Input.Button(PlayerID, "Dodge", DodgeButtonDown, DodgeButtonPressed, DodgeButtonUp)),
            (PauseButton = new JP_Input.Button(PlayerID, "Pause", PauseButtonDown, PauseButtonPressed, PauseButtonUp)),
        };
    }

    protected virtual void InitializeAxis()
    {
        _axisHorizontal = $"{PlayerID}_Horizontal";
        _axisVertical = $"{PlayerID}_Vertical";
    }

    protected virtual void LateUpdate()
    {
        ProcessButtonStates();
    }

    protected virtual void Update()
    {
        if (InputDetectionActive)
        {
            SetMovement();
            GetInputButtons();
        }
    }

    public virtual void SetMovement()
    {
        if (InputDetectionActive)
        {
            if (SmoothMovement)
            {
                _primaryMovement.x = Input.GetAxis(_axisHorizontal);
                _primaryMovement.y = Input.GetAxis(_axisVertical);
            }
            else
            {
                _primaryMovement.x = Input.GetAxisRaw(_axisHorizontal);
                _primaryMovement.y = Input.GetAxisRaw(_axisVertical);
            }
        }
    }

    protected virtual void GetInputButtons()
    {
        foreach (JP_Input.Button button in ButtonList)
        {
            if (Input.GetButtonDown(button.ButtonID))
            {
                button.TriggerButtonDown();
            }
            if (Input.GetButton(button.ButtonID))
            {
                button.TriggerButtonPressed();
            }
            if (Input.GetButtonUp(button.ButtonID))
            {
                button.TriggerButtonUp();
            }
        }
    }

    public virtual void ProcessButtonStates()
    {
        // for each button, if we were at ButtonDown this frame, we go to ButtonPressed. If we were at ButtonUp, we're now Off
        foreach (JP_Input.Button button in ButtonList)
        {
            if (button.State.CurrentState == JP_Input.ButtonStates.ButtonDown)
            {
                button.State.ChangeState(JP_Input.ButtonStates.ButtonPressed);
            }
            if (button.State.CurrentState == JP_Input.ButtonStates.ButtonUp)
            {
                button.State.ChangeState(JP_Input.ButtonStates.Off);
            }
        }
    }

    public virtual JP_Input.Button GetButtonFromID(string buttonID)
    {
        JP_Input.Button targetButton = null;

        if (ButtonList == null)
        {
            Debug.Log($"{this.GetType()}.GetButtonFromList: ButtonList not found.", gameObject);
            return null;
        }

        foreach (JP_Input.Button button in ButtonList)
        {
            //Debug.Log($"{this.GetType()}.GetButtonFromList: Checking [{button.ButtonID}].", gameObject);

            if (button.ButtonID == $"{PlayerID}_{buttonID}")
            {
                targetButton = button;
            }
        }

        if ( targetButton == null )
        {
            Debug.LogWarning($"{this.GetType()}.GetButtonFromList: [{PlayerID}_{buttonID}] not found.", gameObject);
        }

        return targetButton;
    }

    #region BUTTON EVENT METHODS

    public virtual void ShootButtonDown()               { ShootButton.State.ChangeState(JP_Input.ButtonStates.ButtonDown); }
    public virtual void ShootButtonPressed()            { ShootButton.State.ChangeState(JP_Input.ButtonStates.ButtonPressed); }
    public virtual void ShootButtonUp()                 { ShootButton.State.ChangeState(JP_Input.ButtonStates.ButtonUp); }

    public virtual void SecondaryShootButtonDown()      { SecondaryShootButton.State.ChangeState(JP_Input.ButtonStates.ButtonDown); }
    public virtual void SecondaryShootButtonPressed()   { SecondaryShootButton.State.ChangeState(JP_Input.ButtonStates.ButtonPressed); }
    public virtual void SecondaryShootButtonUp()        { SecondaryShootButton.State.ChangeState(JP_Input.ButtonStates.ButtonUp); }

    public virtual void InteractButtonDown()            { InteractButton.State.ChangeState(JP_Input.ButtonStates.ButtonDown); }
    public virtual void InteractButtonPressed()         { InteractButton.State.ChangeState(JP_Input.ButtonStates.ButtonPressed); }
    public virtual void InteractButtonUp()              { InteractButton.State.ChangeState(JP_Input.ButtonStates.ButtonUp); }

    public virtual void MeleeButtonDown()               { MeleeButton.State.ChangeState(JP_Input.ButtonStates.ButtonDown); }
    public virtual void MeleeButtonPressed()            { MeleeButton.State.ChangeState(JP_Input.ButtonStates.ButtonPressed); }
    public virtual void MeleeButtonUp()                 { MeleeButton.State.ChangeState(JP_Input.ButtonStates.ButtonUp); }

    public virtual void DodgeButtonDown()               { DodgeButton.State.ChangeState(JP_Input.ButtonStates.ButtonDown); }
    public virtual void DodgeButtonPressed()            { DodgeButton.State.ChangeState(JP_Input.ButtonStates.ButtonPressed); }
    public virtual void DodgeButtonUp()                 { DodgeButton.State.ChangeState(JP_Input.ButtonStates.ButtonUp); }

    public virtual void PauseButtonDown()               { PauseButton.State.ChangeState(JP_Input.ButtonStates.ButtonDown); }
    public virtual void PauseButtonPressed()            { PauseButton.State.ChangeState(JP_Input.ButtonStates.ButtonPressed); }
    public virtual void PauseButtonUp()                 { PauseButton.State.ChangeState(JP_Input.ButtonStates.ButtonUp); }

    #endregion
}
