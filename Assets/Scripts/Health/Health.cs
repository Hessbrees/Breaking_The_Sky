using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(HealthEvent))]
[DisallowMultipleComponent]
public class Health : MonoBehaviour
{
    [SerializeField] HealthDetailsSO healthDetails;

    private int maxHealth;
    private float currentHealth;

    private float damageMultiplerEffect;
    private float healReductionEffect;

    private int defence;

    private bool isImmuneToDamage;
    private bool isImmuneToHeal;

    private bool isDead;
    private HealthEvent healthEvent;
    private StatusEffects statusEffects;

    private Coroutine damageCoroutine;
    private void Awake()
    {
        healthEvent = GetComponent<HealthEvent>();
        statusEffects = GetComponent<StatusEffects>();
    }

    private void OnEnable()
    {
        statusEffects.statusEffectsEvent.OnStatusEffects += SetHealthStatusEffects_OnStatusEffects;
    }

    private void OnDisable()
    {
        statusEffects.statusEffectsEvent.OnStatusEffects -= SetHealthStatusEffects_OnStatusEffects;
    }

    private void SetHealthStatusEffects_OnStatusEffects(StatusEffectsEvent statusEffectsEvent, StatusEffectsArgs statusEffectsArgs)
    {
        SetHealthStatusEffects(statusEffectsArgs.healReduction, statusEffectsArgs.damageTaken);

        StartDealingDamageOverTime(statusEffectsArgs.damageOverTime);
    }
    private void SetHealthStatusEffects(float healReduction, float damageTaken)
    {
        healReductionEffect = healReduction;
        damageMultiplerEffect = damageTaken;
    }

    private void StartDealingDamageOverTime(float damage)
    {
        if (damageCoroutine != null)
        {
            StopCoroutine(damageCoroutine);
        }

        if (damage == 0) return;

        damageCoroutine = StartCoroutine(DamageCoroutine(damage));
    }
    private IEnumerator DamageCoroutine(float damage)
    {
        while (true)
        {
            TakeDamageOverTime(damage);

            yield return new WaitForSeconds(StatusEffectSettings.damageOverTime_TimePeriod);
        }
    }
    public void TakeDamage(float amount)
    {
        if (!isImmuneToDamage)
        {
            float damage = amount * damageMultiplerEffect - defence;

            if (damage < Settings.minimumDamageToTake) damage = Settings.minimumDamageToTake;

            currentHealth -= damage;

            currentHealth = HelperUtilities.LimitValueToTargetRange(0, maxHealth, currentHealth);
        }
        healthEvent.CallHealthEvent(GetPercentableHealth(), currentHealth, maxHealth, defence, isDead);
    }

    public void TakeDamageOverTime(float amount)
    {
        if (!isImmuneToDamage)
        {
            currentHealth -= amount;

            currentHealth = HelperUtilities.LimitValueToTargetRange(0, maxHealth, currentHealth);
        }
        healthEvent.CallHealthEvent(GetPercentableHealth(), currentHealth, maxHealth, defence, isDead);
    }

    public void Heal(float amount)
    {
        if (!isImmuneToHeal)
        {
            float heal = Mathf.FloorToInt(amount * healReductionEffect);

            currentHealth += heal;

            currentHealth = HelperUtilities.LimitValueToTargetRange(0, maxHealth, currentHealth);
        }

        healthEvent.CallHealthEvent(GetPercentableHealth(), currentHealth, maxHealth, defence, isDead);
    }

    private float GetPercentableHealth()
    {
        return (float)System.Math.Round((double)currentHealth / maxHealth, 2);
    }
    public void SetStartingHealth()
    {
        maxHealth = healthDetails.maximumHealth;
        currentHealth = healthDetails.startingHealth;
        defence = healthDetails.defence;

        healthEvent.CallHealthEvent(GetPercentableHealth(), currentHealth, maxHealth, defence, isDead);
    }
}
