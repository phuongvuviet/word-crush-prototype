using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using TMPro.EditorUtilities;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace WCross
{
    public class WheelLetterViewModel : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _letterTxt;
        [SerializeField] private Image _circleImage;
        [SerializeField] private Color _normalColor, _selectedColor;

        private void Start()
        {
            Unhighlight();
        }

        public void Highlight()
        {
            _circleImage.color = _selectedColor;
        }

        public void Unhighlight()
        {
            _circleImage.color = _normalColor; 
        }

        private char _letter;

        public void SetLetter(char letter)
        {
            _letterTxt.text = letter.ToString();
            this._letter = letter;
        }

        public char GetLetter()
        {
            return _letter;
        }
    }
}
