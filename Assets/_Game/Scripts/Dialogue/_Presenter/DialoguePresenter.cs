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
}