using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager Instance { get; private set; }

    [Header("CẤU HÌNH FILE (QUAN TRỌNG)")]
    [Tooltip("Kéo thả toàn bộ file từ folder _Resources/Dialogues vào đây")]
    public List<TextAsset> dialogueFilesInput; 

    // Database lưu dữ liệu sau khi load
    private Dictionary<string, List<DialogueNode>> _dialogueDatabase = new Dictionary<string, List<DialogueNode>>();
    
    // Lưu các node đã từng hội thoại (để check one-time dialogue)
    private HashSet<string> _playedNodes = new HashSet<string>();

    void Awake()
    {
        if (Instance != null && Instance != this) 
        {
            Destroy(this);
        }
        else 
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            LoadAllDialogues();
        }
    }

    void LoadAllDialogues()
    {
        // Kiểm tra xem đã kéo file vào chưa
        if (dialogueFilesInput == null || dialogueFilesInput.Count == 0)
        {
            Debug.LogError("DialogueManager: Chưa có file nào trong list 'Dialogue Files Input'! Hãy kéo file từ _Resources vào Inspector.");
            return;
        }

        foreach (var file in dialogueFilesInput)
        {
            if (file == null) continue;

            try {
                // Parse JSON từ text của file đã kéo vào
                var graph = JsonConvert.DeserializeObject<DialogueGraph>(file.text);
                
                if (graph != null && !string.IsNullOrEmpty(graph.npc))
                {
                    if (!_dialogueDatabase.ContainsKey(graph.npc))
                    {
                        _dialogueDatabase.Add(graph.npc, graph.nodes);
                    }
                    else 
                    {
                        Debug.LogWarning($"Duplicate NPC ID found: {graph.npc}. Skipping file {file.name}.");
                    }
                }
            }
            catch (System.Exception e) {
                Debug.LogError($"Lỗi khi đọc file {file.name}: {e.Message}");
            }
        }
        
        Debug.Log($"DialogueManager: Đã load thành công {_dialogueDatabase.Count} NPC từ {dialogueFilesInput.Count} file.");
    }

    public DialogueNode GetValidDialogue(string npcName)
    {
        if (!_dialogueDatabase.ContainsKey(npcName)) return null;

        var potentialNodes = _dialogueDatabase[npcName];

        // Lọc các node thỏa mãn điều kiện
        var validNodes = potentialNodes.Where(node => 
        {
            // Kiểm tra nếu node chỉ được chạy 1 lần
            if (node.once && _playedNodes.Contains(node.id)) return false;
            
            // Kiểm tra các điều kiện logic (biến số, thời gian...)
            return CheckConditions(node.conditions);
        }).ToList();

        // Sắp xếp theo độ ưu tiên (Priority): Cao -> Thấp
        validNodes.Sort((a, b) => b.priority.CompareTo(a.priority));

        // Lấy node đầu tiên (có priority cao nhất thỏa mãn điều kiện)
        return validNodes.FirstOrDefault();
    }

    public void MarkNodeAsPlayed(string nodeId)
    {
        if (!_playedNodes.Contains(nodeId)) _playedNodes.Add(nodeId);
    }

    private bool CheckConditions(List<Condition> conditions)
    {
        if (conditions == null || conditions.Count == 0) return true;

        foreach (var cond in conditions)
        {
            string currentValStr = GetValueFromGameData(cond.var);
            if (!Compare(currentValStr, cond.op, cond.value)) return false;
        }
        return true;
    }

    private string GetValueFromGameData(string varName)
    {
        // 1. Mapping Thời gian 
        if (varName == "timeOfDay") 
        {
            if (TimeManager.Instance != null)
            {
                var t = TimeManager.Instance.CurrentTime;
                
                if (t == TimeManager.TimeOfDay.Morning) return "morning";
                if (t == TimeManager.TimeOfDay.Afternoon) return "noon"; 
                if (t == TimeManager.TimeOfDay.Evening) return "evening";
            }
            return "morning"; // Default fallback
        }

        // 2. Stats (Chỉ số E và X)
        if (GameDataManager.Instance != null)
        {
            if (varName == "E") return GameDataManager.Instance.EmpathyScore.ToString();
            if (varName == "X") return GameDataManager.Instance.ReasonScore.ToString();

            // 3. Affinity (Độ thiện cảm)
            if (varName.StartsWith("affinity."))
            {
                string npc = varName.Split('.')[1];
                if (GameDataManager.Instance.PersistentAffinity.ContainsKey(npc))
                    return GameDataManager.Instance.PersistentAffinity[npc].ToString();
                return "0";
            }
            
            // 4. Flags (Cờ sự kiện, ví dụ: mảnh shard)
            if (varName.StartsWith("flags."))
            {
                string flagName = varName.Split('.')[1];
                if (flagName.Contains("shard"))
                {
                     string npcShardName = flagName.Replace("_shard", "");
                     bool hasShard = GameDataManager.Instance.CollectedShards.Exists(x => x.Equals(npcShardName, System.StringComparison.OrdinalIgnoreCase));
                     return hasShard.ToString().ToLower(); 
                }
            }
        }

        return "0";
    }

    private bool Compare(string current, string op, string target)
    {
        // So sánh số (cho E, X, Affinity)
        if (int.TryParse(current, out int curNum) && int.TryParse(target, out int tarNum))
        {
            switch (op)
            {
                case "==": return curNum == tarNum;
                case ">": return curNum > tarNum;
                case ">=": return curNum >= tarNum;
                case "<": return curNum < tarNum;
                case "<=": return curNum <= tarNum;
            }
        }
        // So sánh chuỗi (cho timeOfDay)
        return current.Trim().ToLower() == target.Trim().ToLower();
    }
}