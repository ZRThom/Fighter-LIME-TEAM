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
    
}
