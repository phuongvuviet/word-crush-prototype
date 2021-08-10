using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class FloatingLetterViewModel : MovingElement
{
    [Header("Individual properties")]
    [SerializeField] private TextMeshProUGUI letterText;

    public void SetLetterText(char letter)
    {
        letterText.text = letter.ToString();
    }

    public void SetAnchoredPosition(Vector2 anchoredPosition)
    {
        GetComponent<RectTransform>().anchoredPosition = anchoredPosition;
    }

    public void SetRectSize(Vector2 size)
    {
        GetComponent<RectTransform>().sizeDelta = size;
    }
}
