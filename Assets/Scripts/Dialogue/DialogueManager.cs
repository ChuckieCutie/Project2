using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
// QUAN TRỌNG: Dòng này để gọi bộ điều khiển nhân vật
using StarterAssets;

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager Instance { get; private set; }

    [Header("UI Elements")]
    public GameObject dialogueCanvas;
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI sentenceText;
    public GameObject choiceContainer;
    public GameObject choiceButtonTemplate;

    private Queue<string> sentencesQueue;
    private DialogueData currentDialogue;

    // Biến để điều khiển chuột của nhân vật
    private StarterAssetsInputs _playerInputs;

    void Awake()
    {
        if (Instance != null && Instance != this) Destroy(gameObject);
        else Instance = this;

        sentencesQueue = new Queue<string>();
    }

    void Start()
    {
        dialogueCanvas.SetActive(false);
        choiceButtonTemplate.SetActive(false);

        // Tự động tìm script điều khiển trên người nhân vật
        _playerInputs = FindObjectOfType<StarterAssetsInputs>();
    }

    void Update()
    {
        // Logic bấm chuột để qua câu thoại
        if (dialogueCanvas.activeSelf && !choiceContainer.activeSelf)
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
        choiceContainer.SetActive(false);
        sentenceText.gameObject.SetActive(true);

        nameText.text = dialogue.npcName;
        sentencesQueue.Clear();

        foreach (string sentence in dialogue.sentences)
        {
            sentencesQueue.Enqueue(sentence);
        }

        // === GIẢI PHÓNG CON CHUỘT ===
        // 1. Báo cho bộ điều khiển nhân vật dừng xoay camera
        if (_playerInputs != null)
        {
            _playerInputs.cursorLocked = false;
            _playerInputs.cursorInputForLook = false;
        }
        // 2. Hiện con trỏ chuột của Windows lên
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
        choiceContainer.SetActive(true);

        // Xóa các nút cũ (trừ cái nút mẫu)
        foreach (Transform child in choiceContainer.transform)
        {
            if (child.gameObject != choiceButtonTemplate)
            {
                Destroy(child.gameObject);
            }
        }

        // Tạo nút mới
        for (int i = 0; i < currentDialogue.choices.Length; i++)
        {
            GameObject choiceButton = Instantiate(choiceButtonTemplate, choiceContainer.transform);
            choiceButton.SetActive(true); // Bật nút lên (quan trọng!)

            choiceButton.GetComponentInChildren<TextMeshProUGUI>().text = currentDialogue.choices[i].choiceText;

            int choiceIndex = i;
            choiceButton.GetComponent<Button>().onClick.AddListener(() => SelectChoice(choiceIndex));
        }
    }

    public void SelectChoice(int index)
    {
        Choice selectedChoice = currentDialogue.choices[index];

        // Xử lý cộng điểm E/X/Affinity
        switch (selectedChoice.effectType)
        {
            case DialogueEffect.AddEmpathy:
                if (GameDataManager.Instance) GameDataManager.Instance.AddEmpathy(selectedChoice.effectValue);
                break;
            case DialogueEffect.AddReason:
                if (GameDataManager.Instance) GameDataManager.Instance.AddReason(selectedChoice.effectValue);
                break;
            case DialogueEffect.AddAffinity:
                if (AffinityManager.Instance) AffinityManager.Instance.ChangeAffinity(currentDialogue.npcName, selectedChoice.effectValue);
                break;
        }

        // Chuyển sang hội thoại tiếp theo (nếu có)
        if (selectedChoice.nextDialogue != null)
        {
            StartDialogue(selectedChoice.nextDialogue);
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
        // 1. Cho phép nhân vật xoay camera trở lại
        if (_playerInputs != null)
        {
            _playerInputs.cursorLocked = true;
            _playerInputs.cursorInputForLook = true;
        }
        // 2. Ẩn con trỏ chuột đi
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        // ===========================
    }
}