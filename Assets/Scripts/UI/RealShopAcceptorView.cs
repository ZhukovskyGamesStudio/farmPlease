using UnityEngine;

public class RealShopAcceptorView : MonoBehaviour {
    [SerializeField]
    private Animation _animation;

    [SerializeField]
    private AnimationClip _idleClip, _buyClip;

    [SerializeField]
    private Rigidbody2D _moneyPrefab, _moneyPrefab1, _moneyPrefab2;

    [SerializeField]
    private Transform _moneyContainer;
    
    [SerializeField]
    private float _impulsePower;

    public void ShowBuyAnimation() {
        _animation.Play(_buyClip.name);
        _animation.PlayQueued(_idleClip.name);
    }

    //triggeredByAnimation
    public void SpewMoney() {
        SpewMoneyFromPrefab(_moneyPrefab);
    }

    public void SpewMoney1() {
        SpewMoneyFromPrefab(_moneyPrefab1);
    }

    public void SpewMoney2() {
        SpewMoneyFromPrefab(_moneyPrefab2);
    }

    private void SpewMoneyFromPrefab(Rigidbody2D prefab) {
        var money = Instantiate(prefab, _moneyContainer);
        money.gameObject.SetActive(true);
        Destroy(money.gameObject, 3);
        money.simulated = true;
        money.AddForce(Quaternion.Euler(Vector3.right * Random.Range(-5, 5f)) * Vector3.down * _impulsePower);
    }
}