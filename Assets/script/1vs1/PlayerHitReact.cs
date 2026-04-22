using UnityEngine;
using System.Collections;

public class PlayerHitReact : MonoBehaviour
{
    private PlayerConfig config;
    private PlayerState state;
    private PlayerInputHandler input;
    private Rigidbody2D rb;
    private SpriteRenderer sr;

    private Coroutine currentRoutine;

    private void Awake()
    {
        config = GetComponent<PlayerConfig>();
        state = GetComponent<PlayerState>();
        input = GetComponent<PlayerInputHandler>(); 
        rb = GetComponent<Rigidbody2D>();

        if (config != null)
        {
            //fallbakc spriteRenderer
            if (config.visualRenderer == null) config.visualRenderer = GetComponentInChildren<SpriteRenderer>();
            sr = config.visualRenderer;
        }
    }

    public void PlayHit()
    {
        if (state == null || config == null || sr == null) return;
        if (state.isDead) return;

        var animData = config.characterAnimations[config.playerCharacterIndex];

        if (animData.hitSprites == null || animData.hitSprites.Length == 0) return;
        if (currentRoutine != null) StopCoroutine(currentRoutine);

        currentRoutine = StartCoroutine(HitRoutine(animData));
    }

    public void PlayDeath()
    {
        if (state == null || config == null || sr == null) return;
        if (state.isDead) return;

        var animData = config.characterAnimations[config.playerCharacterIndex];

        if (animData.deathSprites == null || animData.deathSprites.Length == 0) return;
        if (currentRoutine != null) StopCoroutine(currentRoutine);

        currentRoutine = StartCoroutine(DeathRoutine(animData));
    }

    private IEnumerator HitRoutine(PlayerAnimationSet animData)
    {
        state.isHit = true;
        state.animationLocked = true;
        state.isAttacking = false;
        state.isSpecialAttacking = false;
        state.isShielding = false;

        if (input != null) input.ClearInputs();

        if (rb != null) rb.linearVelocity = Vector2.zero;

        yield return StartCoroutine(PlaySpriteArray(animData.hitSprites, animData.hitSpeed, false));

        state.isHit = false;
        state.animationLocked = false;
        currentRoutine = null;
    }

    private IEnumerator DeathRoutine(PlayerAnimationSet animData)
    {
        state.isDead = true;
        state.isHit = false;
        state.animationLocked = true;
        state.isAttacking = false;
        state.isSpecialAttacking = false;
        state.isShielding = false;

        if (input != null)
        {
            input.SetInputsEnabled(false);
            input.ClearInputs();
        }

        if (rb != null) rb.linearVelocity = Vector2.zero;

        if (animData.deathSprites != null && animData.deathSprites.Length > 0)
        {
            yield return StartCoroutine(PlaySpriteArray(animData.deathSprites, animData.deathSpeed, false));
        }
        
        if (animData.deathGroundSprites != null && animData.deathGroundSprites.Length > 0)
        {
            sr.sprite = animData.deathGroundSprites[animData.deathGroundSprites.Length - 1];
        }

        currentRoutine = null;
    }

    private IEnumerator PlaySpriteArray(Sprite[] sprites, float speed, bool loop)
    {
        if (sprites == null || sprites.Length == 0)
        {
            yield break;
        }

        if (!loop)
        {
            for (int i = 0; i < sprites.Length; i++)
            {
                sr.sprite = sprites[i];
                yield return new WaitForSeconds(speed);
            }
        }
        else
        {
            int index = 0;
            while (true)
            {
                sr.sprite = sprites[index];
                index = (index + 1) % sprites.Length;
                yield return new WaitForSeconds(speed);
            }
        }
    }

    public void ResetReactionState()
    {
        if (currentRoutine != null) StopCoroutine(currentRoutine);

        currentRoutine = null;

        if (state != null)
        {
            state.isHit = false;
            state.isDead = false;
            state.animationLocked = false;
        }

        if (input != null)
        {
            input.SetInputsEnabled(true);
            input.ClearInputs();
        }
    }
}
