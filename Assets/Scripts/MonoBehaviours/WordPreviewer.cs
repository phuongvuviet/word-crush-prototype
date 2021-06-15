using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WordPreviewer : MonoBehaviour
{
    [SerializeField] Text word; 
    CanvasGroup canvasGroup;

    private void Awake() {
        canvasGroup = GetComponent<CanvasGroup>();
    }
    public void SetWord(string wordStr)
    {
        canvasGroup.alpha = 1;
        word.gameObject.SetActive(true);
        word.text = wordStr;
    }
    public void ResetText()
    {
        canvasGroup.alpha = 0;
        // word.gameObject.SetActive(false);
        // word.text = " ";
        // word.text = "";
    }
}
