using System.Collections.Generic;
using UnityEngine;

public class AffinityManager : MonoBehaviour
{
    public static AffinityManager Instance { get; private set; }

    // Dữ liệu quan hệ trong phiên chơi này (Tên NPC -> Điểm)
    public Dictionary<string, int> NpcAffinity { get; private set; } = new Dictionary<string, int>();

    // Danh sách tên NPC để khởi tạo (tránh lỗi Null)
    private string[] allNpcNames = { "Ông Lâm", "Cô Hoa", "Anh Sơn", "Bé Hà" };

    void Awake()
    {
        if (Instance != null && Instance != this) Destroy(gameObject);
        else Instance = this;
    }

    void Start()
    {
        LoadAffinity();
    }

    void LoadAffinity()
    {
        if (GameDataManager.Instance != null && GameDataManager.Instance.PersistentAffinity.Count > 0)
        {
            // Copy dữ liệu từ GameData sang
            NpcAffinity = new Dictionary<string, int>(GameDataManager.Instance.PersistentAffinity);
            Debug.Log("Đã tải Affinity từ vòng lặp trước.");
        }
        else
        {
            // Khởi tạo mới nếu chưa có dữ liệu
            foreach (var name in allNpcNames)
            {
                if (!NpcAffinity.ContainsKey(name)) NpcAffinity.Add(name, 0);
            }
            Debug.Log("Khởi tạo Affinity mới.");
        }
    }

    public void ChangeAffinity(string npcName, int amount)
    {
        if (!NpcAffinity.ContainsKey(npcName)) return;

        int oldVal = NpcAffinity[npcName];
        NpcAffinity[npcName] = Mathf.Clamp(oldVal + amount, 0, 100);

        Debug.Log($"Affinity {npcName}: {oldVal} -> {NpcAffinity[npcName]}");

        // Logic mở khóa ký ức (Level Up)
        if (NpcAffinity[npcName] >= 60)
        {
            GameDataManager.Instance.UnlockShard(npcName);
        }
    }
}