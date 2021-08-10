using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


namespace WCross
{
    public class LineSegmentViewModel : MonoBehaviour
    {
        [SerializeField] Image _image;
        Vector2 _startPosition, _endPosition;
        RectTransform _rectTransform;
        private Transform _parentTransform;
        private Vector2 _prevMousePos = Vector2.one * -1;

        private void Awake()
        {
            _rectTransform = GetComponent<RectTransform>();
        }

        public Vector2 StartPosition
        {
            get => _startPosition;
            set => _startPosition = value;
        }

        public Vector2 EndPosition
        {
            get => _endPosition;
            set => _endPosition = value;
        }

        public void Init(Transform parent, Vector2 startPos, Vector2 endPos)
        {
            _parentTransform = parent;
            transform.localPosition = parent.InverseTransformPoint(startPos);
            SetEndPosition(endPos);
        }
        public void SetEndPosition(Vector2 mousePos)
        {
            if (mousePos == _prevMousePos) return;
            _prevMousePos = mousePos;
            // Debug.Log("Set end position: " + mousePos);
            Vector2 startPos = _parentTransform.InverseTransformPoint(transform.position); 
            Vector2 endPos = _parentTransform.InverseTransformPoint(mousePos);
            if (Vector2.Distance(startPos, endPos) <= Mathf.Epsilon) return;
            float angle = Mathf.Atan2(endPos.y - startPos.y, endPos.x - startPos.x);
            _rectTransform.sizeDelta = 
                new Vector2(Vector2.Distance(startPos, endPos), _rectTransform.sizeDelta.y);
            transform.localRotation = Quaternion.Euler(new Vector3(0, 0, Mathf.Rad2Deg * angle));
        }
    }
}
