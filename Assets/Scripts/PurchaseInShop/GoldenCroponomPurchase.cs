using MadPixel.InApps;
using UnityEngine;
using UnityEngine.Purchasing;
using UnityEngine.UI;

public class GoldenCroponomPurchase : MonoBehaviour
{
    [SerializeField] private Text priceText;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
   private void Start()
    {
        if (MobileInAppPurchaser.Exist)
        {
            Product croponom = MobileInAppPurchaser.Instance.GetProduct("com.zhukovskyGames.FarmPlease.GoldenCroponom");
            if (croponom != null)
            {
                priceText.text = croponom.metadata.localizedPriceString;
            }

            MobileInAppPurchaser.Instance.OnPurchaseResult += OnPurchaseResult;
        }
    }

    public void OnButtonClick()
    {
        MobileInAppPurchaser.BuyProduct("com.zhukovskyGames.FarmPlease.GoldenCroponom");

    }

    private void OnPurchaseResult(Product purchasedProduct)
    {
        if (purchasedProduct != null && purchasedProduct.definition.id == "com.zhukovskyGames.FarmPlease.GoldenCroponom")
        {
            //функционал кнопки
        }
    }
    // Update is called once per frame
    
}
