using UnityEngine;
using TMPro;
using Zenject;

[DisallowMultipleComponent]
public class TimeClock : MonoBehaviour
{
    [Inject(Id ="TimeManager")]
    private TimeChangeEvent timeChangeEvent;

    [SerializeField] private TextMeshProUGUI time;

    private void OnEnable()
    {
        timeChangeEvent.OnTimeChange += UpdateClock_OnTimeChange;
    }
    private void OnDisable()
    {
        timeChangeEvent.OnTimeChange -= UpdateClock_OnTimeChange;
    }

    private void UpdateClock_OnTimeChange(TimeChangeEvent timeChangeEvent,TimeChangeArg timeChangeArg)
    {
        UpdateTimeClock(timeChangeArg.gameMinute, timeChangeArg.gameHour);
    }

    private void UpdateTimeClock(int gameMinutes, int gameHours)
    {
        time.text = ShowTime(gameMinutes, gameHours);
    }

    public string ShowTime(int gameMinute, int gameHour)
    {
        return gameHour.ToString("D2") + ":" + gameMinute.ToString("D2");
    }

    public string ShowDay(int gameDay)
    {
        return gameDay.ToString();
    }
}
