using UnityEngine;

// Tạo menu để chuột phải -> Create -> Game -> NPC Profile
[CreateAssetMenu(fileName = "New NPC Profile", menuName = "Game/NPC Profile")]
public class NPCProfile : ScriptableObject
{
    public string npcName; // Tên hiển thị (Ví dụ: "Bác Bảo vệ")
    public string id;      // ID dùng cho logic (Ví dụ: "npc_security_01")
    public Sprite portrait; // Ảnh đại diện (nếu cần)
}