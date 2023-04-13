using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStatusEffectsControl : MonoBehaviour
{
    private StatusEffects statusEffects;

    private Coroutine radiationSicknessCoroutine;
    private int radiationSickenssLvl;
    private void Awake()
    {
        statusEffects = GetComponent<StatusEffects>();
    }

    public void ActivateRadiationSickness()
    {
        if (radiationSicknessCoroutine != null)
        {
            StopCoroutine(radiationSicknessCoroutine);

            StopRadiationSickess(radiationSickenssLvl);

            radiationSickenssLvl = 0;
        }

        radiationSicknessCoroutine = StartCoroutine(RadiationSicknessCoroutine());
    }
    private void StopRadiationSickess(int lvl)
    {
        if (lvl == 0) return;

        statusEffects.DeactivateRadiationSickness(lvl);

    }
    private IEnumerator RadiationSicknessCoroutine()
    {
        radiationSickenssLvl++;

        while (true)
        {

            statusEffects.ActivateRadiationSickness(radiationSickenssLvl);

            if (radiationSickenssLvl == 1)
            {
                yield return new WaitForSeconds(StatusEffectSettings.radiationSicknessDuration_Lvl1);
            }
            else if (radiationSickenssLvl == 2)
            {
                yield return new WaitForSeconds(StatusEffectSettings.radiationSicknessDuration_Lvl2);
            }
            else if (radiationSickenssLvl == 3)
            {
                yield return new WaitForSeconds(StatusEffectSettings.radiationSicknessDuration_Lvl3);
            }

            if (radiationSickenssLvl == 3) break;

            radiationSickenssLvl++;
        }
    }

    public void DeactivateRadiationSickness()
    {
        if (radiationSicknessCoroutine != null)
        {
            StopCoroutine(radiationSicknessCoroutine);

            StopRadiationSickess(radiationSickenssLvl);

            radiationSickenssLvl = 0;
        }
    }
}
