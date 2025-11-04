using UnityEngine;

public class NPCInteractable : MonoBehaviour, IInteractable
{
    [SerializeField] private DialogueData dialogueToStart;
    [SerializeField] private string interactText = "Nói chuyện";

    // Hàm từ Interface
    public void Interact(Transform interactorTransform)
    {
        // Khi Player bấm 'E', gọi hệ thống hội thoại
        DialogueManager.Instance.StartDialogue(dialogueToStart);
    }

    // Hàm từ Interface
    public string GetInteractText()
    {
        return interactText;
    }
}