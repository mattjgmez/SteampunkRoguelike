using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Phoenix.Tools
{
    public class PInput : MonoBehaviour
    {
        public enum ButtonStates { Off, ButtonDown, ButtonPressed, ButtonUp }

        public enum AxisTypes { Positive, Negative }

        public static ButtonStates ProcessAxisAsButton (string axisName, float threshold, ButtonStates currentState, AxisTypes AxisType = AxisTypes.Positive)
        {
            float axisValue = Input.GetAxis (axisName);
            ButtonStates returnState;

            bool comparison = (AxisType == AxisTypes.Positive) ? (axisValue < threshold) : (axisValue > threshold);

            if (comparison)
            {
                if (currentState == ButtonStates.ButtonPressed)
                {
                    returnState = ButtonStates.ButtonUp;
                }
                else
                {
                    returnState = ButtonStates.Off;
                }
            }
            else
            {
                if (currentState == ButtonStates.Off)
                {
                    returnState = ButtonStates.ButtonDown;
                }
                else
                {
                    returnState = ButtonStates.ButtonPressed;
                }
            }
            return returnState;
        }

        public class Button
        {
            public StateMachine<PInput.ButtonStates> State { get; protected set; }
            public string ButtonID;

            public delegate void ButtonDownMethodDelegate();
            public delegate void ButtonPressedMethodDelegate();
            public delegate void ButtonUpMethodDelegate();

            public ButtonDownMethodDelegate ButtonDownMethod;
            public ButtonPressedMethodDelegate ButtonPressedMethod;
            public ButtonUpMethodDelegate ButtonUpMethod;

            public Button(string playerID, string buttonID, ButtonDownMethodDelegate down = null, ButtonPressedMethodDelegate pressed = null, ButtonUpMethodDelegate up = null)
            {
                ButtonID = $"{playerID}_{buttonID}";
                ButtonDownMethod = down;
                ButtonPressedMethod = pressed;
                ButtonUpMethod = up;
                State = new StateMachine<PInput.ButtonStates>(null, false);
                State.ChangeState(PInput.ButtonStates.Off);
            }

            public virtual void TriggerButtonDown()
            {
                if (ButtonDownMethod == null)
                {
                    State.ChangeState(PInput.ButtonStates.ButtonDown);
                }
                else
                {
                    ButtonDownMethod();
                }
            }

            public virtual void TriggerButtonPressed()
            {
                if (ButtonPressedMethod == null)
                {
                    State.ChangeState(PInput.ButtonStates.ButtonPressed);
                }
                else
                {
                    ButtonPressedMethod();
                }
            }

            public virtual void TriggerButtonUp()
            {
                if (ButtonUpMethod == null)
                {
                    State.ChangeState(PInput.ButtonStates.ButtonUp);
                }
                else
                {
                    ButtonUpMethod();
                }
            }
        }
    }
}
