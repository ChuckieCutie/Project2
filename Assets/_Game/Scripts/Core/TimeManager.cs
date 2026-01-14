using UnityEngine;
using System;

public class TimeManager : MonoBehaviour
{
    public static TimeManager Instance { get; private set; }

    // Dùng Tiếng Anh chuẩn
    public enum TimeOfDay { Morning, Afternoon, Evening }
    public TimeOfDay CurrentTime { get; private set; } = TimeOfDay.Morning;

    public event Action<TimeOfDay> OnTimeChanged;

    [Header("Settings")]
    [SerializeField] private float _durationPerPhase = 10f; // 10s đổi buổi 1 lần
    private float _timer;

    void Awake()
    {
        if (Instance != null && Instance != this) Destroy(gameObject);
        else Instance = this;
    }

    void Start()
    {
        _timer = _durationPerPhase;
        OnTimeChanged?.Invoke(CurrentTime);
    }

    void Update()
    {
        _timer -= Time.deltaTime;
        
        // Tự động chuyển giờ
        if (_timer <= 0)
        {
            AdvanceTime();
            _timer = _durationPerPhase;
        }

        // Hoặc bấm T để test
        if (Input.GetKeyDown(KeyCode.T))
        {
            AdvanceTime();
        }
    }

    public void AdvanceTime()
    {
        switch (CurrentTime)
        {
            case TimeOfDay.Morning:
                ChangeTime(TimeOfDay.Afternoon);
                break;
            case TimeOfDay.Afternoon:
                ChangeTime(TimeOfDay.Evening);
                break;
            case TimeOfDay.Evening:
                if (LoopManager.Instance != null) LoopManager.Instance.EndDay();
                break;
        }
    }

    private void ChangeTime(TimeOfDay newTime)
    {
        CurrentTime = newTime;
        Debug.Log($"Time Changed: {CurrentTime}");
        OnTimeChanged?.Invoke(CurrentTime);
    }
    public void StartNextLoop()
    {
        if (TimeManager.Instance != null)
        {
            TimeManager.Instance.CurrentTime = TimeManager.TimeOfDay.Morning;
            Debug.Log("Đã quay ngược thời gian về Buổi Sáng!");
        }
    }
}