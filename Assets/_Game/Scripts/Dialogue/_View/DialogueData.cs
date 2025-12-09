using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(fileName = "New Dialogue", menuName = "Game/Dialogue Data")] // Dòng này tạo menu chuột phải
public class DialogueData : ScriptableObject
{
    public string npcName;
    [TextArea(3, 10)]
    public string[] sentences;
    public Choice[] choices;
}

[System.Serializable]
public class Choice
{
    public string choiceText;
    public DialogueEffect effectType; // Loại tác động
    public int effectValue; // Giá trị cộng thêm (ví dụ: 10 điểm)
    public DialogueData nextDialogue; // (Tùy chọn) Hội thoại tiếp theo nếu có
}

// Danh sách các loại tác động có thể xảy ra
public enum DialogueEffect
{
    None,
    AddEmpathy,   // Tăng E
    AddReason,    // Tăng X
    AddAffinity   // Tăng Quan hệ
}