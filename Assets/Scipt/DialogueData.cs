using UnityEngine;
using UnityEngine.Events; // Cần cho UnityEvent

// Class này không phải là ScriptableObject, nó là 1 phần của DialogueData
[System.Serializable]
public class ChoiceData
{
    public string choiceText;
    public UnityEvent OnChoiceSelected; // Kéo thả hàm vào đây
}

// Đây là ScriptableObject
[CreateAssetMenu(fileName = "New Dialogue", menuName = "Game/Dialogue")]
public class DialogueData : ScriptableObject
{
    public string npcName;
    
    [TextArea(3, 10)]
    public string[] sentences;
    
    public ChoiceData[] choices;
}