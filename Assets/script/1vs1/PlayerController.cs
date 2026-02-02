using UnityEngine;

// Notez le changement ici : typeof(PlayerInputHandler) au lieu de PlayerInput
[RequireComponent(typeof(PlayerConfig), typeof(PlayerState), typeof(PlayerInputHandler))]
[RequireComponent(typeof(PlayerMouvement), typeof(PlayerAttack))]
public class PlayerController : MonoBehaviour
{
    private PlayerConfig config;
    private PlayerState state;
    private PlayerInputHandler input; // Changement de type ici
    private PlayerMouvement mouvement;
    private PlayerAttack attack;

    // Animation vars
    private float animTimer;
    private int currentFrame;
    private Sprite[] currentAnimSet;
    private PlayerAnimationSet currentAnimData;

    void Awake()
    {
        config = GetComponent<PlayerConfig>();
        state = GetComponent<PlayerState>();
        input = GetComponent<PlayerInputHandler>(); // Changement ici
        mouvement = GetComponent<PlayerMouvement>();
        attack = GetComponent<PlayerAttack>();

        if (config.visualRenderer == null)
            config.visualRenderer = GetComponentInChildren<SpriteRenderer>();

        if (config.characterAnimations.Length > 0)
        {
            config.playerCharacterIndex = Mathf.Clamp(config.playerCharacterIndex, 0, config.characterAnimations.Length - 1);
            currentAnimData = config.characterAnimations[config.playerCharacterIndex];
        }
        currentAnimSet = currentAnimData != null ? currentAnimData.idleSprites : null;

        input.Init(config);
        mouvement.Init(config, input, state);
        attack.Init(config, input, state, this);
    }

    void Update()
    {
        input.ReadInputs();
        mouvement.HandleMovement();
        attack.HandleCombat();

        if (state.isShielding) HandleShieldAnimation();
        else if (!state.isAttacking) HandleManualAnimation();
        else HandleAttackAnimation();
    }

    void HandleManualAnimation()
    {
        if (currentAnimData == null) return;

        Sprite[] target = currentAnimData.idleSprites;

        if (state.isCrouching && currentAnimData.crouchSprites.Length > 0)
            target = currentAnimData.crouchSprites;
        else if (!state.isGrounded && currentAnimData.jumpSprites.Length > 0)
            target = currentAnimData.jumpSprites;
        else if (state.isMoving && currentAnimData.walkSprites.Length > 0)
            target = currentAnimData.walkSprites;

        SwitchAnim(target, config.animSpeed);
    }

    public void SetAttackAnim(Sprite[] sprites, float speed)
    {
        currentAnimSet = sprites;
        currentFrame = 0;
        animTimer = 0f;
    }

    void HandleAttackAnimation()
    {
        float speed = config.attackNormalSpeed;
        if (!state.isGrounded) speed = config.attackAirSpeed;
        else if (state.isCrouching) speed = config.attackCrouchSpeed;
        
        SwitchAnim(currentAnimSet, speed);
    }

    void HandleShieldAnimation()
    {
        if (currentAnimData != null && currentAnimData.shieldSprites != null && currentAnimData.shieldSprites.Length > 0)
            SwitchAnim(currentAnimData.shieldSprites, config.animSpeed);
    }

    void SwitchAnim(Sprite[] target, float speed)
    {
        if (target == null || target.Length == 0) return;

        if (currentAnimSet != target)
        {
            currentAnimSet = target;
            currentFrame = 0;
            animTimer = 0f;
        }

        animTimer += Time.deltaTime;
        if (animTimer >= speed)
        {
            animTimer = 0f;
            currentFrame = (currentFrame + 1) % currentAnimSet.Length;
        }

        if(config.visualRenderer != null)
            config.visualRenderer.sprite = currentAnimSet[currentFrame];
    }
}