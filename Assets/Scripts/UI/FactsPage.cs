using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FactsPage : MonoBehaviour
{

    [Header("FactsPage")]
    public Text FactsHeader;
    public Text FactsFirstText;
    public Text FactsSecondText;
    public Image FactsFirstImage;
    public Image FactsSecondImage;


    public void UpdatePage(string header, string firstText, Sprite firstsprite, string secondText, Sprite secondSprite)
    {
        FactsHeader.text = header;
        FactsFirstText.text = firstText;
        FactsSecondText.text = secondText;

        if (firstsprite != null)
        {
            FactsFirstImage.enabled = true;
            FactsFirstImage.sprite = firstsprite;
        }
        else
            FactsFirstImage.enabled = false;


        if (secondSprite != null)
        {
            FactsSecondImage.enabled = true;
            FactsSecondImage.sprite = secondSprite;
        }
           else
            FactsSecondImage.enabled = false;
    }
}
