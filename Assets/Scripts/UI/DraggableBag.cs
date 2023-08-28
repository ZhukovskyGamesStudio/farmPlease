using UnityEngine;
using UnityEngine.Events;

public class DraggableBag : MonoBehaviour
{
    private bool _isDragging;
    private Vector3 _startingPos;
    public UnityEvent OnTryBuy;
    public Rect _buyingBagRect;

    public void BeginDrag()
    {
        _startingPos = transform.position;
    }

    public void Drag()
    {
        Vector3 newpos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        newpos.z = 0;
        transform.position = newpos;
    }


    public void Drop()
    {
        TryBuy();
    }

    public void EndDrag()
    {
        GoBackToPlace();
    }

    private void TryBuy()
    {
        if (IsInBuyingBag)
        {
            OnTryBuy?.Invoke();
        }
    }

    private bool IsInBuyingBag => _buyingBagRect.Contains(Input.mousePosition);


    private void GoBackToPlace()
    {
        _isDragging = false;
        transform.position = _startingPos;
    }
}