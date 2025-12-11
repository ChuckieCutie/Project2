using System.Collections.Generic;

[System.Serializable]
public class DialogueGraph
{
    public string npc; // Tên NPC (VD: "Lam")
    public List<DialogueNode> nodes;
}

[System.Serializable]
public class DialogueNode
{
    public string id;
    public string type; // "dialogue", "task", "memory_shard", "gate"
    public int priority;
    public bool once; // Chỉ chạy 1 lần
    
    public List<Condition> conditions; 
    
    // JSON của bạn dùng nhiều key cho thoại, ta khai báo hết để hứng dữ liệu
    public List<string> npc_lines; 
    public List<string> memory_lines;
    public List<string> task_lines;
    public List<string> hint_lines;
    public List<string> system_lines;

    public List<ChoiceJSON> choices;
    public List<EffectJSON> effects; // Effect khi kết thúc node (nếu ko có choice)
}

[System.Serializable]
public class Condition
{
    public string var; // "timeOfDay", "affinity.Lam", "E", "X"
    public string op;  // "==", ">", ">="
    public string value; 
}

[System.Serializable]
public class ChoiceJSON
{
    public string id;
    public string label;
    public List<EffectJSON> effects;
    public List<string> npc_after;
}

[System.Serializable]
public class EffectJSON
{
    public string op; // "inc", "set"
    public string var; // "E", "X"
    public string value; 
}