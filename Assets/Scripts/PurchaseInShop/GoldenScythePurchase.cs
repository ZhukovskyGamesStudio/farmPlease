using MadPixel.InApps;
using UnityEngine;
using UnityEngine.Purchasing;
using UnityEngine.UI;

public class GoldenScythePurchase : MonoBehaviour
{
    [SerializeField] private Text priceText;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
   private void Start()
    {
        if (MobileInAppPurchaser.Exist)
        {
            Product scythe = MobileInAppPurchaser.Instance.GetProduct("com.zhukovskyGames.FarmPlease.GoldenScythe");
            if (scythe != null)
            {
                priceText.text = scythe.metadata.localizedPriceString;
            }

            MobileInAppPurchaser.Instance.OnPurchaseResult += OnPurchaseResult;
        }
    }

    public void OnButtonClick()
    {
        MobileInAppPurchaser.BuyProduct("com.zhukovskyGames.FarmPlease.GoldenScythe");

    }

    private void OnPurchaseResult(Product purchasedProduct)
    {
        if (purchasedProduct != null && purchasedProduct.definition.id == "com.zhukovskyGames.FarmPlease.GoldenScythe")
        {
            //функционал кнопки
        }
    }
    // Update is called once per frame
    
}
