using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WordAnswersDisplayer : MonoBehaviour
{
    [SerializeField] WordAnswer answerPrefab;
    [SerializeField] AnswerShower endingAnswerPrefab;
    [SerializeField] RectTransform endingAnswerParent;

    List<WordAnswer> answers = new List<WordAnswer>();
    List<AnswerShower> endingAnswers = new List<AnswerShower>();


    public void SetWordAnswers(List<string> words)
    {
        for (int i = 0; i < answers.Count; i++) {
            Destroy(answers[i].gameObject);
        }
        answers.Clear();
        for (int i = 0; i < endingAnswers.Count; i++) {
            Destroy(endingAnswers[i].gameObject);
        }
        endingAnswers.Clear();
        for (int i = 0; i < words.Count; i++)
        {
            WordAnswer answerInstance = Instantiate(answerPrefab, transform);
            answerInstance.SetAnswer(words[i]);
            answers.Add(answerInstance);

            AnswerShower endingAnswer = Instantiate(endingAnswerPrefab, endingAnswerParent);
            // Debug.Log("World position: " + answerInstance.transform.position);
            endingAnswer.gameObject.SetActive(false);
            endingAnswer.SetWord(words[i]);
            endingAnswers.Add(endingAnswer);
        }
        StartCoroutine(DelaySetPosition());
    }
    IEnumerator DelaySetPosition() {
        yield return new WaitForSeconds(.5f);
        for (int i = 0; i < endingAnswers.Count; i++) {
            endingAnswers[i].transform.position = answers[i].transform.position;
        }
    }
    public void ShowAnswers(List<string> words) {
        for (int i = 0; i < words.Count; i++) {
            ShowAnswer(words[i]);
        }
    }
    public void ShowAnswer(string word)
    {
        for (int i = 0; i < answers.Count; i++)
        {
            if (endingAnswers[i].GetWord() == word)
            {
                endingAnswers[i].gameObject.SetActive(true);
                endingAnswers[i].Show();
                break;
            }
        }
    }
    public List<Vector2> GetLetterPositions(string word) {
        for (int i = 0; i < answers.Count; i++)
        {
            if (answers[i].GetAnswer() == word)
            {
                return answers[i].GetLetterPositions();
            }
        }
        return null;
    }

    public void HideAnswerWords() {
        GetComponent<CanvasGroup>().alpha = 0f;
        GetComponent<CanvasGroup>().interactable = false;
    }
    public void HideEndingWords() {
        endingAnswerParent.GetComponent<CanvasGroup>().alpha = 0f;
        endingAnswerParent.GetComponent<CanvasGroup>().interactable = false;
    }
    public void ShowAnswerWords() {
        GetComponent<CanvasGroup>().alpha = 1f;
        GetComponent<CanvasGroup>().interactable = true;
    }
    public void ShowEndingWords() {
        endingAnswerParent.GetComponent<CanvasGroup>().alpha = 1f;
        endingAnswerParent.GetComponent<CanvasGroup>().interactable = true;
    }
}
