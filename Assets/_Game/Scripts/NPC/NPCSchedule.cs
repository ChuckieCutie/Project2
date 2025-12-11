using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class NPCSchedule : MonoBehaviour
{
    private NavMeshAgent agent;

    [Header("Schedule Locations")]
    [SerializeField] private Transform locationMorning;
    [SerializeField] private Transform locationAfternoon;
    [SerializeField] private Transform locationEvening;

    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    void Start()
    {
        if (TimeManager.Instance != null)
        {
            MoveToLocation(TimeManager.Instance.CurrentTime);
        }
    }

    void OnEnable()
    {
        if (TimeManager.Instance != null)
            TimeManager.Instance.OnTimeChanged += MoveToLocation;
    }

    void OnDisable()
    {
        if (TimeManager.Instance != null)
            TimeManager.Instance.OnTimeChanged -= MoveToLocation;
    }

    private void MoveToLocation(TimeManager.TimeOfDay time)
    {
        Transform destination = null;

        switch (time)
        {
            case TimeManager.TimeOfDay.Morning:
                destination = locationMorning;
                break;
            case TimeManager.TimeOfDay.Afternoon:
                destination = locationAfternoon;
                break;
            case TimeManager.TimeOfDay.Evening:
                destination = locationEvening;
                break;
        }

        if (destination != null)
        {
            agent.SetDestination(destination.position);
        }
    }
}