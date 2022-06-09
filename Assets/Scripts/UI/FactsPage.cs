using UnityEngine;
using UnityEngine.UI;

public class FactsPage : MonoBehaviour {
    [Header("FactsPage")]
    public Text FactsHeader;

    public Text FactsFirstText;
    public Text FactsSecondText;
    public Image FactsFirstImage;
    public Image FactsSecondImage;

    public void UpdatePage(SOWithCroponomPage pageData) {
        FactsHeader.text = pageData.header;
        FactsFirstText.text = pageData.firstText;
        FactsSecondText.text = pageData.secondText;

        if (pageData.firstSprite != null) {
            FactsFirstImage.enabled = true;
            FactsFirstImage.sprite = pageData.firstSprite;
        } else {
            FactsFirstImage.enabled = false;
        }

        if (pageData.secondSprite != null) {
            FactsSecondImage.enabled = true;
            FactsSecondImage.sprite = pageData.secondSprite;
        } else {
            FactsSecondImage.enabled = false;
        }
    }
}