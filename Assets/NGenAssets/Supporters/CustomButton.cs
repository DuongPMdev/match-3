using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.Events;

public class CustomButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler, IBeginDragHandler, IEndDragHandler {

    [SerializeField] private UnityEvent onClick;

    private Vector2 pointerDownPos;
    private bool isDragging = false;
    private float dragThreshold = 10f;

    public void OnPointerDown(PointerEventData eventData) {
        pointerDownPos = eventData.position;
        isDragging = false;
    }

    public void OnDrag(PointerEventData eventData) {
        if (!isDragging && Vector2.Distance(pointerDownPos, eventData.position) > dragThreshold)
            isDragging = true;

        transform.parent.GetComponentInParent<ScrollRect>()?.OnDrag(eventData);
    }

    public void OnBeginDrag(PointerEventData eventData) {
        transform.parent.GetComponentInParent<ScrollRect>()?.OnBeginDrag(eventData);
    }

    public void OnEndDrag(PointerEventData eventData) {
        transform.parent.GetComponentInParent<ScrollRect>()?.OnEndDrag(eventData);
    }

    public void OnPointerUp(PointerEventData eventData) {
        if (!isDragging) {
            onClick?.Invoke();
        }
    }

}
