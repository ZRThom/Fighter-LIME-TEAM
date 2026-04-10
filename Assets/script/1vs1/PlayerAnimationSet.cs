using UnityEngine;

[System.Serializable]
public class PlayerAnimationSet
{
    [Header("Character Info")]
    public string characterName;
    public Sprite characterIcon;
    
    [Header("Prefab Character")]
    public GameObject prefab;

    [Header("Sprite Animation")]

    [Header("Movement")]
    public Sprite[] idleSprites;
    public Sprite[] walkSprites;
    public Sprite[] jumpSprites;
    public Sprite[] crouchSprites;

    [Header("Attack")]
    public Sprite[] attackSprites;
    public Sprite[] attackAirSprites;
    public Sprite[] attackCrouchSprites;

    [Header("Shield")]
    public Sprite[] shieldSprites;

    [Header("Rage cut")]
    public RuntimeAnimatorController rageBackgroundController;
    public Sprite ragePortraitSprite;

    [Header("Rage Special Attack")]
    public Sprite[] rageAttackSprites;
    public float rageAttackSpeed = 0.15f;
    [Range(0f, 1f)] public float rageHitPercent = 0.5f;
    public int rageDamage = 30;
    public RageHitEffectType rageHitEffect = RageHitEffectType.None;
    public GameObject rageEffectPrefab;
    public Vector3 rageEffectScale = Vector3.one;
}
