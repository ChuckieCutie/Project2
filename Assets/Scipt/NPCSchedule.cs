using UnityEngine;
using UnityEngine.AI; // Cần cho NavMeshAgent

[RequireComponent(typeof(NavMeshAgent))]
public class NPCSchedule : MonoBehaviour
{
    private NavMeshAgent agent;
    
    [Header("Kéo vị trí vào đây")]
    [SerializeField] private Transform viTriSang; 
    [SerializeField] private Transform viTriChieu; 
    [SerializeField] private Transform viTriToi; 

    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    void OnEnable()
    {
        // Đăng ký "lắng nghe" sự kiện từ TimeManager
        TimeManager.Instance.OnTimeChanged += HandleTimeChange;
    }

    void OnDisable()
    {
        // Hủy đăng ký (quan trọng)
        if (TimeManager.Instance != null)
        {
            TimeManager.Instance.OnTimeChanged -= HandleTimeChange;
        }
    }

    // Hàm này được TimeManager tự động gọi
    private void HandleTimeChange(TimeManager.TimeOfDay newTime)
    {
        Transform destination = null;
        switch (newTime)
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
        }
    }
}