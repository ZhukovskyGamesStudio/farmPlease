using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace UI
{
    public class ClickableObject : MonoBehaviour, IPointerClickHandler, IPointerDownHandler, IPointerUpHandler {
        public float MaxDistanceDeltaToRegisterClick;
        
        public UnityEvent RightclickEvent;
        public UnityEvent LeftclickEvent;

        private Vector2 _rightpos, _leftpos;

        public void OnPointerClick(PointerEventData eventData) {
            /*if (eventData.button == PointerEventData.InputButton.Right)
            RightclickEvent.Invoke();
        if (eventData.button == PointerEventData.InputButton.Left)
            LeftclickEvent.Invoke();  */
        }

        public void OnPointerDown(PointerEventData eventData) {
            if (eventData.button == PointerEventData.InputButton.Right)
                _rightpos = eventData.position;
            if (eventData.button == PointerEventData.InputButton.Left)
                _leftpos = eventData.position;
        }

        public void OnPointerUp(PointerEventData eventData) {
            if (eventData.button == PointerEventData.InputButton.Right &&
                (eventData.position - _rightpos).magnitude < MaxDistanceDeltaToRegisterClick)
                RightclickEvent.Invoke();
            if (eventData.button == PointerEventData.InputButton.Left &&
                (eventData.position - _leftpos).magnitude < MaxDistanceDeltaToRegisterClick)
                LeftclickEvent.Invoke();
        }
    }
}