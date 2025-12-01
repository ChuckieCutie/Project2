using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class NPCSchedule : MonoBehaviour
{
    private NavMeshAgent agent;

    [Header("Lịch trình di chuyển")]
    [SerializeField] private Transform viTriSang;
    [SerializeField] private Transform viTriChieu;
    [SerializeField] private Transform viTriToi;

    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    void Start()
    {
        // Khi game bắt đầu, di chuyển ngay tới vị trí theo giờ hiện tại
        if (TimeManager.Instance != null)
        {
            MoveToLocation(TimeManager.Instance.CurrentTime);
        }
    }

    void OnEnable()
    {
        // Đăng ký nhận thông báo thay đổi giờ
        if (TimeManager.Instance != null)
            TimeManager.Instance.OnTimeChanged += MoveToLocation;
    }

    void OnDisable()
    {
        // Hủy đăng ký để tránh lỗi
        if (TimeManager.Instance != null)
            TimeManager.Instance.OnTimeChanged -= MoveToLocation;
    }

    // Hàm này tự động chạy khi giờ thay đổi
    private void MoveToLocation(TimeManager.TimeOfDay time)
    {
        Transform destination = null;

        switch (time)
        {
            case TimeManager.TimeOfDay.Sang:
                destination = viTriSang;
                break;
            case TimeManager.TimeOfDay.Chieu:
                destination = viTriChieu;
                break;
            case TimeManager.TimeOfDay.Toi:
                destination = viTriToi;
                break;
        }

        if (destination != null)
        {
            agent.SetDestination(destination.position);
            Debug.Log($"{gameObject.name} đang đi tới {destination.name}");
        }
    }
}