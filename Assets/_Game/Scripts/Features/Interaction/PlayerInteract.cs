using UnityEngine;

public class PlayerInteract : MonoBehaviour
{
    [SerializeField] private float interactRange = 3f;
    // (Tùy chọn) [SerializeField] private TextMeshProUGUI interactTextUI; 

    private IInteractable closestInteractable;

    void Update()
    {
        FindClosestInteractable();
        
        if (Input.GetKeyDown(KeyCode.E) && closestInteractable != null)
        {
            closestInteractable.Interact(transform);
        }
    }

    void FindClosestInteractable()
    {
        closestInteractable = null;
        float closestDistance = float.MaxValue;

        Collider[] colliders = Physics.OverlapSphere(transform.position, interactRange);

        foreach (Collider collider in colliders)
        {
            if (collider.TryGetComponent(out IInteractable interactable))
            {
                float distance = Vector3.Distance(transform.position, collider.transform.position);
                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    closestInteractable = interactable;
                }
            }
        }
        
        // (Bạn có thể code thêm để cập nhật UI "Bấm E" ở đây)
    }
}