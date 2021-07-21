using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;

public class MovingElement : MonoBehaviour
{
    [Header("Moving properties")]
    [SerializeField] Vector2 _parallelRange = new Vector2(.3f, .5f);
    [SerializeField] Vector2 _perpendicularRange = new Vector2(.2f, .3f);
    [SerializeField] private float _moveDuration = .5f;
    [SerializeField] Transform destination;
    [SerializeField] Ease easeType = Ease.Linear;
    [SerializeField] DisplacementDirection perpendicularDirection = DisplacementDirection.RANDOM;

    enum DisplacementDirection
    {
        RANDOM,
        LEFT,
        RIGHT
    }
    
    Vector3 _originPosition;


    public void Move1()
    {
        transform.position = _originPosition;
        MoveTo(destination.position, null);
    }
    public void MoveTo(Vector2 destination, Action callback)
    {
        _originPosition = transform.position;
        Vector3[] path = CreatePathWithControlPoints(_originPosition, destination);
        transform.DOPath(path, _moveDuration, PathType.CatmullRom, PathMode.Sidescroller2D, 10, Color.red)
            .SetEase(easeType).SetLookAt(.1f, Vector3.forward, Vector3.right).OnComplete(() =>
            {
                StartCoroutine(OnComplete(callback));
            });
    }

    IEnumerator OnComplete(Action callback)
    {
        transform.localRotation = Quaternion.identity;
        yield return new WaitForSeconds(.2f);
        callback?.Invoke();
        Destroy(gameObject);
    }
    
    Vector3[] CreatePathWithControlPoints(Vector3 startPosition, Vector3 endPosition)
    {
        float distance = Vector3.Distance(startPosition, endPosition);
        Vector3 direction = (endPosition - startPosition).normalized;
        Vector3 perpendicular = Vector3.Cross(direction, Vector3.forward);
        Vector3[] path = new Vector3[3];
        path[0] = startPosition;
        path[2] = endPosition;
        int perpendicularDirection = GetPerpendicularDirection();
        Vector3 parallelDisplacement = direction * (distance * Random.Range(_parallelRange.x, _parallelRange.y));
        Vector3 perpendicularDisplacement = perpendicular *
                        (perpendicularDirection * distance * Random.Range(_perpendicularRange.x, _perpendicularRange.y));
        path[1] = startPosition + parallelDisplacement + perpendicularDisplacement; 
        return path;
    }

    int GetPerpendicularDirection()
    {
        if (perpendicularDirection == DisplacementDirection.LEFT)
        {
            return -1;
        } else if (perpendicularDirection == DisplacementDirection.RIGHT)
        {
            return 1;
        }
        return (Random.Range(0, 1000000) % 2) == 1 ? 1 : -1;
    }
}
