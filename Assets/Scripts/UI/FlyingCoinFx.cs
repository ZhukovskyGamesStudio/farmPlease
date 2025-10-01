using Cysharp.Threading.Tasks;
using UI;
using UnityEngine;

public class FlyingCoinFx : FlyingItemFx {
    public void Init(Vector3 parentPosition) {
        transform.position = parentPosition;
        PlayAnimAndDestroy().Forget();
    }



    protected override RectTransform Target => UIHud.Instance.ProfileView.GetComponent<RectTransform>();
}