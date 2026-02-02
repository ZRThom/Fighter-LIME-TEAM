using UnityEngine;

[System.Serializable]
public class PlayerAnimationSet
{
    

    [Header("Character Info")]
    public string characterName;
    
    public Sprite characterIcon;
    [Header("Prefab du personnage")]
    public GameObject prefab;

    [Header("<-- Animations Sprites -->>")]

    [Header("Mouvement")]
    public Sprite[] idleSprites;
    public Sprite[] walkSprites;
    public Sprite[] jumpSprites;
    public Sprite[] crouchSprites;

    [Header("Attaques")]
    public Sprite[] attackSprites;
    public Sprite[] attackAirSprites;
    public Sprite[] attackCrouchSprites;

    [Header("Bouclier")]
    public Sprite[] shieldSprites;
    
}
