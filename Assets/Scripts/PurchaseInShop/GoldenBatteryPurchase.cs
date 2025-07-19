using MadPixel.InApps;
using UnityEngine;
using UnityEngine.Purchasing;
using UnityEngine.UI;

public class GoldenBatteryPurchase : MonoBehaviour
{
    [SerializeField] private Text priceText;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
   private void Start()
    {
        if (MobileInAppPurchaser.Exist)
        {
            Product battery = MobileInAppPurchaser.Instance.GetProduct("com.zhukovskyGames.FarmPlease.GoldenBattery");
            if (battery != null)
            {
                priceText.text = battery.metadata.localizedPriceString;
            }

            MobileInAppPurchaser.Instance.OnPurchaseResult += OnPurchaseResult;
        }
    }

    public void OnButtonClick()
    {
        MobileInAppPurchaser.BuyProduct("com.zhukovskyGames.FarmPlease.GoldenBattery");

    }

    private void OnPurchaseResult(Product purchasedProduct)
    {
        if (purchasedProduct != null && purchasedProduct.definition.id == "com.zhukovskyGames.FarmPlease.GoldenBattery")
        {
          //  RealShopDialog.BuyGoldenBattery();
            //функционал кнопки
        }
    }
    // Update is called once per frame
    
}
