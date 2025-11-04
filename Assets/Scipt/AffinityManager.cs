using System.Collections.Generic;
using UnityEngine;

public class AffinityManager : MonoBehaviour
{
    public static AffinityManager Instance { get; private set; }

    public Dictionary<string, int> npcAffinity;
    private string[] allNpcNames = { "Bà Hường", "Anh Tuấn", "Cô Hoa", "Bé Nam" }; // Từ GDD [cite: 48, 52, 56, 60]

    void Awake()
    {
        if (Instance != null && Instance != this) Destroy(gameObject);
        else Instance = this;
    }

    void Start()
    {
        // TẢI dữ liệu từ GameDataManager khi bắt đầu vòng lặp mới
        LoadAffinity();
    }

    void LoadAffinity()
    {
        if (GameDataManager.Instance.persistentAffinity != null &&
            GameDataManager.Instance.persistentAffinity.Count > 0)
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
        npcAffinity[npcName] = Mathf.Clamp(oldAffinity + amount, 0, 100); // GDD yêu cầu 0-100 [cite: 26]
        Debug.Log($"Affinity của {npcName} đổi từ {oldAffinity} -> {npcAffinity[npcName]}");
    }
}