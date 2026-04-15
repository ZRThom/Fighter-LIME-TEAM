using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class ControllerSelect : MonoBehaviour
{
    [Header("Select Back")]
    public GameObject defaulSelected;
    void Update()
    {
        if (EventSystem.current.currentSelectedGameObject == null)
        {
            if (Gamepad.current != null && (Gamepad.current.leftStick.ReadValue().magnitude > 0.2f || Gamepad.current.dpad.ReadValue().magnitude > 0.1f))
            {
                EventSystem.current.SetSelectedGameObject(defaulSelected);
            }
        }
    }
}
