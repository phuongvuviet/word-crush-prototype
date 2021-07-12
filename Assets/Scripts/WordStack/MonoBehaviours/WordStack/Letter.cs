using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Letter : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI letterText;

    public void SetLetter(char letter) {
        letterText.text = letter.ToString();
    }
    public void SetSize(Vector2 size) {
        RectTransform rectTrans = GetComponent<RectTransform>();
        rectTrans.sizeDelta = size;
    }
}
