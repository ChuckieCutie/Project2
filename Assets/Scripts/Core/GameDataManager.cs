using System.Collections.Generic;
using UnityEngine;

public class GameDataManager : MonoBehaviour
{
    public static GameDataManager Instance { get; private set; }

    // Dữ liệu cần lưu qua các vòng lặp (Scene Load)
    public Dictionary<string, int> PersistentAffinity { get; set; } = new Dictionary<string, int>();
    public List<string> CollectedShards { get; private set; } = new List<string>();
    
    public int EmpathyScore { get; private set; } = 0;
    public int ReasonScore { get; private set; } = 0;

    void Awake()
    {
        if (Instance != null && Instance != this) Destroy(gameObject);
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    public void ModifyScore(int empathyDelta, int reasonDelta)
    {
        EmpathyScore += empathyDelta;
        ReasonScore += reasonDelta;
        Debug.Log($"Stats Updated -> E: {EmpathyScore} | X: {ReasonScore}");
    }

    public void UnlockShard(string npcName)
    {
        if (!CollectedShards.Contains(npcName))
        {
            CollectedShards.Add(npcName);
            Debug.Log($"ĐÃ MỞ KHÓA KÝ ỨC: {npcName}");
            // TODO: Bắn Event UI hiển thị popup nhận ký ức
        }
    }
}