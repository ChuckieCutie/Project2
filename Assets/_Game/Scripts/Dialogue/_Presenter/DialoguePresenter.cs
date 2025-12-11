using UnityEngine;
using System.Collections.Generic;
using StarterAssets; // Để xử lý input khóa chuột

public class DialoguePresenter : MonoBehaviour
{
    public static DialoguePresenter Instance { get; private set; }

    [Header("Dependencies")]
    [SerializeField] private DialogueUIView _view; // Kéo View vào đây
    private StarterAssetsInputs _inputSystem;

    private Queue<string> _sentencesQueue = new Queue<string>();
    private DialogueData _currentDialogue;

    private void Awake()
    {
        if (Instance != null && Instance != this) Destroy(this);
        else Instance = this;
    }

    private void Start()
    {
        _inputSystem = FindFirstObjectByType<StarterAssetsInputs>();
        
        // Setup sự kiện
        _view.OnNextPressed += HandleNextSentence;
        _view.OnChoiceSelected += HandleChoice;
        
        _view.Show(false);
    }

    // Hàm này được NPCInteractable gọi
    public void StartDialogue(DialogueData dialogue)
    {
        _currentDialogue = dialogue;
        _sentencesQueue.Clear();
        foreach (string s in dialogue.sentences) _sentencesQueue.Enqueue(s);

        SetInputActive(false); // Khóa chuột
        _view.Show(true);
        
        DisplayNextSentence();
    }

    private void HandleNextSentence()
    {
        DisplayNextSentence();
    }

    private void DisplayNextSentence()
    {
        if (_sentencesQueue.Count == 0)
        {
            if (_currentDialogue.choices != null && _currentDialogue.choices.Length > 0)
            {
                _view.ShowChoices(_currentDialogue.choices);
            }
            else
            {
                EndDialogue();
            }
            return;
        }

        string sentence = _sentencesQueue.Dequeue();
        _view.SetContent(_currentDialogue.npcName, sentence);
    }

    private void HandleChoice(Choice choice)
    {
        // Logic cộng điểm nằm ở đây (Presenter điều phối)
        // Lưu ý: Bro cần check null Instance của các Manager khác để tránh lỗi
        if (choice.effectType == DialogueEffect.AddEmpathy && GameDataManager.Instance)
            GameDataManager.Instance.ModifyScore(choice.effectValue, 0);
        
        if (choice.effectType == DialogueEffect.AddReason && GameDataManager.Instance)
            GameDataManager.Instance.ModifyScore(0, choice.effectValue);

        if (choice.effectType == DialogueEffect.AddAffinity && AffinityManager.Instance)
            AffinityManager.Instance.ChangeAffinity(_currentDialogue.npcName, choice.effectValue);

        // Chuyển hội thoại tiếp theo
        if (choice.nextDialogue != null)
            StartDialogue(choice.nextDialogue);
        else
            EndDialogue();
    }

    private void EndDialogue()
    {
        _view.Show(false);
        SetInputActive(true); // Mở khóa chuột
    }

    private void SetInputActive(bool active)
    {
        if (_inputSystem == null) return;
        _inputSystem.cursorLocked = active;
        _inputSystem.cursorInputForLook = active;
        Cursor.lockState = active ? CursorLockMode.Locked : CursorLockMode.None;
        Cursor.visible = !active;
    }
    public void StartDialogueFromNode(DialogueNode node, string npcName)
    {
        // 1. Tạo một ScriptableObject ảo ngay trong RAM (không lưu xuống ổ cứng)
        DialogueData runtimeData = ScriptableObject.CreateInstance<DialogueData>();
        runtimeData.npcName = npcName;

        // 2. Gộp tất cả các loại lines vào 1 list duy nhất để hiển thị
        List<string> content = new List<string>();
        if (node.npc_lines != null) content.AddRange(node.npc_lines);
        if (node.memory_lines != null) content.AddRange(node.memory_lines);
        if (node.task_lines != null) content.AddRange(node.task_lines);
        if (node.hint_lines != null) content.AddRange(node.hint_lines);
        if (node.system_lines != null) content.AddRange(node.system_lines);
        
        runtimeData.sentences = content.ToArray();

        // 3. Chuyển đổi Choices (Lựa chọn)
        if (node.choices != null && node.choices.Count > 0)
        {
            List<Choice> runtimeChoices = new List<Choice>();
            foreach (var cJson in node.choices)
            {
                Choice c = new Choice();
                c.choiceText = cJson.label;
                c.effectType = DialogueEffect.None; // Mặc định

                // Mapping Effects đơn giản (Chỉ lấy cái đầu tiên tìm thấy để demo)
                if (cJson.effects != null)
                {
                    foreach (var eff in cJson.effects)
                    {
                        if (eff.var == "E" && eff.op == "inc") 
                        {
                            c.effectType = DialogueEffect.AddEmpathy;
                            int.TryParse(eff.value, out c.effectValue);
                        }
                        else if (eff.var == "X" && eff.op == "inc")
                        {
                            c.effectType = DialogueEffect.AddReason;
                            int.TryParse(eff.value, out c.effectValue);
                        }
                        else if (eff.var.StartsWith("affinity") && eff.op == "inc")
                        {
                            c.effectType = DialogueEffect.AddAffinity;
                            int.TryParse(eff.value, out c.effectValue);
                        }
                    }
                }
                runtimeChoices.Add(c);
            }
            runtimeData.choices = runtimeChoices.ToArray();
        }

        // 4. Đánh dấu node này đã chơi (quan trọng cho memory shard)
        if (node.once)
        {
            DialogueManager.Instance.MarkNodeAsPlayed(node.id);
            
            // Nếu là Memory Shard, mở khóa trong GameDataManager
            if (node.type == "memory_shard")
            {
                GameDataManager.Instance.UnlockShard(npcName);
            }
        }

        // 5. Gọi lại hàm cũ để hiển thị
        StartDialogue(runtimeData);
    }
}