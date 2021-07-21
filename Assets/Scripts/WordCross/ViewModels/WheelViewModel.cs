using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;


namespace WCross
{
    public class WheelViewModel : MonoBehaviour
    {
        [SerializeField] private WheelLetterViewModel _wheelLetterViewModelPrefab;
        [SerializeField] private float _wheelRadius;
        [SerializeField] private SelectingWordViewModel _selectingWordViewModel;
        [SerializeField] private LineSegmentViewModel _lineSegmentPrefab;
        [SerializeField] private RectTransform _lineSegmentHolder;
        
        List<WheelLetterViewModel> _letterViewModels = new List<WheelLetterViewModel>();
        List<WheelLetterViewModel> _selectingWheelLetters = new List<WheelLetterViewModel>(); 
        List<LineSegmentViewModel> _lineSegments = new List<LineSegmentViewModel>();
        List<Vector2> _letterAnchoredPositions = new List<Vector2>();
        
        string _curSelectingWord = "";
        string _letters;
        PointerEventData _pointerData = new PointerEventData (EventSystem.current)
        {
            pointerId = -1,
        };
        
        // Cache main camera
        Camera _mainCamera;
        
        Action<string> _onWordSelected = null;

        public void Init(string letters, Action<string> onWordSelected)
        {
            _letters = letters;
            _mainCamera = Camera.main;
            _onWordSelected = onWordSelected;
            InitWheelLetters();
        }

        private void InitWheelLetters()
        {
            CleanUpLetters();
            float letterAngle = 2 * Mathf.PI / _letters.Length;
            int n = _letters.Length;
            float curAngle = Mathf.PI / 2f;
            for (int i = 0; i < n; ++i)
            {
                WheelLetterViewModel letterViewModelInstance = Instantiate(_wheelLetterViewModelPrefab, transform);
                letterViewModelInstance.SetLetter(_letters[i]);
                Vector2 pos = _wheelRadius * new Vector2(Mathf.Cos(curAngle), Mathf.Sin(curAngle));
                letterViewModelInstance.GetComponent<RectTransform>().anchoredPosition = pos;
                _letterViewModels.Add(letterViewModelInstance);
                curAngle += letterAngle;
                _letterAnchoredPositions.Add(letterViewModelInstance.GetComponent<RectTransform>().anchoredPosition);
            }
        }

        void CleanUpLetters()
        {
            for (int i = 0; i < _letterViewModels.Count; i++)
            {
                Destroy(_letterViewModels[i].gameObject);
            }
            _letterViewModels.Clear();
            _letterAnchoredPositions.Clear();
        }

        private void Update()
        {
            if (Input.GetMouseButton(0))
            {
                WheelLetterViewModel wheelLetterUnderMouse = GetLetterUnderMouse();
                if (!ReferenceEquals(wheelLetterUnderMouse, null))
                {
                    wheelLetterUnderMouse.Highlight();
                    if (_curSelectingWord == "")
                    {
                        _curSelectingWord = wheelLetterUnderMouse.GetLetter().ToString();
                        _selectingWheelLetters.Add(wheelLetterUnderMouse);
                        _selectingWordViewModel.ChangeSelectingWord(_curSelectingWord);
                        InitNewWheelLetterGO(wheelLetterUnderMouse);
                    }
                    else
                    {
                        if (_selectingWheelLetters.Contains(wheelLetterUnderMouse))
                        {
                            int curConnectingWordLength = _selectingWheelLetters.Count; 
                            if (curConnectingWordLength >= 2)
                            {
                                // if mouse point to pre selected letter, pop last letter and last line segment
                                if (_selectingWheelLetters[curConnectingWordLength - 2] == wheelLetterUnderMouse)
                                {
                                    _selectingWheelLetters[curConnectingWordLength - 1].Unhighlight();
                                    _selectingWheelLetters.RemoveAt(curConnectingWordLength - 1);
                                    _curSelectingWord = _curSelectingWord.Remove(curConnectingWordLength - 1);
                                    _selectingWordViewModel.ChangeSelectingWord(_curSelectingWord);
                                    
                                    Destroy(_lineSegments[_lineSegments.Count - 1].gameObject);
                                    _lineSegments.RemoveAt(_lineSegments.Count - 1);
                                } 
                            }
                        }
                        else
                        {
                            _curSelectingWord += wheelLetterUnderMouse.GetLetter();
                            _selectingWheelLetters.Add(wheelLetterUnderMouse);
                            _selectingWordViewModel.ChangeSelectingWord(_curSelectingWord);
                            _lineSegments[_lineSegments.Count - 1].SetEndPosition(wheelLetterUnderMouse.transform.position);
                            InitNewWheelLetterGO(wheelLetterUnderMouse);
                        }
                    }
                }
                if (_lineSegments.Count > 0)
                {
                    _lineSegments[_lineSegments.Count - 1]
                    .SetEndPosition(GetMouseWorldPosition());
                }
            } else if (Input.GetMouseButtonUp(0))
            {
                if (_curSelectingWord.Length > 0)
                {
                    _onWordSelected?.Invoke(_curSelectingWord);
                    ResetSelectingWordAndLineSegments();
                }
            }
        }

