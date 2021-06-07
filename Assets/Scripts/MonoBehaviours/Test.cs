using UnityEngine;
using UnityEngine.EventSystems;

public class Test : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerEnterHandler
{
    public void OnDrag(PointerEventData eventData)
    {
        Debug.Log("Pointer drag");
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        Debug.LogError("Pointer down");
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        Debug.Log("Pointer enter");
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        Debug.Log("Pointer exit");
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        Debug.Log("Pointer up");
    }
}
