using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class ClickableObject : MonoBehaviour, IPointerClickHandler, IPointerDownHandler, IPointerUpHandler {
    public float MaxDistanceDeltaToRegisterClick;

    public UnityEvent RightclickEvent;
    public UnityEvent LeftclickEvent;

    private Vector2 rightpos, leftpos;

    public void OnPointerClick(PointerEventData eventData) {
        /*if (eventData.button == PointerEventData.InputButton.Right)
            RightclickEvent.Invoke();
        if (eventData.button == PointerEventData.InputButton.Left)
            LeftclickEvent.Invoke();  */
    }

    public void OnPointerDown(PointerEventData eventData) {
        if (eventData.button == PointerEventData.InputButton.Right)
            rightpos = eventData.position;
        if (eventData.button == PointerEventData.InputButton.Left)
            leftpos = eventData.position;
    }

    public void OnPointerUp(PointerEventData eventData) {
        if (eventData.button == PointerEventData.InputButton.Right &&
            (eventData.position - rightpos).magnitude < MaxDistanceDeltaToRegisterClick)
            RightclickEvent.Invoke();
        if (eventData.button == PointerEventData.InputButton.Left &&
            (eventData.position - leftpos).magnitude < MaxDistanceDeltaToRegisterClick)
            LeftclickEvent.Invoke();
    }
}