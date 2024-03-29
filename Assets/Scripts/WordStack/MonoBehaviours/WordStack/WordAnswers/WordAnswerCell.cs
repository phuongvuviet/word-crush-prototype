﻿using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WordAnswerCell : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI letterText;

    // private void Start()
    // {
        // letterText.gameObject.SetActive(false);
    // }
    public void SetLetter(char letter)
    {
        letterText.text = letter.ToString();
        letterText.gameObject.SetActive(false);
    }
    public void Show()
    {
        letterText.gameObject.SetActive(true);
    }
}
