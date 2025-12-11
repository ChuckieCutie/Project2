using UnityEngine;

public class NPCInteractable : MonoBehaviour, IInteractable
{
    // Không dùng DialogueData cứng nữa, chỉ cần tên NPC khớp với file JSON
    [SerializeField] private string _npcID; // VD: nhập "Lam", "Hoa", "Son"
    [SerializeField] private string _interactText = "Nói chuyện";

    public void Interact(Transform interactorTransform)
    {
        // 1. Hỏi Manager: "Ông Lam giờ nói câu gì?"
        var bestNode = DialogueManager.Instance.GetValidDialogue(_npcID);

        if (bestNode != null)
        {
            // 2. Có hội thoại -> Hiển thị
            DialoguePresenter.Instance.StartDialogueFromNode(bestNode, _npcID);
        }
        else
        {
            // 3. Không có gì để nói (hết thoại hoặc chưa đủ điều kiện)
            Debug.Log($"{_npcID} không có gì để nói lúc này.");
            // Có thể hiển thị một câu mặc định kiểu "..."
        }
    }

    public string GetInteractText()
    {
        return _interactText;
    }
}