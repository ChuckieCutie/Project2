using System;

// Interface này quy định những gì View có thể làm
public interface IDialogueView
{
    void Show(bool isVisible);
    void SetContent(string name, string sentence);
    void ShowChoices(Choice[] choices);
    
    event Action OnNextPressed;        // Sự kiện người chơi bấm tiếp
    event Action<Choice> OnChoiceSelected; // Sự kiện người chơi chọn đáp án
}