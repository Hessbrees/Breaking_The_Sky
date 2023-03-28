using UnityEngine;
using TMPro;

[DisallowMultipleComponent]
public class TimeClock : MonoBehaviour
{
    [SerializeField] private TimeChangeEvent timeChangeEvent;
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

        Debug.Log("time " + ShowTime(timeChangeArg.gameMinute, timeChangeArg.gameHour));
    }

    private void UpdateTimeClock(int gameMinutes, int gameHours)
    {
        time.text = ShowTime(gameMinutes, gameHours);
    }

    public string ShowTime(int gameMinute, int gameHour)
    {
        return gameHour.ToString("hh") + ":" + gameMinute.ToString("mm");
    }

    public string ShowDay(int gameDay)
    {
        return gameDay.ToString();
    }
}
