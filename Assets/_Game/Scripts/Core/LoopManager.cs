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
        Debug.Log("Hết ngày. Đang lưu dữ liệu và Reset vòng lặp...");

        // 1. Lưu Affinity từ Scene hiện tại vào GameData (bất tử)
        if (AffinityManager.Instance != null && GameDataManager.Instance != null)
        {
            GameDataManager.Instance.PersistentAffinity = new Dictionary<string, int>(AffinityManager.Instance.NpcAffinity);
        }

        // 2. Load lại Scene hiện tại
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}