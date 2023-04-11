using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(HealthEvent))]
[DisallowMultipleComponent]
public class Health : MonoBehaviour
{
    [SerializeField] HealthDetailsSO healthDetails;

    private int maxHealth;
    private int currentHealth;

    private float damageMultiplerEffect;
    private float regenerationMultiplerEffect;

    private int defence;

    private bool isImmuneToDamage;
    private bool isImmuneToHeal;

    private bool isDead;
    private HealthEvent healthEvent;

    private void Awake()
    {
        healthEvent = GetComponent<HealthEvent>();
    }

    public void TakeDamage(int amount)
    {
        if (!isImmuneToDamage)
        {
            int damage = Mathf.FloorToInt(amount * damageMultiplerEffect - defence);

            if (damage < Settings.minimumDamageToTake) damage = Settings.minimumDamageToTake;

            currentHealth -= damage;

            currentHealth = (int)HelperUtilities.LimitValueToTargetRange(0, maxHealth, currentHealth);

        }

        healthEvent.CallHealthEvent(GetPercentableHealth(), currentHealth, maxHealth,defence, isDead);
    }

    public void Heal(int amount)
    {
        if (!isImmuneToHeal)
        {
            int heal = Mathf.FloorToInt(amount * regenerationMultiplerEffect);

            currentHealth += heal;

            currentHealth = (int)HelperUtilities.LimitValueToTargetRange(0, maxHealth, currentHealth);
        }

        healthEvent.CallHealthEvent(GetPercentableHealth(), currentHealth, maxHealth,defence, isDead);
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

        healthEvent.CallHealthEvent(GetPercentableHealth(), currentHealth, maxHealth,defence, isDead);
    }
}
