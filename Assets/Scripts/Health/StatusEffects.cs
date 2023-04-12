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
    private void Awake()
    {
        statusEffectsEvent = GetComponent<StatusEffectsEvent>();
    }

    public void ActivateRadiationSickness(int level)
    {
        switch (level)
        {
            case 1:
                {
                    damageReduction += StatusEffectSettings.radiationSicknessDamageReduction_Lvl1;
                    healReduction += StatusEffectSettings.radiationSicknessHealReduction_Lvl1;
                    damageOverTime += StatusEffectSettings.radiationSicknessDamage_Lvl1;
                    movementReduction += StatusEffectSettings.radiationSicknessMovementReduction_Lvl1;
                    damageTaken += StatusEffectSettings.radiationSicknessDamageTaken_Lvl1;
                }
                break;
            case 2:
                {
                    damageReduction += StatusEffectSettings.radiationSicknessDamageReduction_Lvl2;
                    healReduction += StatusEffectSettings.radiationSicknessHealReduction_Lvl2;
                    damageOverTime += StatusEffectSettings.radiationSicknessDamage_Lvl2;
                    movementReduction += StatusEffectSettings.radiationSicknessMovementReduction_Lvl2;
                    damageTaken += StatusEffectSettings.radiationSicknessDamageTaken_Lvl2;
                }
                break;
            case 3:
                {
                    damageReduction += StatusEffectSettings.radiationSicknessDamageReduction_Lvl3;
                    healReduction += StatusEffectSettings.radiationSicknessHealReduction_Lvl3;
                    damageOverTime += StatusEffectSettings.radiationSicknessDamage_Lvl3;
                    movementReduction += StatusEffectSettings.radiationSicknessMovementReduction_Lvl3;
                    damageTaken += StatusEffectSettings.radiationSicknessDamageTaken_Lvl3;
                }
                break;
        }

        statusEffectsEvent.CallStatusEffectsEvent(damageReduction,healReduction,damageOverTime,movementReduction,damageTaken);
    }

    public void DeactivateRadiationSickness(int level)
    {
        if(level >= 1)
        {
            damageReduction -= StatusEffectSettings.radiationSicknessDamageReduction_Lvl1;
            healReduction -= StatusEffectSettings.radiationSicknessHealReduction_Lvl1;
            damageOverTime -= StatusEffectSettings.radiationSicknessDamage_Lvl1;
            movementReduction -= StatusEffectSettings.radiationSicknessMovementReduction_Lvl1;
            damageTaken -= StatusEffectSettings.radiationSicknessDamageTaken_Lvl1;
        }

        if(level >= 2)
        {
            damageReduction -= StatusEffectSettings.radiationSicknessDamageReduction_Lvl2;
            healReduction -= StatusEffectSettings.radiationSicknessHealReduction_Lvl2;
            damageOverTime -= StatusEffectSettings.radiationSicknessDamage_Lvl2;
            movementReduction -= StatusEffectSettings.radiationSicknessMovementReduction_Lvl2;
            damageTaken -= StatusEffectSettings.radiationSicknessDamageTaken_Lvl2;
        }

        if (level >= 3)
        {
            damageReduction -= StatusEffectSettings.radiationSicknessDamageReduction_Lvl3;
            healReduction -= StatusEffectSettings.radiationSicknessHealReduction_Lvl3;
            damageOverTime -= StatusEffectSettings.radiationSicknessDamage_Lvl3;
            movementReduction -= StatusEffectSettings.radiationSicknessMovementReduction_Lvl3;
            damageTaken -= StatusEffectSettings.radiationSicknessDamageTaken_Lvl3;
        }

        statusEffectsEvent.CallStatusEffectsEvent(damageReduction, healReduction, damageOverTime, movementReduction,damageTaken);
    }
}
