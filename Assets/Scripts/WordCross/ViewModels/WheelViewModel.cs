using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;


namespace WCross
{
    public class WheelViewModel : MonoBehaviour
    {
        [SerializeField] private WheelLetterViewModel _wheelLetterViewModelPrefab;
        [SerializeField] private float _wheelRadius;
        [SerializeField] private string _word;
        [SerializeField] private SellectingWordViewModel _sellectingWordViewModel;
        private List<WheelLetterViewModel> _letterViewModels = new List<WheelLetterViewModel>();
        public string curSellectingWord = "";
        List<WheelLetterViewModel> _sellectingWheelLetters = new List<WheelLetterViewModel>(); 
        
        Action<string> _onSellectingWordChanged = null; 

        private void Start()
        {
            float letterAngle = 2 * Mathf.PI / _word.Length;
            int n = _word.Length;
            float curAngle = Mathf.PI / 2f;
            for (int i = 0; i < n; ++i)
            {
                WheelLetterViewModel letterViewModelInstance = Instantiate(_wheelLetterViewModelPrefab, transform);
                letterViewModelInstance.SetLetter(_word[i]);
                Vector2 pos = _wheelRadius * new Vector2(Mathf.Cos(curAngle), Mathf.Sin(curAngle));
                letterViewModelInstance.GetComponent<RectTransform>().anchoredPosition = pos;
                _letterViewModels.Add(letterViewModelInstance);
                // Debug.Log("cur angle: " + curAngle);
                // Debug.Log(new Vector2(Mathf.Cos(curAngle), Mathf.Sin(curAngle)));
                curAngle += letterAngle;
            }
        }

        private void Update()
        {
            if (Input.GetMouseButton(0))
            {
                WheelLetterViewModel wheelLetterUnderMouse = GetLetterUnderMouse();
                if (ReferenceEquals(wheelLetterUnderMouse, null))
                {
                    return;
                }
                wheelLetterUnderMouse.Highlight();
                if (curSellectingWord == "")
                {
                    curSellectingWord = wheelLetterUnderMouse.GetLetter().ToString();
                    _sellectingWheelLetters.Add(wheelLetterUnderMouse);
                    _sellectingWordViewModel.ChangeSelectingWord(curSellectingWord);
                }
                else
                {
                    if (_sellectingWheelLetters.Contains(wheelLetterUnderMouse))
                    {
                        int curConnectingWordLength = _sellectingWheelLetters.Count; 
                        if (curConnectingWordLength >= 2)
                        {
                            // if mouse point to pre selected letter, pop last letter
                            if (_sellectingWheelLetters[curConnectingWordLength - 2] == wheelLetterUnderMouse)
                            {
                                _sellectingWheelLetters[curConnectingWordLength - 1].Unhighlight();
                                _sellectingWheelLetters.RemoveAt(curConnectingWordLength - 1);
                                curSellectingWord = curSellectingWord.Remove(curConnectingWordLength - 1);
                                _sellectingWordViewModel.ChangeSelectingWord(curSellectingWord);
                            } 
                        }
                    }
                    else
                    {
                        curSellectingWord += wheelLetterUnderMouse.GetLetter();
                        _sellectingWheelLetters.Add(wheelLetterUnderMouse);
                        _sellectingWordViewModel.ChangeSelectingWord(curSellectingWord);
                    }
                }
            } else if (Input.GetMouseButtonUp(0))
            {
                if (curSellectingWord.Length > 0)
                {
                    ResetSelectingWord();
                }
            }
        }

        void ResetSelectingWord()
        {
            curSellectingWord = "";
            for (int i = 0; i < _sellectingWheelLetters.Count; i++)
            {
                _sellectingWheelLetters[i].Unhighlight();
            }
            _sellectingWheelLetters.Clear();
            _sellectingWordViewModel.ChangeSelectingWord(curSellectingWord);
        }

        WheelLetterViewModel GetLetterUnderMouse()
        {
            PointerEventData pointerData = new PointerEventData (EventSystem.current)
            {
                pointerId = -1,
            };
            pointerData.position = Input.mousePosition;
            List<RaycastResult> results = new List<RaycastResult>();
            EventSystem.current.RaycastAll(pointerData, results);
            for (int i = 0; i < results.Count; i++)
            {
                if (results[i].gameObject.CompareTag("WheelLetter"))
                {
                    return results[i].gameObject.GetComponent<WheelLetterViewModel>();
                }
            }
            return null;
        }
    }
}
