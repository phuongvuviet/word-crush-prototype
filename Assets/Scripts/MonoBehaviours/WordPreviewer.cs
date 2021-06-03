using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WordPreviewer : MonoBehaviour
{
    [SerializeField] Text word; 

    public void SetWord(string wordStr)
    {
        word.text = wordStr;
    }
    public void ResetText()
    {
        word.text = " ";
    }

}
