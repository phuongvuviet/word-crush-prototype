using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WordAnswerCell : MonoBehaviour
{
    [SerializeField] Text letterText;

    private void Start()
    {
        letterText.gameObject.SetActive(false);
    }
    public void SetLetter(char letter)
    {
        letterText.text = letter.ToString();
    }
    public void Show()
    {
        letterText.gameObject.SetActive(true);
    }
}
