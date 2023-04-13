using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(StatusEffectsEvent))]
[DisallowMultipleComponent]
public class StatusEffects : MonoBehaviour
{
    private float damageReduction = 0;
    private float healReduction = 0;
    private float damageOverTime = 0;
    private float movementReduction = 0;
    private float damageTaken = 0;

    [HideInInspector] public StatusEffectsEvent statusEffectsEvent;

    private int currentLevelRadiationSickness = 0;

    private float radiationSicknessDamageReduction;
    private float radiationSicknessHealReduction;
    private float radiationSicknessDamage;
    private float radiationSicknessMovementReduction;
    private float radiationSicknessDamageTaken;

    private void Awake()
    {
        statusEffectsEvent = GetComponent<StatusEffectsEvent>();
    }

    private void UpdateCurrentStatus()
    {
        damageReduction = radiationSicknessDamageReduction;
        healReduction = radiationSicknessHealReduction;
        damageOverTime = radiationSicknessDamage;
        movementReduction = radiationSicknessMovementReduction;
        damageTaken = radiationSicknessDamageTaken;
    }

    public void ActivateRadiationSickness(int level)
    {
        if (level == 1 && currentLevelRadiationSickness < level)
        {
            radiationSicknessDamageReduction += StatusEffectSettings.radiationSicknessDamageReduction_Lvl1;
            radiationSicknessHealReduction += StatusEffectSettings.radiationSicknessHealReduction_Lvl1;
            radiationSicknessDamage += StatusEffectSettings.radiationSicknessDamage_Lvl1;
            radiationSicknessMovementReduction += StatusEffectSettings.radiationSicknessMovementReduction_Lvl1;
            radiationSicknessDamageTaken += StatusEffectSettings.radiationSicknessDamageTaken_Lvl1;
        }
        else if (level == 2 && currentLevelRadiationSickness < level)
        {
            radiationSicknessDamageReduction += StatusEffectSettings.radiationSicknessDamageReduction_Lvl2;
            radiationSicknessHealReduction += StatusEffectSettings.radiationSicknessHealReduction_Lvl2;
            radiationSicknessDamage += StatusEffectSettings.radiationSicknessDamage_Lvl2;
            radiationSicknessMovementReduction += StatusEffectSettings.radiationSicknessMovementReduction_Lvl2;
            radiationSicknessDamageTaken += StatusEffectSettings.radiationSicknessDamageTaken_Lvl2;
        }
        else if (level == 3 && currentLevelRadiationSickness < level)
        {
            radiationSicknessDamageReduction += StatusEffectSettings.radiationSicknessDamageReduction_Lvl3;
            radiationSicknessHealReduction += StatusEffectSettings.radiationSicknessHealReduction_Lvl3;
            radiationSicknessDamage += StatusEffectSettings.radiationSicknessDamage_Lvl3;
            radiationSicknessMovementReduction += StatusEffectSettings.radiationSicknessMovementReduction_Lvl3;
            radiationSicknessDamageTaken += StatusEffectSettings.radiationSicknessDamageTaken_Lvl3;
        }
        else
        {
            return;
        }

        currentLevelRadiationSickness = level;

        UpdateCurrentStatus();

        statusEffectsEvent.CallStatusEffectsEvent(damageReduction, healReduction, damageOverTime, movementReduction, damageTaken);
    }

    public void DeactivateRadiationSickness(int level)
    {
        if (currentLevelRadiationSickness == 0) return;

        radiationSicknessDamageReduction = 0;
        radiationSicknessHealReduction = 0;
        radiationSicknessDamage = 0;
        radiationSicknessMovementReduction = 0;
        radiationSicknessDamageTaken = 0;

        currentLevelRadiationSickness = 0;

        UpdateCurrentStatus() ;

        statusEffectsEvent.CallStatusEffectsEvent(damageReduction, healReduction, damageOverTime, movementReduction, damageTaken);
    }
}
