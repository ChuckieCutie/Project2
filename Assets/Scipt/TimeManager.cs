using UnityEngine;
using System; 

public class TimeManager : MonoBehaviour
{
    public static TimeManager Instance { get; private set; }

    public enum TimeOfDay { Sang, Chieu, Toi }
    public TimeOfDay CurrentTime { get; private set; } = TimeOfDay.Sang;

    public event Action<TimeOfDay> OnTimeChanged;

    void Awake()
    {
        if (Instance != null && Instance != this) Destroy(gameObject);
        else Instance = this;
    }

    // Hàm này để tua nhanh thời gian (dùng để test)
    public void AdvanceTime()
    {
        if (CurrentTime == TimeOfDay.Sang) CurrentTime = TimeOfDay.Chieu;
        else if (CurrentTime == TimeOfDay.Chieu) CurrentTime = TimeOfDay.Toi;
        else if (CurrentTime == TimeOfDay.Toi)
        {
            // Hết ngày! Gọi LoopManager để reset
            LoopManager.Instance.EndDay();
            return; 
        }

        Debug.Log("Thời gian chuyển sang: " + CurrentTime);
        OnTimeChanged?.Invoke(CurrentTime);
    }

    // DEBUG: Dùng phím 'T' để test
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            AdvanceTime();
        }
    }
}