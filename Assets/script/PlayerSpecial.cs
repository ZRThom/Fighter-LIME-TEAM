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
        //check pr eviter le special pendant hit / death
        if (state == null) return;
        if (state.isDead || state.isHit)
        {
            input.SpecialTrigger = false;
            state.isSpecialAttacking = false;
            return;
        }



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

        if (!input.SpecialTrigger) return;

        input.SpecialTrigger = false;

        if (state.isAttacking || state.isShielding || state.isSpecialAttacking || state.animationLocked) return;
        if (rage == null || !rage.IsFull) return;
        if (RageCutInManager.Instance == null) return;

        PlayerAnimationSet data = GetCurrentAnimData();

        if (data == null) return;
        if (data.rageAttackSprites == null || data.rageAttackSprites.Length == 0) return;

        StartCoroutine(SpecialSequence(data));
    }

    IEnumerator SpecialSequence(PlayerAnimationSet data)
    {
        currentSpecialData = data;

        bool launched = RageCutInManager.Instance.PlayCutIn(config.playerNumber, data.rageBackgroundController, data.ragePortraitSprite);
        if (!launched) yield break;

        rage.ConsumeFullRage();

        while (RageCutInManager.Instance != null && RageCutInManager.Instance.IsPlaying)
        {
            if (state == null || state.isDead || state.isHit || state.animationLocked) yield break;
            yield return null;
        }

        if (state == null || state.isDead || state.isHit || state.animationLocked) yield break;
        StartSpecialAttack(data);
    }

    void StartSpecialAttack(PlayerAnimationSet data)
    {
        if (state == null || state.isDead || state.isHit || state.animationLocked) return;
        
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

        if (attack != null && attack.attackPoint != null)point = attack.attackPoint;
        else point = transform;
        
        Collider2D[] hits = Physics2D.OverlapCircleAll(point.position, config.attackRange, config.enemyLayers);

        foreach (Collider2D enemy in hits)
        {
            PlayerHealth health = enemy.GetComponentInParent<PlayerHealth>();
            Manequin manequin = enemy.GetComponentInParent<Manequin>();

            if (health == null && manequin == null) continue;

            if (health != null)
            {
                if (health.gameObject == this.gameObject) continue;

                if (health.playerID == config.playerNumber) continue;
                health.TakeDamage(currentSpecialData.rageDamage);
            }
            
            if (manequin != null) manequin.TakeDamage(currentSpecialData.rageDamage);
            
            Vector3 hitPos = enemy.bounds.center;
            TriggerSpecialEffect(hitPos);
            break;
        }

        void TriggerSpecialEffect(Vector3 effectPosition)
        {
            if (currentSpecialData == null) return;
            if (currentSpecialData.rageEffectPrefab == null) return;

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
                
                case RageHitEffectType.Rain:
                    {
                        Debug.Log("Rain launched");
                        GameObject obj = Instantiate(currentSpecialData.rageEffectPrefab);
                        RageRain rain = obj.GetComponent<RageRain>();
                        bool fromRight = transform.position.x > effectPosition.x;

                        if (rain != null)
                        {
                            rain.Play(fromRight);
                        }
                        else
                        {
                            obj.transform.position = effectPosition;
                        }
                        break;
                    }
                
                case RageHitEffectType.Shockwave:
                    {
                        Debug.Log("Shockwave launched");
                        SpawnShockwave(effectPosition, currentSpecialData.rageEffectScale);
                        SpawnShockwave(effectPosition, currentSpecialData.rageEffectScale / 5f);
                        void SpawnShockwave(Vector3 pos, Vector3 endScale)
                        {
                            GameObject obj = Instantiate(currentSpecialData.rageEffectPrefab, pos, Quaternion.identity);
                            
                            RageShockwave shockwave = obj.GetComponent<RageShockwave>();
                            if (shockwave != null)
                            {
                                shockwave.Play(pos, endScale);
                            }
                        }
                        
                        break;
                    }
            }
        }
    }

    // Takes the characterAnim array + character index and returns the character data
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
