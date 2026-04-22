using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputHandler : MonoBehaviour
{
    [Header("Configuration")]
    public bool allowController = true;

    [Header("IA Settings")]
    public float aiAgroRange = 10f;
    public float aiAttackRangeDist = 1.5f;
    private float aiActionCooldown = 0f;
    private float aiHoldTimer = 0f;
    private bool aiHoldingShield = false;
    private bool aiHoldingCrouch = false;
    private float aiMoveTimer = 0f;
    private float aiMoveDir = 0f;

    public bool InputsEnabled { get; private set; } = true;
    public Vector2 MoveInput { get; private set; }
    public bool JumpTriggered { get; set; }
    public bool AttackTriggered { get; set; }
    public bool ShieldPressed { get; private set; }
    public bool CrouchHeld { get; private set; }

    public bool SpecialTrigger { get; set; }

    private PlayerConfig config;
    private PlayerState state;

    public void Init(PlayerConfig pc)
    {
        config = pc;
        state = GetComponent<PlayerState>();
    }

    // check pr eviter les input en memoire
    public void SetInputsEnabled(bool enabled)
    {
        InputsEnabled = enabled;
        if (!enabled) ClearInputs();
    }
    
    public void ClearInputs()
    {
        MoveInput = Vector2.zero;
        JumpTriggered = false;
        AttackTriggered = false;
        ShieldPressed = false;
        CrouchHeld = false;
        SpecialTrigger = false;
    }

    public void ReadInputs()
    {
        // === AI INTEGRATION ===
        // If the character is flagged as an AI in its PlayerState, we bypass the 
        // keyboard/gamepad reading and directly execute the AI "brain".

        if (!InputsEnabled || (state != null && (state.isDead || state.isHit)))
        {
            ClearInputs();
            return;
        }

        if (state != null && state.isAI)
        {
            ProcessAILogic();
            return;
        }

        float h = 0f;
        bool jump = false;
        bool attack = false;
        bool shield = false;
        bool crouch = false;

        bool special = false;


        if (Keyboard.current != null)
        {
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

            if (gamepadIndex >= 0 && gamepadIndex < Gamepad.all.Count)
            {
                var myGamepad = Gamepad.all[gamepadIndex];

                float gamepadX = myGamepad.leftStick.x.ReadValue();
                if (Mathf.Abs(gamepadX) > 0.1f) h = gamepadX;

                if (myGamepad.buttonSouth.wasPressedThisFrame) jump = true;
                if (myGamepad.buttonWest.wasPressedThisFrame) attack = true;
                if (myGamepad.rightShoulder.isPressed) shield = true;
                if (myGamepad.buttonNorth.wasPressedThisFrame) special = true;
                
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

    // === AI BEHAVIOR ===
    // Autonomously generates virtual inputs (movement, attack, jump, ultimate...).
    void ProcessAILogic()
    {
        float h = 0f;
        bool jump = false;
        bool attack = false;
        bool shield = false;
        bool crouch = false;
        bool special = false;

        if (state == null || state.isDead || state.isHit)
        {
            ClearInputs();
            return;
        }

        // 1. TARGETING: Find opponent if null.
        if (config.opponentTransform == null)
        {
            PlayerHealth[] players = FindObjectsOfType<PlayerHealth>();
            foreach (PlayerHealth p in players)
            {
                if (p.gameObject != this.gameObject)
                {
                    config.opponentTransform = p.transform;
                    break;
                }
            }
            if (config.opponentTransform == null) return;
        }

        // 2. TIMERS: Update cooldowns and hold states.
        aiActionCooldown -= Time.deltaTime;
        aiHoldTimer -= Time.deltaTime;
        aiMoveTimer -= Time.deltaTime;

        if (aiHoldTimer <= 0f)
        {
            aiHoldingShield = false;
            aiHoldingCrouch = false;
        }

        // 3. SPATIAL ANALYSIS: Calculate distance and direction.
        float distance = Vector2.Distance(transform.position, config.opponentTransform.position);
        float directionX = config.opponentTransform.position.x - transform.position.x;
        float directionY = config.opponentTransform.position.y - transform.position.y;

        PlayerRage rage = GetComponent<PlayerRage>();
        bool hasFullRage = rage != null && rage.IsFull;

        // Cancel defense if Ultimate is ready.
        if (hasFullRage && !state.isSpecialAttacking && !state.isAttacking)
        {
            aiHoldingShield = false;
            aiHoldingCrouch = false;
        }

        float effectiveRange = hasFullRage ? (aiAttackRangeDist + 0.8f) : aiAttackRangeDist;
        bool inAttackRange = Mathf.Abs(directionX) <= effectiveRange && Mathf.Abs(directionY) < 2f;

        // === OUT OF RANGE BEHAVIOR ===
        if (!inAttackRange && distance < aiAgroRange)
        {
            if (hasFullRage)
            {
                // A. WITH ULTIMATE: Rush the player.
                h = Mathf.Sign(directionX);

                if (state.isGrounded && aiActionCooldown <= 0f && directionY > 1.5f)
                {
                    jump = true;
                    aiActionCooldown = 0.5f;
                }
            }
            else
            {
                // B. WITHOUT ULTIMATE: Random movement decisions.
                if (aiMoveTimer <= 0f)
                {
                    float rand = Random.value;
                    if (rand < 0.35f) // 35%: Advance
                    {
                        aiMoveDir = Mathf.Sign(directionX);
                        aiMoveTimer = Random.Range(0.4f, 1.2f);
                    }
                    else if (rand < 0.70f) // 35%: Retreat
                    {
                        aiMoveDir = -Mathf.Sign(directionX);
                        aiMoveTimer = Random.Range(0.2f, 0.6f);
                    }
                    else if (rand < 0.85f) // 15%: Jump forward
                    {
                        aiMoveDir = Mathf.Sign(directionX);
                        jump = true;
                        aiMoveTimer = Random.Range(0.4f, 0.8f);
                    }
                    else // 15%: Stop and attack
                    {
                        aiMoveDir = 0f;
                        attack = true;
                        aiMoveTimer = Random.Range(0.2f, 0.5f);
                    }
                }

                h = aiMoveDir;

                // Force return if too far.
                if (distance > aiAgroRange * 0.8f) h = Mathf.Sign(directionX);

                // Follow airborne player.
                if (state.isGrounded && aiActionCooldown <= 0f && directionY > 1.5f)
                {
                    jump = true;
                    aiActionCooldown = Random.Range(0.5f, 1f);
                }
            }
        }
        // === MELEE COMBAT BEHAVIOR ===
        else if (inAttackRange)
        {
            // Micro-adjustments.
            if (aiMoveTimer <= 0f)
            {
                aiMoveDir = hasFullRage ? 0f : (distance > (aiAttackRangeDist * 0.7f) ? Mathf.Sign(directionX) * 0.5f : (Random.value < 0.3f ? -Mathf.Sign(directionX) * 0.5f : 0f));
                aiMoveTimer = Random.Range(0.2f, 0.5f);
            }
            h = aiMoveDir;

            // A. WITH ULTIMATE: Instant cast.
            if (hasFullRage && !state.isAttacking && !state.isSpecialAttacking && !state.isShielding)
            {
                special = true;
                aiActionCooldown = 1.5f;
            }
            // B. WITHOUT ULTIMATE: Random combat actions.
            else if (aiActionCooldown <= 0f && !state.isAttacking && !state.isSpecialAttacking && !state.isShielding)
            {
                int randomAction = Random.Range(0, 100);

                if (randomAction < 40) // 40%: Basic attack
                {
                    attack = true;
                    aiActionCooldown = Random.Range(0.2f, 0.5f);
                }
                else if (randomAction < 60) // 20%: Crouch attack
                {
                    aiHoldingCrouch = true;
                    aiHoldTimer = 0.4f;
                    attack = true;
                    aiActionCooldown = Random.Range(0.4f, 0.7f);
                }
                else if (randomAction < 75) // 15%: Jump attack
                {
                    jump = true;
                    attack = true;
                    aiActionCooldown = Random.Range(0.6f, 1.2f);
                }
                else // 25%: Defense or retreat
                {
                    if (Random.value < 0.6f)
                    {
                        aiHoldingShield = true;
                        aiHoldTimer = Random.Range(0.3f, 0.8f);
                        aiActionCooldown = aiHoldTimer + 0.1f;
                    }
                    else
                    {
                        aiMoveDir = -Mathf.Sign(directionX);
                        aiMoveTimer = Random.Range(0.3f, 0.6f);
                        aiActionCooldown = 0.3f;
                    }
                }
            }
        }

        // Apply hold states.
        if (aiHoldingShield) shield = true;
        if (aiHoldingCrouch) crouch = true;

        // 4. RESOLUTION: Send virtual inputs.
        MoveInput = new Vector2(h, 0f);
        if (jump) JumpTriggered = true;
        if (attack) AttackTriggered = true;
        if (special) SpecialTrigger = true;
        ShieldPressed = shield;
        CrouchHeld = crouch;
    }
}