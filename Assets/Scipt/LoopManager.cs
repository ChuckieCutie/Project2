using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class LoopManager : MonoBehaviour
{
    public static LoopManager Instance { get; private set; }

    void Awake()
    {
        if (Instance != null && Instance != this) Destroy(gameObject);
        else Instance = this;
    }

    public void EndDay()
    {
        Debug.Log("Hết ngày. Đang LƯU dữ liệu...");
        
        // LƯU Affinity hiện tại vào DataManager "bất tử"
        GameDataManager.Instance.persistentAffinity = 
            new Dictionary<string, int>(AffinityManager.Instance.npcAffinity);

        Debug.Log("Reset vòng lặp!");
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}