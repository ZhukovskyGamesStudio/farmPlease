using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace UI{
    public class DraggableUIElement : MonoBehaviour{
        [SerializeField]
        private List<RectEventMap> _rectEvents;

        [SerializeField]
        private bool _horizontal = true, _vertical = true;

        [SerializeField]
        private Vector2 _clampHorizontal = new Vector2(-2000, 2000),
            _clampVertical = new Vector2(-2000, 2000);

        private bool _isDragging;
        private Vector3 _startingPos;
        private Coroutine _smoothingCoroutine;
        private Vector3 shift;
        public Func<bool> IsActive;

        [SerializeField]
        private EventTrigger _eventTrigger;

        private void Awake(){
            SubscribeEventTrigger();
        }

        public void SetIsActiveFunc(Func<bool> func){
            IsActive = null;
            IsActive = func;
        }

        public void SetClampValues(bool isVertical, Vector2 values){
            if (isVertical){
                _clampVertical = values;
            } else{
                _clampHorizontal = values;
            }
        }

        private void SubscribeEventTrigger(){
            EventTrigger eventTrigger = _eventTrigger;

            var entry = new EventTrigger.Entry();
            entry.eventID = EventTriggerType.BeginDrag;
            entry.callback.AddListener(delegate{ BeginDrag(); });
            eventTrigger.triggers.Add(entry);

            entry = new EventTrigger.Entry();
            entry.eventID = EventTriggerType.EndDrag;
            entry.callback.AddListener(delegate{ EndDrag(); });
            eventTrigger.triggers.Add(entry);

            entry = new EventTrigger.Entry();
            entry.eventID = EventTriggerType.Drag;
            entry.callback.AddListener(delegate{ Drag(); });
            eventTrigger.triggers.Add(entry);

            entry = new EventTrigger.Entry();
            entry.eventID = EventTriggerType.Drop;
            entry.callback.AddListener(delegate{ Drop(); });
            eventTrigger.triggers.Add(entry);
        }

        public void BeginDrag(){
            if (IsActive != null && !IsActive()){
                return;
            }

            _startingPos = transform.position;
            shift = Camera.main.ScreenToWorldPoint(Input.mousePosition) - _startingPos;
            if (_smoothingCoroutine != null){
                StopCoroutine(_smoothingCoroutine);
            }
        }

        public void Drag(){
            if (IsActive != null && !IsActive()){
                return;
            }

            Vector3 newpos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            newpos -= shift;
            newpos = ClampMovement(newpos);
            transform.position = newpos;
        }

        private Vector3 ClampMovement(Vector3 newpos){
            newpos.x = _horizontal
                ? Mathf.Clamp(newpos.x, _clampHorizontal.x, _clampHorizontal.y)
                : transform.position.x;

            newpos.y = _vertical
                ? Mathf.Clamp(newpos.y, _clampVertical.x, _clampVertical.y)
                : transform.position.y;

            newpos.z = 0;
            return newpos;
        }

        public void Drop(){
            if (IsActive != null && !IsActive()){
                return;
            }

            TryInvokeRectEvents();
        }

        public void EndDrag(){
            if (IsActive != null && !IsActive()){
                return;
            }

            GoBackToPlace();
        }

        private void TryInvokeRectEvents(){
            List<UnityEvent> events = _rectEvents
                .Where(re => re.CheckMouseInside)
                .Select(rectEvent => rectEvent.EndDragInsideEvent).ToList();

            foreach (var needToInvokeEvent in events){
                needToInvokeEvent?.Invoke();
            }
        }

        public void GoBackToPlace(){
            _isDragging = false;
            transform.position = _startingPos;
        }

        public void GoBackToPlaceSmooth(){
            if (_smoothingCoroutine != null){
                StopCoroutine(_smoothingCoroutine);
            }

            _smoothingCoroutine = StartCoroutine(SmoothToPlace());
        }

        private IEnumerator SmoothToPlace(){
            _isDragging = false;
            transform.position = _startingPos;
            yield break;
        }

        public void ChangeStartingPlace(Vector3 startingPlace){
            _startingPos = startingPlace;
        }

        protected void ChangeRectEventActive(int index, bool newValue){
            if (_rectEvents.Count > index){
                _rectEvents[index].SetActive(newValue);
            }
        }
    }

    [System.Serializable]
    public class RectEventMap{
        [SerializeField]
        private bool _isActive = true;

        public UnityEvent EndDragInsideEvent;

        [SerializeField]
        private Rect _rect;

        public void SetActive(bool value) => _isActive = value;

        public bool CheckMouseInside => _isActive && _rect.Contains(Input.mousePosition);
    }
}