        private void InitNewWheelLetterGO(WheelLetterViewModel wheelLetterUnderMouse)
        {
            LineSegmentViewModel lineSegmentInstance = Instantiate(_lineSegmentPrefab, _lineSegmentHolder);
            lineSegmentInstance.Init(_lineSegmentHolder, wheelLetterUnderMouse.transform.position,
                GetMouseWorldPosition());
            _lineSegments.Add(lineSegmentInstance);
        }

        Vector2 GetMouseWorldPosition()
        {
            return _mainCamera.ScreenToWorldPoint(Input.mousePosition);
        }

        void ResetSelectingWordAndLineSegments()
        {
            _curSelectingWord = "";
            for (int i = 0; i < _selectingWheelLetters.Count; i++)
            {
                _selectingWheelLetters[i].Unhighlight();
            }
            _selectingWheelLetters.Clear();
            _selectingWordViewModel.ChangeSelectingWord(_curSelectingWord);
            for (int i = 0; i < _lineSegments.Count; i++)
            {
                Destroy(_lineSegments[i].gameObject);
            }
            _lineSegments.Clear();
        }

        WheelLetterViewModel GetLetterUnderMouse()
        {
            _pointerData.position = Input.mousePosition;
            List<RaycastResult> results = new List<RaycastResult>();
            EventSystem.current.RaycastAll(_pointerData, results);
            for (int i = 0; i < results.Count; i++)
            {
                if (results[i].gameObject.CompareTag("WheelLetter"))
                {
                    return results[i].gameObject.GetComponent<WheelLetterViewModel>();
                }
            }
            return null;
        }

        Coroutine _coShuffleLetters = null;
        public void ShuffleWheelLetters()
        {
            Utilities.Shuffle(_letterAnchoredPositions);
            if (_coShuffleLetters != null)
            {
                StopCoroutine(_coShuffleLetters);
            }
            for (int i = 0; i < _letterViewModels.Count; i++)
            {
                _letterViewModels[i].GetComponent<RectTransform>().DOKill();
            }
            _coShuffleLetters = StartCoroutine((COShuffleWheelLetters(_letterAnchoredPositions)));
        }

        public IEnumerator COShuffleWheelLetters(List<Vector2> newPositions)
        {
            for (int i = 0; i < _letterViewModels.Count; i++)
            {
                _letterViewModels[i].GetComponent<RectTransform>()
                    .DOAnchorPos(Vector2.zero, .2f).SetEase(Ease.Linear);
            }
            yield return new WaitForSeconds(.25f);
            for (int i = 0; i < _letterViewModels.Count; i++)
            {
                _letterViewModels[i].GetComponent<RectTransform>().DOAnchorPos(newPositions[i], .2f)
                    .SetEase(Ease.Linear);
            }
        }
    }
}
