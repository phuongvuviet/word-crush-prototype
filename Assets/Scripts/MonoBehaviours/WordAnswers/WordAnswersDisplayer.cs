using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WordAnswersDisplayer : MonoBehaviour
{
    [SerializeField] WordAnswer answerPrefab;

    List<WordAnswer> answers = new List<WordAnswer>();

    public void SetWordAnswers(List<string> words)
    {
        for (int i = 0; i < answers.Count; i++) {
            Destroy(answers[i].gameObject);
        }
        answers.Clear();
        for (int i = 0; i < words.Count; i++)
        {
            WordAnswer answerInstance = Instantiate(answerPrefab, transform);
            answerInstance.SetAnswer(words[i]);
            answers.Add(answerInstance);
        }
    }
    public void ShowAnswer(List<string> words) {
        for (int i = 0; i < answers.Count; i++)
        {
            if (words.Contains(answers[i].GetAnswer())) {
                answers[i].Show();
            }
        }
    }
    public void ShowAnswer(string word)
    {
        for (int i = 0; i < answers.Count; i++)
        {
            if (answers[i].GetAnswer() == word)
            {
                answers[i].Show();
                break;
            }
        }
    }
}
