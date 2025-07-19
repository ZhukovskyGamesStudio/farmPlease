using MadPixel.InApps;
using UnityEngine;
using UnityEngine.Purchasing;
using UnityEngine.UI;

public class GoldenClockPurchase : MonoBehaviour
{
    [SerializeField] private Text priceText;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
   private void Start()
    {
        if (MobileInAppPurchaser.Exist)
        {
            Product clock = MobileInAppPurchaser.Instance.GetProduct("com.zhukovskyGames.FarmPlease.GoldenClock");
            if (clock != null)
            {
                priceText.text = clock.metadata.localizedPriceString;
            }

            MobileInAppPurchaser.Instance.OnPurchaseResult += OnPurchaseResult;
        }
    }

    public void OnButtonClick()
    {
        MobileInAppPurchaser.BuyProduct("com.zhukovskyGames.FarmPlease.GoldenClock");

    }

    private void OnPurchaseResult(Product purchasedProduct)
    {
        if (purchasedProduct != null && purchasedProduct.definition.id == "com.zhukovskyGames.FarmPlease.GoldenClock")
        {
            //функционал кнопки
        }
    }
    // Update is called once per frame
    
}
