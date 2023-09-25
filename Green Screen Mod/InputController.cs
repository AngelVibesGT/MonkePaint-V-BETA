using UnityEngine;

namespace Green_Screen_Mod
{
    public class InputController : MonoBehaviour
    {
        public static InputController instance;

        public bool gripDown;
        public bool triggerDown;
        public bool primaryDown;
        public bool Wiping;
        public bool TriggerToggle;
        public bool SecondaryToggle;
        public bool LPrimaryDown;
        public bool LPrimaryToggle;
        public bool FreezeBrush;
        public bool Drawing;
        public bool Dropping;

        public float GSY;

        void Start()
        {
            InputController.instance = this;
        }

        void Update()
        {
            try
            {
                //primary button
                if (ControllerInputPoller.instance.rightControllerPrimaryButton)
                {
                    primaryDown = true;
                }
                else
                {
                    primaryDown = false;
                }
                //grip button
                if (ControllerInputPoller.instance.rightControllerGripFloat > 0)
                {
                    gripDown = true;
                }
                else
                {
                    gripDown = false;
                }
                //if the brush isnt frozen, left joystick y pos
                if (!FreezeBrush)
                {
                    GSY = ControllerInputPoller.instance.rightControllerPrimary2DAxis.y;
                }
                //trigger button
                if (ControllerInputPoller.instance.rightControllerIndexFloat > 0)
                {
                    triggerDown = true;
                }
                else
                {
                    triggerDown = false;
                }
                // secondary button
                if (ControllerInputPoller.instance.rightControllerSecondaryButton)
                {
                    SecondaryToggle = true;
                }
                else
                {
                    SecondaryToggle = false;
                }
                // left primary button
                if (ControllerInputPoller.instance.leftControllerPrimaryButton)
                {
                    LPrimaryDown = true;
                }
                else
                {
                    LPrimaryDown = false;
                }
            }
            catch
            {

            }
        }
    }
}
