using UnityEngine;
using System; // Để dùng Action

public class TimeManager : MonoBehaviour
{
    public static TimeManager Instance { get; private set; }

    public enum TimeOfDay { Sang, Chieu, Toi }
    public TimeOfDay CurrentTime { get; private set; } = TimeOfDay.Sang;

    // Sự kiện để NPC lắng nghe
    public event Action<TimeOfDay> OnTimeChanged;

    void Awake()
    {
        if (Instance != null && Instance != this) Destroy(gameObject);
        else Instance = this;
    }

    void Update()
    {
        // Debug: Bấm T để tua nhanh thời gian
        if (Input.GetKeyDown(KeyCode.T))
        {
            AdvanceTime();
        }
    }

    public void AdvanceTime()
    {
        switch (CurrentTime)
        {
            case TimeOfDay.Sang:
                ChangeTime(TimeOfDay.Chieu);
                break;
            case TimeOfDay.Chieu:
                ChangeTime(TimeOfDay.Toi);
                break;
            case TimeOfDay.Toi:
                // Hết ngày -> Gọi LoopManager xử lý reset
                LoopManager.Instance.EndDay();
                break;
        }
    }

    private void ChangeTime(TimeOfDay newTime)
    {
        CurrentTime = newTime;
        Debug.Log($"Thời gian chuyển sang: {CurrentTime}");
        // Bắn pháo hiệu cho tất cả NPC biết
        OnTimeChanged?.Invoke(CurrentTime);
    }
}