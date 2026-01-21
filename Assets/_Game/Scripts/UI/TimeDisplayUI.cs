using UnityEngine;
using TMPro;

public class TimeDisplayUI : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private TextMeshProUGUI _timeText;

    private void Start()
    {
        if (TimeManager.Instance != null)
        {
            UpdateClock(TimeManager.Instance.CurrentTime);
            TimeManager.Instance.OnTimeChanged += UpdateClock;
        }
    }

    private void OnDestroy()
    {
        if (TimeManager.Instance != null)
        {
            TimeManager.Instance.OnTimeChanged -= UpdateClock;
        }
    }

    private void UpdateClock(TimeManager.TimeOfDay time)
    {
        switch (time)
        {
            case TimeManager.TimeOfDay.Morning:
                _timeText.text = "Sáng"; 
                _timeText.color = Color.yellow;
                break;
            case TimeManager.TimeOfDay.Afternoon: 
                _timeText.text = "Chiều";
                _timeText.color = new Color(1f, 0.5f, 0f);
                break;
            case TimeManager.TimeOfDay.Evening:
                _timeText.text = "Tối";
                _timeText.color = new Color(0.2f, 0.2f, 0.8f);
                break;
        }
    }
}