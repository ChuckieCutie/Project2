using System.Collections.Generic;
using UnityEngine;

public class GameDataManager : MonoBehaviour
{
    public static GameDataManager Instance { get; private set; }

    // Dữ liệu "bất tử"
    public Dictionary<string, int> persistentAffinity;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject); // Hủy bản sao
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Bất tử!
            
            persistentAffinity = new Dictionary<string, int>();
        }
    }
}