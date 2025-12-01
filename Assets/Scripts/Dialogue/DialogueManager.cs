using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using StarterAssets; // Gọi bộ điều khiển nhân vật

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager Instance { get; private set; }

    [Header("UI Elements")]
    [SerializeField] private GameObject dialogueCanvas;
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private TextMeshProUGUI sentenceText;
    [SerializeField] private Transform choiceContainer;
    [SerializeField] private GameObject choiceButtonTemplate;

    private Queue<string> sentencesQueue = new Queue<string>();
    private DialogueData currentDialogue;
    
    private StarterAssetsInputs _playerInputs;

    void Awake()
    {
        if (Instance != null && Instance != this) Destroy(gameObject);
        else Instance = this;
    }

    void Start()
    {
        dialogueCanvas.SetActive(false);
        choiceButtonTemplate.SetActive(false);

        // Tìm script input (Dùng lệnh mới cho Unity bản mới)
        _playerInputs = FindFirstObjectByType<StarterAssetsInputs>();
    }

    void Update()
    {
        // Logic bấm chuột để qua câu thoại
        if (dialogueCanvas.activeSelf && !choiceContainer.gameObject.activeSelf)
        {
            if (Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.Space))
            {
                DisplayNextSentence();
            }
        }
    }

    public void StartDialogue(DialogueData dialogue)
    {
        currentDialogue = dialogue;
        dialogueCanvas.SetActive(true);
        choiceContainer.gameObject.SetActive(false);
        sentenceText.gameObject.SetActive(true);

        nameText.text = dialogue.npcName;
        sentencesQueue.Clear();

        foreach (string sentence in dialogue.sentences)
        {
            sentencesQueue.Enqueue(sentence);
        }

        // === GIẢI PHÓNG CON CHUỘT ===
        if (_playerInputs != null)
        {
            _playerInputs.cursorLocked = false;
            _playerInputs.cursorInputForLook = false;
        }
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        // ==============================

        DisplayNextSentence();
    }

    public void DisplayNextSentence()
    {
        if (sentencesQueue.Count == 0)
        {
            if (currentDialogue.choices != null && currentDialogue.choices.Length > 0)
            {
                DisplayChoices();
            }
            else
            {
                EndDialogue();
            }
            return;
        }

        string sentence = sentencesQueue.Dequeue();
        StopAllCoroutines();
        StartCoroutine(TypeSentence(sentence));
    }

    IEnumerator TypeSentence(string sentence)
    {
        sentenceText.text = "";
        foreach (char letter in sentence.ToCharArray())
        {
            sentenceText.text += letter;
            yield return new WaitForSeconds(0.02f);
        }
    }

    void DisplayChoices()
    {
        sentenceText.gameObject.SetActive(false);
        choiceContainer.gameObject.SetActive(true);

        // Xóa nút cũ (trừ template)
        foreach (Transform child in choiceContainer)
        {
            if (child.gameObject != choiceButtonTemplate) Destroy(child.gameObject);
        }

        // Tạo nút mới
        foreach (Choice choice in currentDialogue.choices)
        {
            GameObject btn = Instantiate(choiceButtonTemplate, choiceContainer);
            btn.SetActive(true);
            btn.GetComponentInChildren<TextMeshProUGUI>().text = choice.choiceText;

            // Bắt sự kiện click
            btn.GetComponent<Button>().onClick.AddListener(() => SelectChoice(choice));
        }
    }

    // ĐÃ SỬA: Dùng trực tiếp 'Choice' thay vì 'DialogueData.Choice'
    public void SelectChoice(Choice choice)
    {
        // ĐÃ SỬA: Dùng trực tiếp 'DialogueEffect' thay vì 'DialogueData.DialogueEffect'
        switch (choice.effectType)
        {
            case DialogueEffect.AddEmpathy:
                if (GameDataManager.Instance) 
                    GameDataManager.Instance.ModifyScore(choice.effectValue, 0); 
                break;
            case DialogueEffect.AddReason:
                if (GameDataManager.Instance) 
                    GameDataManager.Instance.ModifyScore(0, choice.effectValue); 
                break;
            case DialogueEffect.AddAffinity:
                if (AffinityManager.Instance) 
                    AffinityManager.Instance.ChangeAffinity(currentDialogue.npcName, choice.effectValue);
                break;
        }

        if (choice.nextDialogue != null)
        {
            StartDialogue(choice.nextDialogue);
        }
        else
        {
            EndDialogue();
        }
    }

    void EndDialogue()
    {
        dialogueCanvas.SetActive(false);

        // === KHÓA LẠI CON CHUỘT ===
        if (_playerInputs != null)
        {
            _playerInputs.cursorLocked = true;
            _playerInputs.cursorInputForLook = true;
        }
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        // ===========================
    }
}