using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputHandler : MonoBehaviour
{
    [Header("Configuration")]
    public bool allowController = true; 

    public Vector2 MoveInput { get; private set; }
    public bool JumpTriggered { get; set; }
    public bool AttackTriggered { get; set; }
    public bool ShieldPressed { get; private set; }
    public bool CrouchHeld { get; private set; }

    public bool SpecialTrigger { get; set; }

    private PlayerConfig config;

    public void Init(PlayerConfig pc)
    {
        config = pc;
    }

    // check pr eviter les input en memoire
    //void Update()
    //{
    //    if (RageCutInManager.Instance != null && RageCutInManager.Instance.IsPlaying)
    //    {
    //        input.ClearInputs();
    //        
    //    }
    //}
    
    //public void ClearInputs()
    //{
    //    MoveInput = Vector2.zero;
    //    JumpTriggered = false;
    //    AttackTriggered = false;
    //    ShieldPressed = false;
    //    CrouchHeld = false;
    //    SpecialTrigger = false;
    //}

    public void ReadInputs()
    {
        float h = 0f;
        bool jump = false;
        bool attack = false;
        bool shield = false;
        bool crouch = false;

        bool special = false;


        if (Keyboard.current != null)
        {
            // --- JOUEUR 1 : ZQSD / WASD ---
            if (config.playerNumber == 1)
            {
                if (Keyboard.current.aKey.isPressed) h = -1f;
                else if (Keyboard.current.dKey.isPressed) h = 1f;

                if (Keyboard.current.spaceKey.wasPressedThisFrame) jump = true;
                if (Keyboard.current.sKey.isPressed) crouch = true;
                
                if (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame) attack = true;
                if (Keyboard.current.eKey.isPressed) shield = true;

                if (Keyboard.current.tKey.wasPressedThisFrame) special = true;
            }
            // --- JOUEUR 2 : Flèches ---
            else
            {
                if (Keyboard.current.leftArrowKey.isPressed) h = -1f;
                else if (Keyboard.current.rightArrowKey.isPressed) h = 1f;

                if (Keyboard.current.rightShiftKey.wasPressedThisFrame || Keyboard.current.upArrowKey.wasPressedThisFrame) jump = true;
                if (Keyboard.current.downArrowKey.isPressed) crouch = true;
                
                if (Mouse.current != null && Mouse.current.rightButton.wasPressedThisFrame) attack = true;
                
                if (Keyboard.current.rightCtrlKey.isPressed) shield = true; 

                if (Keyboard.current.yKey.wasPressedThisFrame) special = true;
            }
        }

        if (allowController)
        {
            int gamepadIndex = config.playerNumber - 1; 

            if (Gamepad.all.Count > gamepadIndex)
            {
                var myGamepad = Gamepad.all[gamepadIndex];

                // MOUVEMENT
                float gamepadX = myGamepad.leftStick.x.ReadValue();
                if (Mathf.Abs(gamepadX) > 0.1f) h = gamepadX; 

                // ACTIONS
                if (myGamepad.buttonSouth.wasPressedThisFrame) jump = true;
                if (myGamepad.buttonWest.wasPressedThisFrame) attack = true;
                if (myGamepad.rightShoulder.isPressed) shield = true;
                if (myGamepad.buttonNorth.wasPressedThisFrame) special = true;
                
                // Crouch
                bool stickDown = myGamepad.leftStick.y.ReadValue() < -0.5f;
                bool dpadDown = myGamepad.dpad.down.isPressed;
                if (stickDown || dpadDown) crouch = true;
            }
        }


        MoveInput = new Vector2(h, 0f);
        
        if (jump) JumpTriggered = true;
        if (attack) AttackTriggered = true;
        if (special) SpecialTrigger = true;
        
        ShieldPressed = shield;
        CrouchHeld = crouch;
    }
}