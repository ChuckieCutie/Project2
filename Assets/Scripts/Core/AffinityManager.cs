using System.Collections.Generic;
using UnityEngine;

public class AffinityManager : MonoBehaviour
{
    public static AffinityManager Instance { get; private set; }

    public Dictionary<string, int> npcAffinity;
    
    // Tên NPC chuẩn theo GDD
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
        // Lấy dữ liệu từ GameDataManager (bất tử) đổ vào đây
        if (GameDataManager.Instance.persistentAffinity != null && GameDataManager.Instance.persistentAffinity.Count > 0)
        {
            npcAffinity = new Dictionary<string, int>(GameDataManager.Instance.persistentAffinity);
            Debug.Log("Affinity đã TẢI từ vòng lặp trước.");
        }
        else
        {
            npcAffinity = new Dictionary<string, int>();
            foreach (string name in allNpcNames)
            {
                if (!npcAffinity.ContainsKey(name)) npcAffinity.Add(name, 0);
            }
            Debug.Log("Khởi tạo Affinity mới.");
        }
    }

    public void ChangeAffinity(string npcName, int amount)
    {
        if (!npcAffinity.ContainsKey(npcName)) return;

        int oldAffinity = npcAffinity[npcName];
        // Cộng điểm và kẹp trong khoảng 0-100
        npcAffinity[npcName] = Mathf.Clamp(oldAffinity + amount, 0, 100); 
        
        Debug.Log($"Affinity của {npcName} đổi từ {oldAffinity} -> {npcAffinity[npcName]}");

        // Kiểm tra xem có lên cấp để mở khóa ký ức không
        CheckLevelUp(npcName);
    }

    void CheckLevelUp(string npcName)
    {
        int score = npcAffinity[npcName];
        // Quy đổi level: >60 điểm là Level 2 (Mở khóa Memory Shard)
        if (score >= 60 && !GameDataManager.Instance.collectedShards.Contains(npcName))
        {
            Debug.Log($"MỞ KHÓA KÍ ỨC (Memory Shard) của: {npcName}");
            GameDataManager.Instance.collectedShards.Add(npcName);
            // TODO: Thêm code hiển thị thông báo UI tại đây
        }
    }
}