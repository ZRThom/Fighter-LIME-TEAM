using UnityEngine;
using System.Collections;

[RequireComponent(typeof(PlayerRage))]
public class PlayerSpecial : MonoBehaviour
{
    private PlayerConfig config;
    private PlayerInputHandler input;
    private PlayerRage rage;
    private PlayerState state;
    private PlayerController controller;
    private PlayerAttack attack;
    private bool hasHitThisSpecial;
    private float specialTimer;
    private float hitMoment;
    private PlayerAnimationSet currentSpecialData;

    public void Init(PlayerConfig pc, PlayerInputHandler pi, PlayerState ps, PlayerController ctrl)
    {
        config = pc;
        input = pi;
        state = ps;
        controller = ctrl;
        rage = GetComponent<PlayerRage>();
        attack = GetComponent<PlayerAttack>();
    }

    public void HandleSpecial()
    {
        if (state.isSpecialAttacking)
        {
            specialTimer -= Time.deltaTime;
            if (!hasHitThisSpecial && specialTimer <= hitMoment)
            {
                PerformSpecialHit();
                hasHitThisSpecial = true;
            }

            if (specialTimer <= 0f)
            {
                state.isSpecialAttacking = false;
            }
            return;
        }
        if (!input.SpecialTrigger)
        {
            return;
        }
        input.SpecialTrigger = false;

        if (state.isAttacking || state.isShielding || state.isSpecialAttacking)
        {
            return;
        }

        if (rage == null || !rage.IsFull)
        {
            return;
        }

        if (RageCutInManager.Instance == null)
        {
            return;
        }

        PlayerAnimationSet data = GetCurrentAnimData();
        if (data == null)
        {
            return;
        }

        if (data.rageAttackSprites == null || data.rageAttackSprites.Length == 0)
        {
            return;
        }
        StartCoroutine(SpecialSequence(data));
    }

    IEnumerator SpecialSequence(PlayerAnimationSet data)
    {
        currentSpecialData = data;
        bool launched = RageCutInManager.Instance.PlayCutIn(config.playerNumber, data.rageBackgroundController, data.ragePortraitSprite);
        if (!launched)
        {
            yield break;
        }
        rage.ConsumeFullRage();

        while (RageCutInManager.Instance != null && RageCutInManager.Instance.IsPlaying)
        {
            yield return null;
        }

        StartSpecialAttack(data);
    }

    void StartSpecialAttack(PlayerAnimationSet data)
    {
        state.isAttacking = false;
        state.isShielding = false;
        state.isSpecialAttacking = true;
        hasHitThisSpecial = false;

        specialTimer = data.rageAttackSpeed * data.rageAttackSprites.Length;
        hitMoment = specialTimer * Mathf.Clamp01(data.rageHitPercent);
        controller.SetSpecialAnim(data.rageAttackSprites, data.rageAttackSpeed);
    }

    void PerformSpecialHit()
    {
        Transform point = null;
        if (attack != null && attack.attackPoint != null)
        {
            point = attack.attackPoint;
        }
        else
        {
            point = transform;
        }
        Collider2D[] hits = Physics2D.OverlapCircleAll(point.position, config.attackRange, config.enemyLayers);

        foreach (Collider2D enemy in hits)
        {
            PlayerHealth health = enemy.GetComponentInParent<PlayerHealth>();
            if (health == null)
            {
                continue;
            }

            if (health.playerID == config.playerNumber)
            {
                continue;
            }
            // ordre pr pas que les rage effect deconne (stock pos / effect / degat)
            Vector3 hitPos = enemy.bounds.center;
            TriggerSpecialEffect(hitPos);
            health.TakeDamage(currentSpecialData.rageDamage);
            break;
        }

        void TriggerSpecialEffect(Vector3 effectPosition)
        {
            if (currentSpecialData == null) return;
            if (currentSpecialData.rageEffectPrefab == null) return;

            // select rafe effect
            switch (currentSpecialData.rageHitEffect)
            {
                case RageHitEffectType.Beam:
                    {
                        Debug.Log("Beam launched");
                        GameObject obj = Instantiate(currentSpecialData.rageEffectPrefab);
                        RageBeamLine beam = obj.GetComponent<RageBeamLine>();
                        if (beam != null)
                        {
                            beam.Play(state.facingRight, effectPosition.y);
                        }
                        else
                        {
                            obj.transform.position = effectPosition;
                        }
                        break;
                    }
                
                case RageHitEffectType.Lightning:
                    {
                        Debug.Log("Lightning launched");
                        Vector3 spawnPos = effectPosition + Vector3.up * 0.5f;
                        spawnPos.z = 0f;
                        GameObject fx = Instantiate(currentSpecialData.rageEffectPrefab, spawnPos, Quaternion.identity);
                        fx.transform.localScale = currentSpecialData.rageEffectScale;
                        SpriteRenderer sr = fx.GetComponent<SpriteRenderer>();
                        if (sr != null)
                        {
                            sr.sortingLayerName = "FX";
                            sr.sortingOrder = 200;
                        }
                        break;
                    }
            }
        }
    }

    // prend tableau charactAnim... + index du perso et retourne les donnée du perso
    PlayerAnimationSet GetCurrentAnimData()
    {
        if (config.characterAnimations == null || config.characterAnimations.Length == 0)
        {
            return null;
        }

        int index = Mathf.Clamp(config.playerCharacterIndex, 0, config.characterAnimations.Length - 1);
        return config.characterAnimations[index];
    }
}
