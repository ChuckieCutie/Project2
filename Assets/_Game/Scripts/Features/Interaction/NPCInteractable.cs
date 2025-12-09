using UnityEngine;

public class NPCInteractable : MonoBehaviour, IInteractable
{
    [SerializeField] private DialogueData _dialogueData; // Đổi tên biến cho đúng chuẩn
    [SerializeField] private string _interactText = "Nói chuyện";

    public void Interact(Transform interactorTransform)
    {
        // Gọi đến Presenter mới
        if (DialoguePresenter.Instance != null)
        {
            DialoguePresenter.Instance.StartDialogue(_dialogueData);
        }
        else
        {
            Debug.LogError("Thiếu DialoguePresenter trong Scene!");
        }
    }

    public string GetInteractText()
    {
        return _interactText;
    }
}