using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Zenject;

public class PlayerHealthUI : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI currentHealthText;
    [SerializeField] TextMeshProUGUI defenceText;

    private Color healthLowColor = new Color32(0,255,0,255);
    private Color healthHighColor = new Color32(255,0,0,255);

    private Color defenceLowColor = new Color32(255, 255, 255, 255);
    private Color defenceHighColor = new Color32(0, 0, 255, 255);

    [Inject(Id = "Player")]
    private Player player;

    private void Awake()
    {
        player.InitAllComponents();
    }

    private void OnEnable()
    {
        player.healthEvent.OnHealth += UpdateUI_OnHealth;
    }
    private void OnDisable()
    {
        player.healthEvent.OnHealth -= UpdateUI_OnHealth;
    }
    private void UpdateUI_OnHealth(HealthEvent healthEvent, HealthArg healthArg)
    {
        UpdateHealthValue(healthArg.currentHealth);

        UpdateHealthColor(healthArg.percentHealth);

        UpdateDefenceValue(healthArg.defence);

        UpdateDefenceColor(healthArg.defence);

    }

    private void UpdateHealthValue(int currentHealth)
    {
        currentHealthText.text = currentHealth.ToString();
    }

    private void UpdateHealthColor(float healthInPercent)
    {
        currentHealthText.color = Color.Lerp(healthHighColor, healthLowColor, healthInPercent);
    }

    private void UpdateDefenceValue(int defence)
    {
        defenceText.text = defence.ToString();
    }

    private void UpdateDefenceColor(int defence)
    {
        if(defence > Settings.maxDefenceColorValue)
        {
            defenceText.color = defenceHighColor;

        }
        else
        {
        defenceText.color = Color.Lerp(defenceLowColor, defenceHighColor,  (float)defence/(float)Settings.maxDefenceColorValue);
        }
    }
    

}
