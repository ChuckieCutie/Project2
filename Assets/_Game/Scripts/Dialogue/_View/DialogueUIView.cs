using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;
using System.Collections;

public class DialogueUIView : MonoBehaviour, IDialogueView
{
    [Header("UI References")]
    [SerializeField] private GameObject _canvasObj;
    [SerializeField] private TextMeshProUGUI _nameText;
    [SerializeField] private TextMeshProUGUI _sentenceText;
    [SerializeField] private Transform _choiceContainer;
    [SerializeField] private GameObject _choiceButtonPrefab;

    public event Action OnNextPressed;
    public event Action<Choice> OnChoiceSelected;

    private void Awake() 
    {
        _choiceButtonPrefab.SetActive(false); // Ẩn nút mẫu
    }

    private void Update()
    {
        // View chỉ bắt input và báo lên trên, không xử lý logic game
        if (_canvasObj.activeSelf && !_choiceContainer.gameObject.activeSelf)
        {
            if (Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.Space))
            {
                OnNextPressed?.Invoke();
            }
        }
    }

    // --- IMPLEMENT INTERFACE ---

    public void Show(bool isVisible)
    {
        _canvasObj.SetActive(isVisible);
    }

    public void SetContent(string name, string sentence)
    {
        _nameText.text = name;
        _sentenceText.gameObject.SetActive(true);
        _choiceContainer.gameObject.SetActive(false);
        
        StopAllCoroutines();
        StartCoroutine(TypeSentenceEffect(sentence));
    }

    public void ShowChoices(Choice[] choices)
    {
        _sentenceText.gameObject.SetActive(false);
        _choiceContainer.gameObject.SetActive(true);

        // Clear nút cũ
        foreach (Transform child in _choiceContainer)
        {
            if (child.gameObject != _choiceButtonPrefab) Destroy(child.gameObject);
        }

        // Tạo nút mới
        foreach (var choice in choices)
        {
            var btnObj = Instantiate(_choiceButtonPrefab, _choiceContainer);
            btnObj.SetActive(true);
            btnObj.GetComponentInChildren<TextMeshProUGUI>().text = choice.choiceText;
            
            // Add listener
            btnObj.GetComponent<Button>().onClick.AddListener(() => {
                OnChoiceSelected?.Invoke(choice);
            });
        }
    }

    private IEnumerator TypeSentenceEffect(string sentence)
    {
        _sentenceText.text = "";
        foreach (char letter in sentence.ToCharArray())
        {
            _sentenceText.text += letter;
            yield return new WaitForSeconds(0.02f);
        }
    }
}