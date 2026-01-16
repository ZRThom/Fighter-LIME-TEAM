using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputHandler : MonoBehaviour
{
    public Vector2 MoveInput { get; private set; }
    public bool JumpTriggered { get; set; }
    public bool AttackTriggered { get; set; }
    public bool ShieldPressed { get; private set; }
    public bool CrouchHeld { get; private set; }

    private PlayerConfig config;

    public void Init(PlayerConfig pc)
    {
        config = pc;
    }

    public void ReadInputs()
    {
        float h = 0f;

        if (config.playerNumber == 1)
        {
            if (Keyboard.current.aKey.isPressed) h = -1f;
            else if (Keyboard.current.dKey.isPressed) h = 1f;

            if (Keyboard.current.spaceKey.wasPressedThisFrame) JumpTriggered = true;
            CrouchHeld = Keyboard.current.sKey.isPressed;
            if (Mouse.current.leftButton.wasPressedThisFrame) AttackTriggered = true;
            ShieldPressed = Keyboard.current.eKey.isPressed;
        }
        else
        {
            if (Keyboard.current.leftArrowKey.isPressed) h = -1f;
            else if (Keyboard.current.rightArrowKey.isPressed) h = 1f;

            if (Keyboard.current.rightShiftKey.wasPressedThisFrame) JumpTriggered = true;
            CrouchHeld = Keyboard.current.downArrowKey.isPressed;
            if (Mouse.current.rightButton.wasPressedThisFrame) AttackTriggered = true;
            ShieldPressed = Keyboard.current.rKey.isPressed;
        }

        MoveInput = new Vector2(h, 0f);
    }
}