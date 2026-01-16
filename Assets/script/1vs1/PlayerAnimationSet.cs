using UnityEngine;

[System.Serializable]
public class PlayerAnimationSet
{
    public string characterName;

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
