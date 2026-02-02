using UnityEngine;

public class PlayerConfig : MonoBehaviour
{
    [Header("Réglages Joueur")]
    public int playerNumber = 1;
    public int playerCharacterIndex = 0;
    public PlayerAnimationSet[] characterAnimations;

    public float moveSpeed = 5f;
    public float jumpForce = 10f;

    [Header("Attaque")]
    public float attackRange = 1.5f;
    public int attackDamage = 10;
    public LayerMask enemyLayers;

    [Header("Vitesse d'animation des attaques")]
    public float attackNormalSpeed = 0.16f;
    public float attackAirSpeed = 0.2f;
    public float attackCrouchSpeed = 0.18f;

    [Header("Bouclier")]
    public float maxShieldTime = 15f;
    public float shieldBurnOutCooldown = 30f;

    [Header("Ground Check")]
    public Transform groundCheck;
    public float groundCheckRadius = 0.12f;
    public LayerMask groundLayer;

    [Header("Visuel")]
    public SpriteRenderer visualRenderer;
    public float animSpeed = 0.1f;
    [Header("Hitbox")]
    public float playerHitboxReduction = 1f;

    public float hitboxScale = 1f;
    [Header("Hitbox Crouch")]
    public Vector2 crouchColliderSize = new Vector2(1f, 1.0f);
}
