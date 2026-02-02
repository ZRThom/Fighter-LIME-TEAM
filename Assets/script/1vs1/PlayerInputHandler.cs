using UnityEngine;
using UnityEngine.InputSystem; // Nécessaire pour le nouveau système

public class PlayerInputHandler : MonoBehaviour
{
    // --- NOUVEAU : Case à cocher dans l'inspecteur ---
    [Header("Contrôles")]
    public bool useController = false; 

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

        // --- JOUEUR 1 (Clavier : ZQSD / WASD) ---
        if (config.playerNumber == 1 && !useController)
        {
            if (Keyboard.current != null)
            {
                if (Keyboard.current.aKey.isPressed) h = -1f;
                else if (Keyboard.current.dKey.isPressed) h = 1f;

                if (Keyboard.current.spaceKey.wasPressedThisFrame) JumpTriggered = true;
                CrouchHeld = Keyboard.current.sKey.isPressed;
                
                if (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame) AttackTriggered = true;
                if (Keyboard.current.eKey.isPressed) ShieldPressed = true;
                else ShieldPressed = false;
            }
        }
        // --- JOUEUR 2 ou MANETTE ---
        else
        {
            // SI on veut utiliser la manette ET qu'une manette est connectée
            if (useController && Gamepad.current != null)
            {
                var gamepad = Gamepad.current;

                // 1. Mouvement (Stick Gauche)
                h = gamepad.leftStick.x.ReadValue();
                
                // Zone morte pour éviter que ça bouge tout seul (si stick un peu vieux)
                if (Mathf.Abs(h) < 0.1f) h = 0f;

                // 2. Saut (Bouton Sud = A sur Xbox / Croix sur PS)
                if (gamepad.buttonSouth.wasPressedThisFrame) JumpTriggered = true;

                // 3. Crouch (Stick vers le bas ou Flèche Bas du D-Pad)
                bool stickDown = gamepad.leftStick.y.ReadValue() < -0.5f;
                bool dpadDown = gamepad.dpad.down.isPressed;
                CrouchHeld = stickDown || dpadDown;

                // 4. Attaque (Bouton Ouest = X sur Xbox / Carré sur PS)
                if (gamepad.buttonWest.wasPressedThisFrame) AttackTriggered = true;

                // 5. Bouclier (Gâchette Droite ou R1)
                ShieldPressed = gamepad.rightShoulder.isPressed;
            }
            // SINON : On utilise les flèches du clavier (Fallback)
            else if (Keyboard.current != null)
            {
                if (Keyboard.current.leftArrowKey.isPressed) h = -1f;
                else if (Keyboard.current.rightArrowKey.isPressed) h = 1f;

                if (Keyboard.current.rightShiftKey.wasPressedThisFrame) JumpTriggered = true;
                CrouchHeld = Keyboard.current.downArrowKey.isPressed;
                
                if (Mouse.current != null && Mouse.current.rightButton.wasPressedThisFrame) AttackTriggered = true;
                
                if (Keyboard.current.rKey.isPressed) ShieldPressed = true;
                else ShieldPressed = false;
            }
        }

        MoveInput = new Vector2(h, 0f);
    }
}