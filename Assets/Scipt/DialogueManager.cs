using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro; // Cần thư viện này
using UnityEngine.UI; // Cần thư viện này

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

        foreach (Transform child in choiceContainer.transform)
        {
            if (child != choiceButtonTemplate.transform) 
            {
                Destroy(child.gameObject);
            }
        }

        for (int i = 0; i < currentDialogue.choices.Length; i++)
        {
            GameObject choiceButton = Instantiate(choiceButtonTemplate, choiceContainer.transform);
            choiceButton.SetActive(true);

            choiceButton.GetComponentInChildren<TextMeshProUGUI>().text = currentDialogue.choices[i].choiceText;

            int choiceIndex = i; 
            choiceButton.GetComponent<Button>().onClick.AddListener(() => SelectChoice(choiceIndex));
        }
    }

    public void SelectChoice(int index)
    {
        currentDialogue.choices[index].OnChoiceSelected.Invoke();
        EndDialogue();
    }

    void EndDialogue()
    {
        dialogueCanvas.SetActive(false);
    }
}