using System.Collections.Generic;
using UnityEngine;

public class GameDataManager : MonoBehaviour
{
    public static GameDataManager Instance { get; private set; }

    // Dữ liệu Affinity (Lưu điểm số 0-100)
    public Dictionary<string, int> persistentAffinity = new Dictionary<string, int>();

    // Dữ liệu cốt lõi: Điểm Cảm xúc (E) và Lý trí (X)
    public int empathyScore = 0; 
    public int reasonScore = 0;  
    
    // Danh sách Memory Shard đã nhặt
    public List<string> collectedShards = new List<string>(); 

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Giữ nguyên qua các Scene
        }
    }

    // Hàm tiện ích để cộng điểm (Dùng cho Button trong UI)
    public void AddEmpathy(int amount) 
    { 
        empathyScore += amount; 
        Debug.Log($"E Score: {empathyScore}"); 
    }
    
    public void AddReason(int amount) 
    { 
        reasonScore += amount; 
        Debug.Log($"X Score: {reasonScore}"); 
    }
}