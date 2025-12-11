using System.Collections.Generic;
using UnityEngine;

public class AffinityManager : MonoBehaviour
{
    public static AffinityManager Instance { get; private set; }

    public Dictionary<string, int> NpcAffinity { get; private set; } = new Dictionary<string, int>();

    // SỬA: Dùng ID tiếng Anh chuẩn (Trùng với JSON và tên NPCInteractable)
    private string[] allNpcNames = { "Lam", "Hoa", "Son", "BeHa" };

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
            NpcAffinity = new Dictionary<string, int>(GameDataManager.Instance.PersistentAffinity);
            Debug.Log("Loaded Affinity Data.");
        }
        else
        {
            foreach (var name in allNpcNames)
            {
                if (!NpcAffinity.ContainsKey(name)) NpcAffinity.Add(name, 0);
            }
            Debug.Log("Initialized New Affinity Data.");
        }
    }

    public void ChangeAffinity(string npcName, int amount)
    {
        if (!NpcAffinity.ContainsKey(npcName)) 
            NpcAffinity.Add(npcName, 0);

        int oldVal = NpcAffinity[npcName];
        NpcAffinity[npcName] = Mathf.Clamp(oldVal + amount, 0, 100);

        Debug.Log($"Affinity {npcName}: {oldVal} -> {NpcAffinity[npcName]}");
    }
}